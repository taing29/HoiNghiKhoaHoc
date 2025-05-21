using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoiNghiKhoaHoc.Migrations
{
    /// <inheritdoc />
    public partial class AddImageConference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerImage",
                table: "Conferences",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerImage",
                table: "Conferences");
        }
    }
}
