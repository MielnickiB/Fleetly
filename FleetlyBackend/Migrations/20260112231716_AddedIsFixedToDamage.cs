using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsFixedToDamage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FixedAt",
                table: "Damages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FixedByProtocolId",
                table: "Damages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFixed",
                table: "Damages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedAt",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "FixedByProtocolId",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "IsFixed",
                table: "Damages");
        }
    }
}
