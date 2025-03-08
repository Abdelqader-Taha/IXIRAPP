using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluationBackend.Migrations
{
    /// <inheritdoc />
    public partial class inniitt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreCount",
                table: "Users");
        }
    }
}
