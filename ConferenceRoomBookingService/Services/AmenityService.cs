using ConferenceRoomBookingService.Data;
using ConferenceRoomBookingService.Data.DTO;
using ConferenceRoomBookingService.Data.DTO.RequestDto.Amenity;
using ConferenceRoomBookingService.Data.DTO.ResponseDto.Amenity;
using ConferenceRoomBookingService.Entity;
using ConferenceRoomBookingService.Enums;
using ConferenceRoomBookingService.Helpers;
using ConferenceRoomBookingService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBookingService.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AmenityService> _logger;

        public AmenityService(AppDbContext db, ILogger<AmenityService> logger)
        {
            _db = db;
            _logger = logger;
        }


        public async Task<List<AmenityResponseDto>> GetAllAmenitiesAsync()
        {
            var amenities = await _db.Amenities
                .AsNoTracking()
                .Select(a => new AmenityResponseDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Price = a.Price,
                }).ToListAsync();
            return amenities;
        }

        public async Task<BaseResponseDto<AmenityResponseDto>> GetAmenityByIdAsync(int amenityId)
        {
            var amenity = await _db.Amenities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == amenityId);
            if (amenity == null)
            {
                _logger.LogWarning("Amenity not found for id: {AmenityId}", amenityId);
                return ResponseFactory.Fail<AmenityResponseDto>(
                    ResponseType.NotFound,
                    "Amenity not found for this id");
            }

            var response = new AmenityResponseDto
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Price = amenity.Price,
            };

            return ResponseFactory.Ok(response);
        }

        public async Task<BaseResponseDto> CreateAmenityAsync(CreateAmenityRequestDto request)
        {
            var amenityAlreadyExist = await _db.Amenities
                .AsNoTracking()
                .AnyAsync(a => a.Name == request.Name);
            if (amenityAlreadyExist)
            {
                _logger.LogWarning("Amenity with name {AmenityName} already exists", request.Name);
                return ResponseFactory.Fail(
                    ResponseType.Conflict,
                    "Amenity with same name already exist");
            }

            var newAmenity = new Amenity
            {
                Name = request.Name,
                Price = request.Price,
            };

            await _db.Amenities.AddAsync(newAmenity);
            await _db.SaveChangesAsync();

            return ResponseFactory.Ok("Amenity successfully created");
        }

        public async Task<BaseResponseDto> UpdateAmenityAsync(UpdateAmenityRequestDto request ,int amenityId)
        {
            var amenity = await _db.Amenities.FirstOrDefaultAsync(a => a.Id == amenityId);
            if (amenity == null)
            {
                _logger.LogWarning("Amenity not found for id: {AmenityId}", amenityId);
                return ResponseFactory.Fail(
                    ResponseType.NotFound,
                    "Not found amenity for this id");
            }
            var amenityAlreadyExist = await _db.Amenities
                .AsNoTracking()
                .AnyAsync(a => a.Name == request.Name && a.Id != amenityId);
            if (amenityAlreadyExist)
            {
                return ResponseFactory.Fail(
                    ResponseType.Conflict,
                    "Amenity with same name already exist");
            }

                if (request.Name != null)
                amenity.Name = request.Name;
            if(request.Price != null)
                amenity.Price = request.Price.Value;

            _db.Amenities.Update(amenity);
            await _db.SaveChangesAsync();

            return ResponseFactory.Ok("Amenity successfully has been updated");
        }

        public async Task<BaseResponseDto> DeleteAmenityAsync(int amenityId)
        {
            var amenity = await _db.Amenities.FirstOrDefaultAsync(a => a.Id == amenityId);
            if (amenity == null)
            {
                _logger.LogWarning("Amenity not found for id: {AmenityId}", amenityId);
                return ResponseFactory.Fail(
                    ResponseType.NotFound,
                    "Amenity not found for this id");
            }

            _db.Amenities.Remove(amenity);
            await _db.SaveChangesAsync();

            return ResponseFactory.Ok("Amenity successfully deleted.");
        }
    }
}
