using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedEndContactToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndContactName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndContactPhone",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndContactName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EndContactPhone",
                table: "Orders");
        }
    }
}
