using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluationBackend.Migrations
{
    /// <inheritdoc />
    public partial class seedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 14, 20, 37, 725, DateTimeKind.Utc).AddTicks(137));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 14, 20, 37, 725, DateTimeKind.Utc).AddTicks(142));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 14, 16, 36, 968, DateTimeKind.Utc).AddTicks(1725));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 14, 16, 36, 968, DateTimeKind.Utc).AddTicks(1729));
        }
    }
}
