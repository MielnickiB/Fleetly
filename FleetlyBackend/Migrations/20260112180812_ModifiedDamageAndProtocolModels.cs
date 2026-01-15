using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedDamageAndProtocolModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Damages_Protocols_ProtocolId",
                table: "Damages");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_ProtocolTypes_ProtocolTypeId",
                table: "Protocols");

            migrationBuilder.DropTable(
                name: "ProtocolTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "DamageLocation",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "DamagePart",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "DamageSide",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "DamageType",
                table: "Damages");

            migrationBuilder.RenameColumn(
                name: "ProtocolTypeId",
                table: "Protocols",
                newName: "VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Protocols_ProtocolTypeId",
                table: "Protocols",
                newName: "IX_Protocols_VehicleId");

            migrationBuilder.AlterColumn<string>(
                name: "SignatureUrl",
                table: "Protocols",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<decimal>(
                name: "FuelLevel",
                table: "Protocols",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "LocationLatitude",
                table: "Protocols",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LocationLongitude",
                table: "Protocols",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Protocols",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProtocolId",
                table: "Damages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Damages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Part",
                table: "Damages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Side",
                table: "Damages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Damages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Damages_Protocols_ProtocolId",
                table: "Damages",
                column: "ProtocolId",
                principalTable: "Protocols",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_Vehicles_VehicleId",
                table: "Protocols",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Damages_Protocols_ProtocolId",
                table: "Damages");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_Vehicles_VehicleId",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "FuelLevel",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "LocationLatitude",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "LocationLongitude",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "Part",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "Side",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Damages");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "Protocols",
                newName: "ProtocolTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Protocols_VehicleId",
                table: "Protocols",
                newName: "IX_Protocols_ProtocolTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "SignatureUrl",
                table: "Protocols",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Protocols",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ProtocolId",
                table: "Damages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Damages",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DamageLocation",
                table: "Damages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DamagePart",
                table: "Damages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DamageSide",
                table: "Damages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DamageType",
                table: "Damages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProtocolTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtocolTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Damages_Protocols_ProtocolId",
                table: "Damages",
                column: "ProtocolId",
                principalTable: "Protocols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_ProtocolTypes_ProtocolTypeId",
                table: "Protocols",
                column: "ProtocolTypeId",
                principalTable: "ProtocolTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
