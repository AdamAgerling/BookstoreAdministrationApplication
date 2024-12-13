using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreAdmin.Migrations
{
    /// <inheritdoc />
    public partial class ChangePublisherFoundedYearColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublisherFoundedYear",
                table: "Publishers");

            migrationBuilder.AddColumn<int>(
                name: "PublisherFoundedYear",
                table: "Publishers",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublisherFoundedYear",
                table: "Publishers");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublisherFoundedYear",
                table: "Publishers",
                nullable: true);
        }
    }
}
