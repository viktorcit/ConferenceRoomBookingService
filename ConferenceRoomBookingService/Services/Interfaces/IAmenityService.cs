using ConferenceRoomBookingService.Data.DTO;
using ConferenceRoomBookingService.Data.DTO.RequestDto.Amenity;
using ConferenceRoomBookingService.Data.DTO.ResponseDto.Amenity;

namespace ConferenceRoomBookingService.Services.Interfaces
{
    public interface IAmenityService
    {
        Task<List<AmenityResponseDto>> GetAllAmenitiesAsync();
        Task<BaseResponseDto<AmenityResponseDto>> GetAmenityByIdAsync(int amenityId);
        Task<BaseResponseDto> CreateAmenityAsync(CreateAmenityRequestDto request);
        Task<BaseResponseDto> UpdateAmenityAsync(UpdateAmenityRequestDto request, int amenityId);
        Task<BaseResponseDto> DeleteAmenityAsync(int amenityId);
    }
}
