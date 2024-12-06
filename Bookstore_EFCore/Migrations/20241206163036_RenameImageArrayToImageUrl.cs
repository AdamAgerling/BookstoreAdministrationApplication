using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreAdmin.Migrations
{
    /// <inheritdoc />
    public partial class RenameImageArrayToImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageArray",
                table: "Images",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Images",
                newName: "ImageArray");
        }
    }
}
