namespace ConferenceRoomBookingService.Entity
{
    public class Amenity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }
}
