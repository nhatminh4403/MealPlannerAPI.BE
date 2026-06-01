using System;
using System.Collections.Generic;

namespace MealPlannerAPI.DataSeeder.Helpers;

/// <summary>
/// Stable, publicly hosted food photos (Unsplash) for demo recipe cards.
/// </summary>
public static class RecipeSeedImages
{
    private static readonly Dictionary<string, string> Urls = new(StringComparer.OrdinalIgnoreCase)
    {
        // MealPlannerAPIDataSeedContributor
        ["Classic Spaghetti Carbonara"] = "https://images.unsplash.com/photo-1613529865876-2b79d908f5d5?w=800&q=80",
        ["Margherita Pizza"] = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=800&q=80",
        ["Tiramisu"] = "https://images.unsplash.com/photo-1571877227205-a0d98ea607e9?w=800&q=80",
        ["Quinoa Buddha Bowl"] = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800&q=80",
        ["Grilled Salmon with Asparagus"] = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=800&q=80",
        ["Green Smoothie Bowl"] = "https://images.unsplash.com/photo-1590301157890-4810ed352733?w=800&q=80",
        ["Chocolate Chip Cookies"] = "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?w=800&q=80",
        ["New York Cheesecake"] = "https://images.unsplash.com/photo-1524354219884-b37b1a0b8d1e?w=800&q=80",
        ["Sourdough Bread"] = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=800&q=80",
        ["Pad Thai"] = "https://images.unsplash.com/photo-1559314809-0d155014e29d?w=800&q=80",
        ["Korean Bibimbap"] = "https://images.unsplash.com/photo-1553163147-622ab57be1c7?w=800&q=80",
        ["Japanese Ramen"] = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=800&q=80",
        ["Vegan Lentil Curry"] = "https://images.unsplash.com/photo-1455619452474-d2be8b1e70dd?w=800&q=80",
        ["Chickpea Tacos"] = "https://images.unsplash.com/photo-1565299585323-38174c4a6b8a?w=800&q=80",
        ["Vegan Chocolate Cake"] = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=800&q=80",
        ["Admin Herb Roast Chicken"] = "https://images.unsplash.com/photo-1598103447897-ff6c7454c8c8?w=800&q=80",
        ["Admin Garden Vegetable Soup"] = "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=800&q=80",

        // NutritionRecipeDataSeedContributor / RecipeBuilders
        ["Grilled Chicken with Broccoli"] = "https://images.unsplash.com/photo-1604908176997-f4310c54fa09?w=800&q=80",
        ["Salmon with Brown Rice"] = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=800&q=80",
        ["Pasta Pomodoro"] = "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=800&q=80",
        ["Spinach and Cheese Omelette"] = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=800&q=80",
        ["Sweet Potato & Black Bean Bowl"] = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800&q=80",
        ["Chicken Fried Rice"] = "https://images.unsplash.com/photo-1603133872871-684f608fb84b?w=800&q=80",
        ["Banana Peanut Butter Oatmeal"] = "https://images.unsplash.com/photo-1517673400267-025144b7742e?w=800&q=80",
        ["Ground Beef and Potato Bake"] = "https://images.unsplash.com/photo-1600891964092-4316c288032e?w=800&q=80",
        ["Greek Yogurt Parfait"] = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=800&q=80",
        ["Peanut Butter Banana Toast"] = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=800&q=80",
        ["Beef and Bell Pepper Stir Fry"] = "https://images.unsplash.com/photo-1603133872871-684f608fb84b?w=800&q=80",
        ["Chicken and Spinach Salad"] = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800&q=80",
        ["Avocado Toast"] = "https://images.unsplash.com/photo-1541519227354-08fa5d50c44d?w=800&q=80",
        ["Mushroom Omelette"] = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=800&q=80",
        ["Tofu Stir Fry"] = "https://images.unsplash.com/photo-1512058564366-18510be2db19?w=800&q=80",
        ["Pork Chop with Sweet Potato"] = "https://images.unsplash.com/photo-1432136657670-1c99adbc4a0d?w=800&q=80",
        ["Hearty Lentil Soup"] = "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=800&q=80",
        ["Greek Salad"] = "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=800&q=80",
        ["Apple & Almond Snack"] = "https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=800&q=80",
        ["Chicken Rice Bowl"] = "https://images.unsplash.com/photo-1603133872871-684f608fb84b?w=800&q=80",
        ["Beef Burger Patty Salad"] = "https://images.unsplash.com/photo-1550547660-d9450f859349?w=800&q=80",
        ["Peanut Butter Apple Toast"] = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=800&q=80",
        ["Vegetable Fried Rice"] = "https://images.unsplash.com/photo-1603133872871-684f608fb84b?w=800&q=80",
        ["Mushroom Pasta"] = "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=800&q=80",
        ["Spicy Tofu Scramble"] = "https://images.unsplash.com/photo-1512058564366-18510be2db19?w=800&q=80",
        ["Garlic Butter Pork Chops"] = "https://images.unsplash.com/photo-1432136657670-1c99adbc4a0d?w=800&q=80",
        ["Breakfast Burrito Bowl"] = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800&q=80",
        ["Chicken & Mushroom Sauté"] = "https://images.unsplash.com/photo-1604908176997-f4310c54fa09?w=800&q=80",
        ["Carrot & Lentil Mash"] = "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=800&q=80",
        ["Yogurt with Almonds"] = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=800&q=80",
    };

    public static string? TryGet(string recipeName) =>
        Urls.TryGetValue(recipeName, out var url) ? url : null;
}
