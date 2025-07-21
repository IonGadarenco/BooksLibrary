using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLikeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "UserLike",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLike", x => new { x.UserId, x.BookId });
                    table.ForeignKey(
                        name: "FK_UserLike_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLike_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLike_BookId",
                table: "UserLike",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLike");

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
