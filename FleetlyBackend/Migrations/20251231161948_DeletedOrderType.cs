using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class DeletedOrderType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Locations_ServiceLocationId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ServiceLocationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ActualArrivedServiceTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ActualLeftServiceTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceLocationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualArrivedServiceTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualLeftServiceTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceLocationId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ServiceLocationId",
                table: "Orders",
                column: "ServiceLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Locations_ServiceLocationId",
                table: "Orders",
                column: "ServiceLocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
