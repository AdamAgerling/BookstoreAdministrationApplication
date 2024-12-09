using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreAdmin.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Books_ImageId",
                table: "Images");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "Books",
                type: "nvarchar(13)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ImageId",
                table: "Books",
                column: "ImageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Images_ImageId",
                table: "Books",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Images_ImageId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_ImageId",
                table: "Books");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Books_ImageId",
                table: "Images",
                column: "ImageId",
                principalTable: "Books",
                principalColumn: "ISBN13",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
