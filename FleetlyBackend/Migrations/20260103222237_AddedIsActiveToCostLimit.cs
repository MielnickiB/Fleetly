using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsActiveToCostLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CostLimits_CostLimitId",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CostLimits",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CostLimits_CostLimitId",
                table: "Orders",
                column: "CostLimitId",
                principalTable: "CostLimits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CostLimits_CostLimitId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CostLimits");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CostLimits_CostLimitId",
                table: "Orders",
                column: "CostLimitId",
                principalTable: "CostLimits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
