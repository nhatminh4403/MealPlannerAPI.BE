namespace MealPlannerAPI.Users.Dtos
{
    public class UserStatsDto
    {
        public int RecipesCreated { get; set; }
        public int RecipesLiked { get; set; }
        public int MealsPlanned { get; set; }
        public int ShoppingListsGenerated { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public string? Specialty { get; set; }
    }
}
