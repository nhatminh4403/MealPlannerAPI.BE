using MealPlannerAPI.Nutritions;
using MealPlannerAPI.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.Configurations
{
    public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
    {
        public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
        {
            builder.ToTable("AppRecipeIngredients");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.QuantityGrams)
                .HasColumnType("real");

            builder.Property(x => x.DisplayQuantity)
                .HasMaxLength(64);

            // Optional FK — no cascade delete, nutrition data is shared
            builder.HasOne<IngredientNutrition>()
                .WithMany()
                .HasForeignKey(x => x.IngredientNutritionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
