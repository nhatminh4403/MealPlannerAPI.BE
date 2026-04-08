using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NutritionId",
                table: "RecipeIngredients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Leaved",
                table: "AbpUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "PropertyTypeFullName",
                table: "AbpEntityPropertyChanges",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "EntityTypeFullName",
                table: "AbpEntityChanges",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_NutritionId",
                table: "RecipeIngredients",
                column: "NutritionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_IngredientNutritions_NutritionId",
                table: "RecipeIngredients",
                column: "NutritionId",
                principalTable: "IngredientNutritions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_IngredientNutritions_NutritionId",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_NutritionId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "NutritionId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Leaved",
                table: "AbpUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyTypeFullName",
                table: "AbpEntityPropertyChanges",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "EntityTypeFullName",
                table: "AbpEntityChanges",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);
        }
    }
}
