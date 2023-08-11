using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace conduflex_api.Migrations
{
    /// <inheritdoc />
    public partial class homeenglishfieldsadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "EnglishDescription",
                table: "Home",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnglishTitle",
                table: "Home",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnglishDescription",
                table: "Home");

            migrationBuilder.DropColumn(
                name: "EnglishTitle",
                table: "Home");
        }
    }
}
