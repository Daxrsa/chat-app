using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FileTypeaddedtoFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "kite",
                table: "Files",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "kite",
                table: "Files");
        }
    }
}
