namespace ConferenceRoomBookingService.Entity
{
    public class Room
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Capacity { get; set; }
        public required decimal BasePrice { get; set; }

    }
}
