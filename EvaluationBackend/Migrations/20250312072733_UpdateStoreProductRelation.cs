using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IXIR.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreProductRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Products_ProductId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_ProductId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Stores");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "Stores",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "StoreProducts",
                columns: table => new
                {
                    ProductsId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoresId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProducts", x => new { x.ProductsId, x.StoresId });
                    table.ForeignKey(
                        name: "FK_StoreProducts_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProducts_Stores_StoresId",
                        column: x => x.StoresId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2025, 3, 12, 7, 27, 32, 884, DateTimeKind.Utc).AddTicks(8889));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2025, 3, 12, 7, 27, 32, 884, DateTimeKind.Utc).AddTicks(8893));

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_StoresId",
                table: "StoreProducts",
                column: "StoresId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreProducts");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Stores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2025, 3, 11, 10, 6, 27, 689, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2025, 3, 11, 10, 6, 27, 689, DateTimeKind.Utc).AddTicks(9539));

            migrationBuilder.CreateIndex(
                name: "IX_Stores_ProductId",
                table: "Stores",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Products_ProductId",
                table: "Stores",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
