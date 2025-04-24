using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeServiceOrderIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobActivities_ServiceOrders_ServiceOrderId",
                table: "JobActivities");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceOrderId",
                table: "JobActivities",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_JobActivities_ServiceOrders_ServiceOrderId",
                table: "JobActivities",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobActivities_ServiceOrders_ServiceOrderId",
                table: "JobActivities");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceOrderId",
                table: "JobActivities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobActivities_ServiceOrders_ServiceOrderId",
                table: "JobActivities",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
