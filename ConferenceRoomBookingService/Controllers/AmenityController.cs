using ConferenceRoomBookingService.Data.DTO.RequestDto.Amenity;
using ConferenceRoomBookingService.Data.DTO.ResponseDto.Amenity;
using ConferenceRoomBookingService.Enums;
using ConferenceRoomBookingService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityService _amenityService;
        private readonly ILogger<AmenityController> _logger;
        public AmenityController(IAmenityService amenityService, ILogger<AmenityController> logger) 
        {
            _amenityService = amenityService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<List<AmenityResponseDto>>> GetAllAmenitiesAsync()
        {
            var result = await _amenityService.GetAllAmenitiesAsync();
            return result;
        }

        [HttpGet("{amenityId}")]
        public async Task<ActionResult<AmenityResponseDto>> GetAmenityByIdAsync(int amenityId)
        {
            _logger.LogInformation("Request for amenity with ID: {AmenityId}", amenityId);
            var result = await _amenityService.GetAmenityByIdAsync(amenityId);
            return result.ResponseType switch
            {
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                _ => Ok(result.Data),
            };
        }

        [HttpPost]
        public async Task<ActionResult> CreateAmenityAsync(CreateAmenityRequestDto request)
        {
            if (request.Price <= 0)
            {
                return BadRequest("Price must be greater than zero");
            }

            _logger.LogInformation("Request to create amenity with Name: {AmenityName} and Price: {AmenityPrice}", request.Name, request.Price);
            var result = await _amenityService.CreateAmenityAsync(request);
            return result.ResponseType switch
            {
                ResponseType.Conflict => Conflict(result.ResponseMessage),
                _ => Ok(result.ResponseMessage)
            };
        }

        [HttpPatch("{amenityId}")]
        public async Task<ActionResult> UpdateAmenityAsync(UpdateAmenityRequestDto request, int amenityId)
        {
            if ((request.Name == null && request.Price == null) || request.Price <= 0)
            {
                return BadRequest("Invalid data for update");
            }

            _logger.LogInformation("Request to update amenity with ID: {AmenityId}", amenityId);
            var result = await _amenityService.UpdateAmenityAsync(request, amenityId);
            return result.ResponseType switch
            {
                ResponseType.Conflict => Conflict(result.ResponseMessage),
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                _ => Ok(result.ResponseMessage)
            };
        }

        [HttpDelete("{amenityId}")]
        public async Task<ActionResult> DeleteAmenityAsync(int amenityId)
        {
            _logger.LogInformation("Request to delete amenity with ID: {AmenityId}", amenityId);
            var result = await _amenityService.DeleteAmenityAsync(amenityId);
            return result.ResponseType switch
            {
                ResponseType.NotFound => NotFound(result.ResponseMessage),
                _ => Ok(result.ResponseMessage)
            };
        }
    }
}
