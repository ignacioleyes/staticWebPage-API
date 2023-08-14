using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace conduflex_api.Migrations
{
    /// <inheritdoc />
    public partial class english_alternatives_for_product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnglishAlternatives",
                table: "Products",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnglishAlternatives",
                table: "Products");
        }
    }
}
