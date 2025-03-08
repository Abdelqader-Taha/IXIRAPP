using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluationBackend.Migrations
{
    /// <inheritdoc />
    public partial class addadmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 13, 5, 3, 680, DateTimeKind.Utc).AddTicks(9532));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 13, 5, 3, 680, DateTimeKind.Utc).AddTicks(9537));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 12, 5, 0, 625, DateTimeKind.Utc).AddTicks(5315));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2025, 3, 8, 12, 5, 0, 625, DateTimeKind.Utc).AddTicks(5320));
        }
    }
}
