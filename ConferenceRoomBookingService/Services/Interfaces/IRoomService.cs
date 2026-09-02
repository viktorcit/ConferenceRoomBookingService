using ConferenceRoomBookingService.Data.DTO;
using ConferenceRoomBookingService.Data.DTO.RequestDto.Room;
using ConferenceRoomBookingService.Data.DTO.ResponseDto.Room;

namespace ConferenceRoomBookingService.Services.Interfaces
{
    public interface IRoomService
    {
        Task<BaseResponseDto<List<RoomResponseDto>>> GetAvailableRooms(GetAvailableRoomsRequestDto request);
        Task<BaseResponseDto> CreateConferenceRoomAsync(CreateRoomRequestDto request);
        Task<BaseResponseDto> BookConferenceRoomAsync(BookRoomRequestDto request, int roomId);
        Task<BaseResponseDto> UpdateConferenceRoomAsync(UpdateRoomRequestDto request, int roomId);
        Task<BaseResponseDto> DeleteConferenceRoomAsync(int id);

    }
}
