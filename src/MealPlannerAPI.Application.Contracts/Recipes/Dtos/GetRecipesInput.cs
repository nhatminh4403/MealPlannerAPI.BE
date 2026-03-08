using MealPlannerAPI.Enums;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class GetRecipesInput : PagedAndSortedResultRequestDto
    {
        public string? SearchTerm { get; set; }
        public string? Cuisine { get; set; }
        public DifficultyLevel? Difficulty { get; set; }
        public int? MaxTotalTimeMinutes { get; set; }
        public bool? Vegetarian { get; set; }
    }
}
