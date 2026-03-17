using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNutrition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRecipeIngredients_AppRecipes_RecipeId",
                table: "AppRecipeIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppRecipeIngredients",
                table: "AppRecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AppRecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AppRecipeIngredients");

            migrationBuilder.RenameTable(
                name: "AppRecipeIngredients",
                newName: "RecipeIngredients");

            migrationBuilder.RenameIndex(
                name: "IX_AppRecipeIngredients_RecipeId",
                table: "RecipeIngredients",
                newName: "IX_RecipeIngredients_RecipeId");

            migrationBuilder.AddColumn<string>(
                name: "DisplayQuantity",
                table: "RecipeIngredients",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientNutritionId",
                table: "RecipeIngredients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "QuantityGrams",
                table: "RecipeIngredients",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeIngredients",
                table: "RecipeIngredients",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IngredientNutritions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CaloriesPer100g = table.Column<float>(type: "real", nullable: false),
                    ProteinPer100g = table.Column<float>(type: "real", nullable: false),
                    CarbsPer100g = table.Column<float>(type: "real", nullable: false),
                    FatPer100g = table.Column<float>(type: "real", nullable: false),
                    FiberPer100g = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientNutritions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientNutritionId",
                table: "RecipeIngredients",
                column: "IngredientNutritionId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientNutritions_NormalizedName",
                table: "IngredientNutritions",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_AppRecipes_RecipeId",
                table: "RecipeIngredients",
                column: "RecipeId",
                principalTable: "AppRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_IngredientNutritions_IngredientNutritionId",
                table: "RecipeIngredients",
                column: "IngredientNutritionId",
                principalTable: "IngredientNutritions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_AppRecipes_RecipeId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_IngredientNutritions_IngredientNutritionId",
                table: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "IngredientNutritions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeIngredients",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_IngredientNutritionId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "DisplayQuantity",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "IngredientNutritionId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "QuantityGrams",
                table: "RecipeIngredients");

            migrationBuilder.RenameTable(
                name: "RecipeIngredients",
                newName: "AppRecipeIngredients");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredients_RecipeId",
                table: "AppRecipeIngredients",
                newName: "IX_AppRecipeIngredients_RecipeId");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "AppRecipeIngredients",
                type: "decimal(10,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AppRecipeIngredients",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppRecipeIngredients",
                table: "AppRecipeIngredients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRecipeIngredients_AppRecipes_RecipeId",
                table: "AppRecipeIngredients",
                column: "RecipeId",
                principalTable: "AppRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
