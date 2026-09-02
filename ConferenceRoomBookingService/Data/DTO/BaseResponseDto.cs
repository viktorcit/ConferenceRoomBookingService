using ConferenceRoomBookingService.Enums;

namespace ConferenceRoomBookingService.Data.DTO
{
    public class BaseResponseDto
    {
        public required bool IsSuccess { get; set; }
        public string? ResponseMessage { get; set; }
        public required ResponseType ResponseType { get; set; }
    }

    public class BaseResponseDto<T> : BaseResponseDto
    {
        public T? Data { get; set; }
    }
}
