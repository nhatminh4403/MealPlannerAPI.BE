using System.Collections.Generic;

namespace MealPlannerAPI.DataSeeder.Helpers
{
    public static class SeedDemoUserDefs
    {
        public sealed record DemoUserDef(string UserName, string Email, string Name, string Surname, string Specialty);

        public static readonly IReadOnlyList<DemoUserDef> All = 
        [
            new DemoUserDef("chef_maria",       "maria@mealplanner.demo", "Maria", "Rodriguez", "Italian Cuisine"),
            new DemoUserDef("healthy_john",     "john@mealplanner.demo",  "John",  "Smith",     "Healthy Eating"),
            new DemoUserDef("baker_sarah", "sarah@mealplanner.demo", "Sarah", "Johnson", "Baking & Desserts"),
            new DemoUserDef("asian_fusion_mike", "mike@mealplanner.demo", "Mike", "Chen", "Asian Fusion"),
            new DemoUserDef("vegan_emma", "emma@mealplanner.demo", "Emma", "Williams", "Vegan Cooking"),
        ];
    }
}
