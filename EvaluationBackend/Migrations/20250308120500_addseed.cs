using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EvaluationBackend.Migrations
{
    /// <inheritdoc />
    public partial class addseed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure no duplicates before inserting
            migrationBuilder.Sql("DELETE FROM \"Roles\" WHERE \"Id\" IN (1, 2);");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreationDate", "Deleted", "Name" },
                values: new object[,]
                {
                    { 1, DateTime.UtcNow, false, "Admin" },
                    { 2, DateTime.UtcNow, false, "DataEntry" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
