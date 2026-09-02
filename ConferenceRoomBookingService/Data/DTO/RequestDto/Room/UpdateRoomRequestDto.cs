using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBookingService.Data.DTO.RequestDto.Room
{
    public class UpdateRoomRequestDto
    {
        public List<string>? Amenities { get; set; }
        public decimal? BasePrice { get; set; }
    }
}
