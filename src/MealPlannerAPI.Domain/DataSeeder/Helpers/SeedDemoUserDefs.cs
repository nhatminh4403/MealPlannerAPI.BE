using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.DataSeeder.Helpers
{
    public static class SeedDemoUserDefs
    {
        public sealed record DemoUserDef(string UserName, string Email, string Name, string Surname, string Specialty, int Followers, int Following);

        public static readonly IReadOnlyList<DemoUserDef> All = new[]
        {
            new DemoUserDef("chef_maria",       "maria@mealplanner.demo", "Maria", "Rodriguez", "Italian Cuisine",  1250, 340),
            new DemoUserDef("healthy_john",     "john@mealplanner.demo",  "John",  "Smith",     "Healthy Eating",   890,  210),
            new DemoUserDef("baker_sarah",      "sarah@mealplanner.demo", "Sarah", "Johnson",   "Baking & Desserts",2100, 450),
            new DemoUserDef("asian_fusion_mike","mike@mealplanner.demo",  "Mike",  "Chen",      "Asian Fusion",     1560, 380),
            new DemoUserDef("vegan_emma",       "emma@mealplanner.demo",  "Emma",  "Williams",  "Vegan Cooking",    3200, 520),
        };
    }
}
