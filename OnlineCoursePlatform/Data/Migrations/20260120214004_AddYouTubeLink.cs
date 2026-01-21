using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCoursePlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddYouTubeLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeUrl",
                table: "Courses",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeUrl",
                table: "Courses");
        }
    }
}
