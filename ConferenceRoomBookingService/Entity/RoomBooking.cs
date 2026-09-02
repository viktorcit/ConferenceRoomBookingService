namespace ConferenceRoomBookingService.Entity
{
    public class RoomBooking
    {
        public int Id { get; set; }
        public required int RoomId { get; set; }
        public required DateOnly Date { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public required decimal TotalPrice { get; set; }
    }
}
