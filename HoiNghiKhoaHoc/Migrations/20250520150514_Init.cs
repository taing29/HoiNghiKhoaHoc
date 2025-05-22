using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoiNghiKhoaHoc.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Conferences");

            migrationBuilder.DropColumn(
                name: "Organizer",
                table: "Conferences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Conferences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Organizer",
                table: "Conferences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
