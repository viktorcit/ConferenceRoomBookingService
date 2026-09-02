using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBookingService.Data.DTO.RequestDto.Amenity
{
    public class CreateAmenityRequestDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required decimal Price { get; set; }
    }
}
