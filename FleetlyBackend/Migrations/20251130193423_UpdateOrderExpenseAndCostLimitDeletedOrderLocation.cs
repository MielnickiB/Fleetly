using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderExpenseAndCostLimitDeletedOrderLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderLocation_EndLocationId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderLocation_StartLocationId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderLocation");

            migrationBuilder.RenameColumn(
                name: "RangeOfKm",
                table: "CostLimits",
                newName: "RangeOfKmMin");

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

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

            migrationBuilder.AlterColumn<string>(
                name: "CostPhotoUrl",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "RangeOfKmMax",
                table: "CostLimits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ServiceLocationId",
                table: "Orders",
                column: "ServiceLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Locations_EndLocationId",
                table: "Orders",
                column: "EndLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Locations_ServiceLocationId",
                table: "Orders",
                column: "ServiceLocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Locations_StartLocationId",
                table: "Orders",
                column: "StartLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Locations_EndLocationId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Locations_ServiceLocationId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Locations_StartLocationId",
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

            migrationBuilder.DropColumn(
                name: "RangeOfKmMax",
                table: "CostLimits");

            migrationBuilder.RenameColumn(
                name: "RangeOfKmMin",
                table: "CostLimits",
                newName: "RangeOfKm");

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "Orders",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CostPhotoUrl",
                table: "Expenses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "OrderLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartmentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BuildingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLocation", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderLocation_EndLocationId",
                table: "Orders",
                column: "EndLocationId",
                principalTable: "OrderLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderLocation_StartLocationId",
                table: "Orders",
                column: "StartLocationId",
                principalTable: "OrderLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
