using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBookingService.Data.DTO.RequestDto.Room
{
    public class GetAvailableRoomsRequestDto
    {
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        [Required, Range(1, 300)]
        public int Capacity { get; set; }
    }
}
