using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceRoomBookingService.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPriceInRoomBookingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "RoomBookings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "RoomBookings");
        }
    }
}
