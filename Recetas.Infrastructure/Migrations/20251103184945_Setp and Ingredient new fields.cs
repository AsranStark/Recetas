using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recetas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetpandIngredientnewfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Steps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Ingredients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Ingredients");
        }
    }
}
