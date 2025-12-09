using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedFuelTypeToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FuelType",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "Vehicles");
        }
    }
}
