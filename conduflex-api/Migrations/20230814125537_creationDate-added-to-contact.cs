using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace conduflex_api.Migrations
{
    /// <inheritdoc />
    public partial class creationDateaddedtocontact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Contacts");
        }
    }
}
