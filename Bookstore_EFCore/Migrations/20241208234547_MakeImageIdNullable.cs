using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookstoreAdmin.Migrations
{
    /// <inheritdoc />
    public partial class MakeImageIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                type: "nvarchar(13)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ImageId",
                table: "Books",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Images_ImageId",
                table: "Books",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId");
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
                type: "nvarchar(13)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldNullable: true);

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
    }
}
