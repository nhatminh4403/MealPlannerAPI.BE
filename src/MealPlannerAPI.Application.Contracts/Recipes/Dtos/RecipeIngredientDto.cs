using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class RecipeIngredientDto : EntityDto<Guid>
    {
        public string Name { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
    }


}
