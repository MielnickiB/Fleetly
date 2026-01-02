using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetlyBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedCostLimitToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostLimitId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CostLimitId",
                table: "Orders",
                column: "CostLimitId");

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

            migrationBuilder.DropIndex(
                name: "IX_Orders_CostLimitId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CostLimitId",
                table: "Orders");
        }
    }
}
