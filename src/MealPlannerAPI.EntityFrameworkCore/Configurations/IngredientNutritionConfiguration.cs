using MealPlannerAPI.Nutritions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.Configurations
{
    public class IngredientNutritionConfiguration : IEntityTypeConfiguration<IngredientNutrition>
    {
        public void Configure(EntityTypeBuilder<IngredientNutrition> builder)
        {
            builder.ToTable("IngredientNutritions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(x => x.NormalizedName)
                .IsUnique();

            builder.Property(x => x.CaloriesPer100g).HasColumnType("real");
            builder.Property(x => x.ProteinPer100g).HasColumnType("real");
            builder.Property(x => x.CarbsPer100g).HasColumnType("real");
            builder.Property(x => x.FatPer100g).HasColumnType("real");
            builder.Property(x => x.FiberPer100g).HasColumnType("real");
        }
    }
}
