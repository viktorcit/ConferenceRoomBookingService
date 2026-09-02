namespace ConferenceRoomBookingService.Entity
{
    public class RoomAmenity
    {
        public int Id { get; set; }
        public required int RoomId { get; set; }
        public required int AmenityId { get; set; }
        public required Amenity Amenity { get; set; }
    }
}
