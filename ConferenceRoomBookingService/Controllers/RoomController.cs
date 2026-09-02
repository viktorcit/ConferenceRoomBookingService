using ConferenceRoomBookingService.Data.DTO.RequestDto.Room;
using ConferenceRoomBookingService.Data.DTO.ResponseDto.Room;
using ConferenceRoomBookingService.Enums;
using ConferenceRoomBookingService.Helpers;
using ConferenceRoomBookingService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }


        [HttpPost("available")]
        public async Task<ActionResult<List<RoomResponseDto>>> GetAvailableRooms([FromBody] GetAvailableRoomsRequestDto request)
        {
            if (request.StartTime >= request.EndTime || request.Date < DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest("You have selected an invalid time or date, please select valid values");
            }

            var result = await _roomService.GetAvailableRooms(request);
            return result.ResponseType switch
            {
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                ResponseType.BadRequest => BadRequest(result.ResponseMessage),
                _ => Ok(result.Data),
            };
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateConferenceRoomAsync([FromBody] CreateRoomRequestDto request)
        {
            if (request.BasePrice <= 0)
            {
                return BadRequest("BasePrice must be greater than zero");
            }

            _logger.LogInformation("Creating a new conference room with name: {RoomName}, capacity: {Capacity}, base price: {BasePrice}",
                request.Name, request.Capacity, request.BasePrice);
            var result = await _roomService.CreateConferenceRoomAsync(request);
            return result.ResponseType switch
            {
                ResponseType.BadRequest => BadRequest(result.ResponseMessage),
                ResponseType.Conflict => Conflict(result.ResponseMessage),
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                _ => Ok(result.ResponseMessage),
            };
        }

        [HttpPost("book/{roomId}")]
        public async Task<ActionResult> BookConferenceRoomAsync([FromBody] BookRoomRequestDto request, [FromRoute] int roomId)
        {
            if (request.StartTime >= request.EndTime || request.Date < DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest("You have selected an invalid time or date, please select valid values");
            }

            _logger.LogInformation("Booking conference room with ID: {RoomId} for date: {Date}, start time: {StartTime}, end time: {EndTime}",
                roomId, request.Date, request.StartTime, request.EndTime);
            var result = await _roomService.BookConferenceRoomAsync(request, roomId);
            return result.ResponseType switch
            {
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                ResponseType.BadRequest => BadRequest(result.ResponseMessage),
                ResponseType.Conflict => Conflict(result.ResponseMessage),
                _ => Ok(result.ResponseMessage),
            };
        }

        [HttpPatch("{roomId}")]
        public async Task<ActionResult> UpdateConferenceRoomAsync([FromBody] UpdateRoomRequestDto request, [FromRoute] int roomId)
        {
            if ((request.BasePrice == null && request.Amenities == null) || request.BasePrice <= 0)
            {
                return BadRequest("Invalid data for update");
            }

            _logger.LogInformation("Updating conference room with ID: {RoomId}. New base price: {BasePrice}, new amenities: {Amenities}",
                roomId, request.BasePrice, request.Amenities);
            var result = await _roomService.UpdateConferenceRoomAsync(request, roomId);
            return result.ResponseType switch
            {
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                ResponseType.BadRequest => BadRequest(result.ResponseMessage),
                _ => Ok(result.ResponseMessage),
            };
        }

        [HttpDelete("{roomId}")]
        public async Task<ActionResult> DeleteConferenceRoomAsync([FromRoute] int roomId)
        {
            _logger.LogInformation("Deleting conference room with ID: {RoomId}", roomId);
            var result = await _roomService.DeleteConferenceRoomAsync(roomId);
            return result.ResponseType switch
            {
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                _ => Ok(result.ResponseMessage),
            };
        }
    }
}
