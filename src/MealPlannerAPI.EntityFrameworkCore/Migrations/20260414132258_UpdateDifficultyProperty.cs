using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDifficultyProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Difficulty",
                table: "AppRecipes",
                type: "int",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "AppRecipes",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 32);
        }
    }
}
