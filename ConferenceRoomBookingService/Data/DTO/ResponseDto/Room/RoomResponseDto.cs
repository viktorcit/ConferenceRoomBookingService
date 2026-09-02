namespace ConferenceRoomBookingService.Data.DTO.ResponseDto.Room
{
    public class RoomResponseDto
    {
        public required int RoomId { get; set; }
        public required string Name { get; set; }
        public required int Capacity { get; set; }
        public required List<string> Amenities { get; set; } = [];
    }
}
