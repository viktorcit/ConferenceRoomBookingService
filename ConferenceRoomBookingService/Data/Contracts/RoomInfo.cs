namespace ConferenceRoomBookingService.Data.Contracts
{
    public class RoomInfo
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Capacity { get; set; }
    }
}
