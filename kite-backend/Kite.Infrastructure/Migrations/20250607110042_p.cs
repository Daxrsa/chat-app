using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "kite",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "DiskFilePath",
                schema: "kite",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiskFilePath",
                schema: "kite",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "kite",
                table: "AspNetUsers",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }
    }
}
