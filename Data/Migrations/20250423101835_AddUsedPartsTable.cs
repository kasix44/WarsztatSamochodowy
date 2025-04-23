using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsedPartsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPart_Parts_PartId",
                table: "UsedPart");

            migrationBuilder.DropForeignKey(
                name: "FK_UsedPart_ServiceOrders_ServiceOrderId",
                table: "UsedPart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsedPart",
                table: "UsedPart");

            migrationBuilder.RenameTable(
                name: "UsedPart",
                newName: "UsedParts");

            migrationBuilder.RenameIndex(
                name: "IX_UsedPart_ServiceOrderId",
                table: "UsedParts",
                newName: "IX_UsedParts_ServiceOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_UsedPart_PartId",
                table: "UsedParts",
                newName: "IX_UsedParts_PartId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsedParts",
                table: "UsedParts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedParts_Parts_PartId",
                table: "UsedParts",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsedParts_ServiceOrders_ServiceOrderId",
                table: "UsedParts",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedParts_Parts_PartId",
                table: "UsedParts");

            migrationBuilder.DropForeignKey(
                name: "FK_UsedParts_ServiceOrders_ServiceOrderId",
                table: "UsedParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsedParts",
                table: "UsedParts");

            migrationBuilder.RenameTable(
                name: "UsedParts",
                newName: "UsedPart");

            migrationBuilder.RenameIndex(
                name: "IX_UsedParts_ServiceOrderId",
                table: "UsedPart",
                newName: "IX_UsedPart_ServiceOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_UsedParts_PartId",
                table: "UsedPart",
                newName: "IX_UsedPart_PartId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsedPart",
                table: "UsedPart",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPart_Parts_PartId",
                table: "UsedPart",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPart_ServiceOrders_ServiceOrderId",
                table: "UsedPart",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
