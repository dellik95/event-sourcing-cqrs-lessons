using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Post.Query.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePorperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Comments");
        }
    }
}
