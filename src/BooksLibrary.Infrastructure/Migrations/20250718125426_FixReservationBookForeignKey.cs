using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReservationBookForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_UserId",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookId",
                table: "Reservations",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_BookId",
                table: "Reservations");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
