using ConferenceRoomBookingService.Entity;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBookingService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomBooking> RoomBookings => Set<RoomBooking>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<RoomAmenity> RoomAmenities => Set<RoomAmenity>();
    }
}
