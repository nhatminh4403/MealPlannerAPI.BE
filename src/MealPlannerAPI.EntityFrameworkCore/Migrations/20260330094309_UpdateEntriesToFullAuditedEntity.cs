using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntriesToFullAuditedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppMealPlanEntries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppMealPlanEntries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppMealPlanEntries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppMealPlanEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppMealPlanEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppMealPlanEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppMealPlanEntries",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppMealPlanEntries");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppMealPlanEntries");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppMealPlanEntries");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppMealPlanEntries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppMealPlanEntries");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppMealPlanEntries");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppMealPlanEntries");
        }
    }
}
