using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreAdmin.Migrations
{
    /// <inheritdoc />
    public partial class CheckPendingChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Images_ImageId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId1",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_ImageId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_PublisherId1",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublisherId1",
                table: "Books");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Books_ImageId",
                table: "Images",
                column: "ImageId",
                principalTable: "Books",
                principalColumn: "ISBN13",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Books_ImageId",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Books",
                type: "nvarchar(13)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublisherId1",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_ImageId",
                table: "Books",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherId1",
                table: "Books",
                column: "PublisherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Images_ImageId",
                table: "Books",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId1",
                table: "Books",
                column: "PublisherId1",
                principalTable: "Publishers",
                principalColumn: "PublisherId");
        }
    }
}
