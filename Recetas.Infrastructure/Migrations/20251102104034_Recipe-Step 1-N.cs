using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recetas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecipeStep1N : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeStep");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Steps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeId",
                table: "Steps",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Steps_RecipeId",
                table: "Steps",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Recipes_RecipeId",
                table: "Steps",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Recipes_RecipeId",
                table: "Steps");

            migrationBuilder.DropIndex(
                name: "IX_Steps_RecipeId",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Steps");

            migrationBuilder.CreateTable(
                name: "RecipeStep",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeStep", x => new { x.RecipeId, x.StepsId });
                    table.ForeignKey(
                        name: "FK_RecipeStep_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeStep_Steps_StepsId",
                        column: x => x.StepsId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeStep_StepsId",
                table: "RecipeStep",
                column: "StepsId");
        }
    }
}
