using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class TrendingRecipeDto : EntityDto<Guid>
    {
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public double TrendingScore { get; set; }
        public string TrendingSince { get; set; } = null!;
    }
}
