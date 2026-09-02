using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBookingService.Data.DTO.RequestDto.Amenity
{
    public class UpdateAmenityRequestDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
    }
}
