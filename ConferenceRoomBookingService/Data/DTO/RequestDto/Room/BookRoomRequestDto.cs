using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBookingService.Data.DTO.RequestDto.Room
{
    public class BookRoomRequestDto
    {
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        public List<string>? Amenities { get; set; }
    }
}
