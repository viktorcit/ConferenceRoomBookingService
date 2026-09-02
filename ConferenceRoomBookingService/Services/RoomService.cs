using ConferenceRoomBookingService.Data;
using ConferenceRoomBookingService.Data.DTO;
using ConferenceRoomBookingService.Data.DTO.RequestDto.Room;
using ConferenceRoomBookingService.Data.DTO.ResponseDto.Room;
using ConferenceRoomBookingService.Entity;
using ConferenceRoomBookingService.Enums;
using ConferenceRoomBookingService.Helpers;
using ConferenceRoomBookingService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConferenceRoomBookingService.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<RoomService> _logger;
        public RoomService(AppDbContext db, ILogger<RoomService> logger)
        {
            _db = db;
            _logger = logger;
        }


        public async Task<BaseResponseDto<List<RoomResponseDto>>> GetAvailableRooms(GetAvailableRoomsRequestDto request)
        {
            var availableRooms = await _db.Rooms
                .AsNoTracking()
                .Where(r => r.Capacity >= request.Capacity)
                .Where(r => !_db.RoomBookings.Any(b => b.RoomId == r.Id
                && ((b.StartTime < request.EndTime && b.EndTime > request.StartTime))
                    && b.Date == request.Date))
                .ToListAsync();
            if (availableRooms.Count == 0)
            {
                return ResponseFactory.Fail<List<RoomResponseDto>>(
                    ResponseType.NotFound,
                    "No rooms found matching your criteria.");
            }

            var availableRoomIds = availableRooms.Select(r => r.Id).ToList();

            var roomAmenities = await _db.RoomAmenities
                .AsNoTracking()
                .Where(ra => availableRoomIds.Contains(ra.RoomId))
                .Include(ra => ra.Amenity)
                .ToListAsync();

            var roomResponse = availableRooms.Select(room => new RoomResponseDto
            {
                RoomId = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Amenities = roomAmenities
                    .Where(ra => ra.RoomId == room.Id)
                    .Select(ra => ra.Amenity.Name)
                    .ToList()
            }).ToList();

            return ResponseFactory.Ok(roomResponse);
        }

        public async Task<BaseResponseDto> CreateConferenceRoomAsync(CreateRoomRequestDto request)
        {
            var roomExist = await _db.Rooms
                .AsNoTracking()
                .AnyAsync(r => r.Name == request.Name);
            if (roomExist)
            {
                _logger.LogWarning("Room with name {RoomName} already exists.", request.Name);
                return ResponseFactory.Fail(ResponseType.Conflict, "Room with same name already exist!");
            }

            var amenitiesListDistinct = request.Amenities.Distinct().ToList();

            var amenitiesExist = await _db.Amenities
                .Where(a => request.Amenities.Contains(a.Name))
                .Distinct()
                .ToListAsync();
            if (amenitiesExist.Count != amenitiesListDistinct.Count)
            {
                var missingAmenities = amenitiesListDistinct.Except(amenitiesExist.Select(a => a.Name)).ToList();
                _logger.LogWarning("Some amenities not found: {MissingAmenities}", string.Join(", ", missingAmenities));
                return ResponseFactory.Fail(ResponseType.NotFound, "Some amenities not found");
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            _logger.LogInformation("Transaction started for creating a new conference room.");
            try
            {
                var newRoomId = await CreateAndSaveRoom(request);
                if (amenitiesListDistinct.Count != 0)
                {
                    await AddRoomAmenities(newRoomId, amenitiesExist);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Transaction committed successfully for creating a new conference room with ID {RoomId}.", newRoomId);
                return ResponseFactory.Ok("A new conference room has been successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new conference room. Transaction is being rolled back.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BaseResponseDto> BookConferenceRoomAsync(BookRoomRequestDto request, int roomId)
        {
            decimal totalPrice = 0;
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
            {
                return ResponseFactory.Fail(ResponseType.NotFound, "Room for this ID not found");
            }

            if (request.Amenities != null)
            {
                var priceForAmenities = await CalculatePriceForAmenities(request.Amenities, totalPrice);
                if (priceForAmenities == null)
                {
                    return ResponseFactory.Fail(ResponseType.NotFound, "Some amenities not found");
                }
                totalPrice += priceForAmenities.Value;
            }

            var hasConflictInTime = await _db.RoomBookings
                .AnyAsync(rb => rb.RoomId == roomId
                && rb.StartTime < request.EndTime
                && rb.EndTime > request.StartTime
                && rb.Date == request.Date);
            if (hasConflictInTime)
            {
                _logger.LogWarning("Booking conflict detected for room ID {RoomId} on {Date} from {StartTime} to {EndTime}.",
                    roomId, request.Date, request.StartTime, request.EndTime);
                return ResponseFactory.Fail(ResponseType.Conflict, "These rental hours are already booked. Please choose other hours.");
            }

            totalPrice = CalculatePriceForBook(request, room.BasePrice, totalPrice);

            var booking = new RoomBooking
            {
                TotalPrice = totalPrice,
                RoomId = room.Id,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
            };

            await _db.RoomBookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Conference room with ID {RoomId} has been successfully booked on {Date} from {StartTime} to {EndTime}. Total price: {Price}.",
                roomId, request.Date, request.StartTime, request.EndTime, totalPrice);
            return ResponseFactory.Ok($"Conference room has been successfully booked. Total price: {totalPrice}");
        }

        public async Task<BaseResponseDto> UpdateConferenceRoomAsync(UpdateRoomRequestDto request, int roomId)
        {
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
            {
                _logger.LogWarning("Room not found for ID {RoomId}. Update operation aborted.", roomId);
                return ResponseFactory.Fail(ResponseType.NotFound, "Room not found for this ID");
            }

            if (request.BasePrice != null)
                room.BasePrice = request.BasePrice.Value;
            if (request.Amenities != null)
            {
                var amenitiesListDistinct = request.Amenities.Distinct().ToList();
                var amenitiesExist = await CheckAmenitiesInDb(amenitiesListDistinct);
                if (amenitiesExist == null)
                {
                    _logger.LogWarning("Some amenities not found for room ID {RoomId}. Update operation aborted.", roomId);
                    return ResponseFactory.Fail(ResponseType.NotFound, "Some amenities not found");
                }
                await RemoveOldAmenities(roomId);
                await AddNewAmenities(roomId, amenitiesListDistinct);
            }

            _logger.LogInformation("Room data for ID {RoomId} has been successfully updated.", roomId);
            await _db.SaveChangesAsync();
            return ResponseFactory.Ok("Room data has been successfully updated");
        }

        public async Task<BaseResponseDto> DeleteConferenceRoomAsync(int id)
        {
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                _logger.LogWarning("Room not found for ID {RoomId}. Delete operation aborted.", id);
                return ResponseFactory.Fail(ResponseType.NotFound, "Room for this ID not found");
            }

            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Room with ID {RoomId} has been successfully deleted.", id);
            return ResponseFactory.Ok("Room successfully deleted.");
        }


        //private methods
        private async Task<int> CreateAndSaveRoom(CreateRoomRequestDto request)
        {
            var newRoom = new Room
            {
                Name = request.Name,
                Capacity = request.Capacity,
                BasePrice = request.BasePrice
            };

            await _db.Rooms.AddAsync(newRoom);
            await _db.SaveChangesAsync();
            return newRoom.Id;
        }

        private async Task AddRoomAmenities(int roomId, List<Amenity> amenities)
        {
            foreach (var amenity in amenities)
            {
                var newRoomAmenity = new RoomAmenity
                {
                    Amenity = amenity,
                    AmenityId = amenity.Id,
                    RoomId = roomId
                };
                await _db.RoomAmenities.AddAsync(newRoomAmenity);
            }
        }

        private async Task<decimal?> CalculatePriceForAmenities(List<string> Amenities, decimal totalPrice)
        {
            if (Amenities == null)
            {
                return totalPrice;
            }
            foreach (var amenity in Amenities)
            {
                var amenityExist = await _db.Amenities.FirstOrDefaultAsync(a => a.Name == amenity);
                if (amenityExist != null)
                {
                    totalPrice += amenityExist.Price;
                }
                else
                {
                    return null;
                }
            }
            return totalPrice;
        }

        private async Task RemoveOldAmenities(int roomId)
        {
            var amenities = await _db.RoomAmenities
                .Where(a => a.RoomId == roomId)
                .Distinct()
                .ToListAsync();
            foreach (var roomAmenity in amenities)
            {
                _db.RoomAmenities.Remove(roomAmenity);
            }
        }

        private async Task AddNewAmenities(int roomId, List<string> Amenities)
        {
            foreach (var amenity in Amenities.Distinct())
            {
                var amenityExistInDb = await _db.Amenities.FirstOrDefaultAsync(a => a.Name == amenity);
                if (amenityExistInDb != null)
                {
                    var newRoomAmenity = new RoomAmenity
                    {
                        Amenity = amenityExistInDb,
                        AmenityId = amenityExistInDb.Id,
                        RoomId = roomId
                    };
                    await _db.RoomAmenities.AddAsync(newRoomAmenity);
                }
            }
        }

        private async Task<List<string>?> CheckAmenitiesInDb(List<string> amenitiesDistinctList)
        {
            var amenitiesExist = await _db.Amenities
                .Where(a => amenitiesDistinctList.Contains(a.Name))
                .Distinct()
                .ToListAsync();
            if (amenitiesExist.Count != amenitiesDistinctList.Count)
            {
                return null;
            }
            return amenitiesExist.Select(a => a.Name).ToList();
        }



        //private static methods

        private static decimal CalculatePriceForBook(BookRoomRequestDto request, decimal roomBasePrice, decimal price)
        {
            var currentTime = request.StartTime;
            while (currentTime < request.EndTime)
            {
                decimal priceForHour = CalculatePriceForHour(currentTime, roomBasePrice);
                price += priceForHour;
                currentTime = currentTime.AddHours(1);
            }
            return price;
        }
        private static decimal CalculatePriceForHour(TimeOnly currentTime, decimal basePrice)
        {
            var rules = PricingRules.Modifiers;

            foreach (var rule in rules)
            {
                if (currentTime >= rule.Start && currentTime < rule.End)
                {
                    var modifiedPrice = (basePrice * rule.Modifier) + basePrice;
                    return modifiedPrice;
                }
            }
            return basePrice;
        }
    }
}
