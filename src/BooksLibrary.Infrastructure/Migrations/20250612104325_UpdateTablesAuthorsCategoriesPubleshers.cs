using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesAuthorsCategoriesPubleshers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Publishers",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Authors",
                newName: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Publishers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Authors",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Authors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
