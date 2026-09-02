using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBookingService.Data.DTO.RequestDto.Room
{
    public class CreateRoomRequestDto
    {
        [Required, MinLength(2), MaxLength(25)]
        public required string Name { get; set; }
        [Required, Range(1, 300)]
        public int Capacity { get; set; }
        public List<string> Amenities { get; set; } = [];
        [Required]
        public required decimal BasePrice { get; set; }
    }
}
