using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace conduflex_api.Migrations
{
    /// <inheritdoc />
    public partial class productapplicationfieldadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnglishAlternatives",
                table: "Products",
                newName: "EnglishApplication");

            migrationBuilder.RenameColumn(
                name: "Alternatives",
                table: "Products",
                newName: "Application");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnglishApplication",
                table: "Products",
                newName: "EnglishAlternatives");

            migrationBuilder.RenameColumn(
                name: "Application",
                table: "Products",
                newName: "Alternatives");
        }
    }
}
