using MealPlannerAPI.Nutritions;
using MealPlannerAPI.Recipes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace MealPlannerAPI.DataSeeder
{
    public class NutritionDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IIngredientNutritionRepository _ingredientNutritionRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IGuidGenerator _guidGenerator;
        public int Order => 1;
        public NutritionDataSeedContributor(IIngredientNutritionRepository ingredientNutritionRepository,
                                            IRecipeRepository recipeRepository,
                                            IGuidGenerator guidGenerator)
        {
            _ingredientNutritionRepository = ingredientNutritionRepository;
            _recipeRepository = recipeRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var lookup = await SeedIngredientNutritionsAsync();
            await SeedSampleRecipesAsync(lookup);
        }
        private async Task<Dictionary<string, Guid>> SeedIngredientNutritionsAsync()
        {
            // (name, cal, protein, carbs, fat, fiber) — all per 100g
            var ingredients = new (string Name, float Cal, float Pro, float Carb, float Fat, float Fib)[]
            {
            ("Chicken Breast",       165, 31.0f, 0.0f,  3.6f, 0.0f),
            ("Salmon",               208, 20.0f, 0.0f,  13.0f, 0.0f),
            ("Egg",                  155, 13.0f, 1.1f,  11.0f, 0.0f),
            ("Whole Milk",           61,  3.2f,  4.8f,  3.3f, 0.0f),
            ("Cheddar Cheese",       403, 25.0f, 1.3f,  33.0f, 0.0f),
            ("Pasta (Dry)",          371, 13.0f, 74.0f, 1.5f, 3.2f),
            ("White Rice (Dry)",     365, 7.1f,  80.0f, 0.7f, 1.3f),
            ("Brown Rice (Dry)",     362, 7.5f,  76.0f, 2.7f, 3.5f),
            ("Bread (White)",        265, 9.0f,  49.0f, 3.2f, 2.7f),
            ("Olive Oil",            884, 0.0f,  0.0f,  100.0f, 0.0f),
            ("Butter",               717, 0.9f,  0.1f,  81.0f, 0.0f),
            ("Tomato",               18,  0.9f,  3.9f,  0.2f, 1.2f),
            ("Onion",                40,  1.1f,  9.3f,  0.1f, 1.7f),
            ("Garlic",               149, 6.4f,  33.0f, 0.5f, 2.1f),
            ("Broccoli",             34,  2.8f,  7.0f,  0.4f, 2.6f),
            ("Spinach",              23,  2.9f,  3.6f,  0.4f, 2.2f),
            ("Potato",               77,  2.0f,  17.0f, 0.1f, 2.2f),
            ("Sweet Potato",         86,  1.6f,  20.0f, 0.1f, 3.0f),
            ("Black Beans (Cooked)", 132, 8.9f,  24.0f, 0.5f, 8.7f),
            ("Oats (Dry)",           389, 17.0f, 66.0f, 7.0f, 10.6f),
            ("Ground Beef",          332, 14.0f, 0.0f,  30.0f, 0.0f),
            ("Bell Pepper",          31,  1.0f,  6.0f,  0.3f,  2.1f),
            ("Greek Yogurt",         59,  10.0f, 3.6f,  0.4f,  0.0f),
            ("Banana",               89,  1.1f,  22.8f, 0.3f,  2.6f),
            ("Peanut Butter",        588, 25.0f, 20.0f, 50.0f, 6.0f),
            ("Honey",                304, 0.3f,  82.4f, 0.0f,  0.2f),
            ("Apple",                52,  0.3f,  14.0f, 0.2f,  2.4f),
            ("Almonds",              579, 21.0f, 22.0f, 49.0f, 12.0f),
            ("Carrot",               41,  0.9f,  10.0f, 0.2f,  2.8f),
            ("Lentils (Dry)",        353, 25.0f, 60.0f, 1.0f,  30.0f),
            ("Pork Chop",            231, 24.0f, 0.0f,  14.0f, 0.0f),
            ("Tofu",                 144, 15.0f, 2.8f,  8.0f,  2.0f),
            ("Soy Sauce",            53,  8.0f,  4.9f,  0.1f,  0.8f),
            ("Mushrooms",            22,  3.1f,  3.3f,  0.3f,  1.0f),
            ("Cucumber",             15,  0.6f,  3.6f,  0.1f,  0.5f),
            ("Avocado",              160, 2.0f,  8.5f,  14.7f, 6.7f),
            ("Lettuce",              15,  1.4f,  2.9f,  0.2f,  1.2f),
            };

            var lookup = new Dictionary<string, Guid>();

            foreach (var (name, cal, pro, carb, fat, fib) in ingredients)
            {
                var existing = await _ingredientNutritionRepository.FindAsync(x => x.NormalizedName == name.ToLowerInvariant());

                if (existing != null)
                {
                    lookup[name] = existing.Id;
                    continue;
                }

                var entity = new IngredientNutrition(
                    _guidGenerator.Create(), name, cal, pro, carb, fat, fib);

                await _ingredientNutritionRepository.InsertAsync(entity, autoSave: true);
                lookup[name] = entity.Id;
            }

            return lookup;
        }

        // ── Sample Recipes ────────────────────────────────────────────────────────

        private async Task SeedSampleRecipesAsync(Dictionary<string, Guid> lookup)
        {
            var recipes = new[]
            {
            BuildGrilledChicken(lookup),
            BuildSalmonRice(lookup),
            BuildPastaPomodoro(lookup),
            BuildSpinachOmelette(lookup),
            BuildSweetPotatoBowl(lookup),
            BuildChickenFriedRice(lookup),
            BuildBananaOatmeal(lookup),
            BuildBeefAndPotatoBake(lookup),
            BuildYogurtParfait(lookup),
            BuildPeanutButterToast(lookup),
            BuildBeefStirFry(lookup),
            BuildChickenSalad(lookup),
            BuildAvocadoToast(lookup),
            BuildMushroomOmelette(lookup),
            BuildTofuStirFry(lookup),
            BuildPorkChopSweetPotato(lookup),
            BuildLentilSoup(lookup),
            BuildGreekSalad(lookup),
            BuildAppleAlmondSnack(lookup),
            BuildChickenRiceBowl(lookup),
            BuildBeefBurgerSalad(lookup),
            BuildPeanutButterAppleToast(lookup),
            BuildVeggieFriedRice(lookup),
            BuildMushroomPasta(lookup),
            BuildSpicyTofuScramble(lookup),
            BuildGarlicButterPorkChops(lookup),
            BuildBreakfastBurritoBowl(lookup),
            BuildChickenMushroomSaute(lookup),
            BuildCarrotLentilMash(lookup),
            BuildYogurtAlmonds(lookup),
        };

            foreach (var recipe in recipes)
            {
                var exists = await _recipeRepository.AnyAsync(r => r.Name == recipe.Name);
                if (!exists)
                    await _recipeRepository.InsertAsync(recipe, autoSave: true);
            }
        }

        private Recipe BuildGrilledChicken(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Grilled Chicken with Broccoli",
                cuisine: "American",
                description: "A clean high-protein meal with grilled chicken breast and steamed broccoli.",
                servings: 2,
                prepMinutes: 10,
                cookMinutes: 20,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Chicken Breast", 300, "300g", l),
                Ing(_guidGenerator, "Broccoli",       200, "200g", l),
                Ing(_guidGenerator, "Olive Oil",       10, "1 tbsp", l),
                Ing(_guidGenerator, "Garlic",           5, "2 cloves", l),
                }
            );

        private Recipe BuildSalmonRice(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Salmon with Brown Rice",
                cuisine: "Japanese",
                description: "Omega-3 rich salmon fillet served over nutty brown rice.",
                servings: 2,
                prepMinutes: 10,
                cookMinutes: 25,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Salmon",          250, "250g", l),
                Ing(_guidGenerator, "Brown Rice (Dry)",  80, "80g dry", l),
                Ing(_guidGenerator, "Olive Oil",          8, "1 tbsp", l),
                Ing(_guidGenerator, "Spinach",           50, "handful", l),
                }
            );

        private Recipe BuildPastaPomodoro(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Pasta Pomodoro",
                cuisine: "Italian",
                description: "Classic Italian pasta with a simple fresh tomato sauce.",
                servings: 4,
                prepMinutes: 10,
                cookMinutes: 20,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Pasta (Dry)",  320, "320g", l),
                Ing(_guidGenerator, "Tomato",       400, "4 large", l),
                Ing(_guidGenerator, "Garlic",        10, "4 cloves", l),
                Ing(_guidGenerator, "Olive Oil",     20, "2 tbsp", l),
                Ing(_guidGenerator, "Onion",         80, "1 medium", l),
                }
            );

        private Recipe BuildSpinachOmelette(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Spinach and Cheese Omelette",
                cuisine: "French",
                description: "Fluffy omelette loaded with wilted spinach and melted cheddar.",
                servings: 1,
                prepMinutes: 5,
                cookMinutes: 8,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Egg",            150, "3 eggs", l),
                Ing(_guidGenerator, "Spinach",         60, "handful", l),
                Ing(_guidGenerator, "Cheddar Cheese",  30, "30g", l),
                Ing(_guidGenerator, "Butter",          10, "1 tsp", l),
                }
            );

        private Recipe BuildSweetPotatoBowl(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Sweet Potato & Black Bean Bowl",
                cuisine: "Mexican",
                description: "Hearty plant-based bowl with roasted sweet potato and spiced black beans.",
                servings: 2,
                prepMinutes: 10,
                cookMinutes: 30,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Sweet Potato",         300, "2 medium", l),
                Ing(_guidGenerator, "Black Beans (Cooked)", 200, "1 can drained", l),
                Ing(_guidGenerator, "Olive Oil",             15, "1 tbsp", l),
                Ing(_guidGenerator, "Onion",                 80, "1 medium", l),
                Ing(_guidGenerator, "Garlic",                 5, "2 cloves", l),
                }
            );

        private Recipe BuildChickenFriedRice(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Chicken Fried Rice",
                cuisine: "Chinese",
                description: "Classic homemade fried rice with chicken breast, eggs, and onions.",
                servings: 2,
                prepMinutes: 10,
                cookMinutes: 15,
                ingredients: new[]
                {
                Ing(_guidGenerator, "White Rice (Dry)",  100, "100g dry", l),
                Ing(_guidGenerator, "Chicken Breast",    150, "150g", l),
                Ing(_guidGenerator, "Egg",               100, "2 eggs", l),
                Ing(_guidGenerator, "Onion",             50,  "half onion", l),
                Ing(_guidGenerator, "Olive Oil",         15,  "1 tbsp", l),
                }
            );

        private Recipe BuildBananaOatmeal(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Banana Peanut Butter Oatmeal",
                cuisine: "Breakfast",
                description: "A hearty breakfast bowl of oats topped with fresh banana and peanut butter.",
                servings: 1,
                prepMinutes: 5,
                cookMinutes: 10,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Oats (Dry)",        50,  "50g", l),
                Ing(_guidGenerator, "Whole Milk",        150, "150ml", l),
                Ing(_guidGenerator, "Banana",            118, "1 medium", l),
                Ing(_guidGenerator, "Peanut Butter",     32,  "2 tbsp", l),
                Ing(_guidGenerator, "Honey",             10,  "1 tsp", l),
                }
            );

        private Recipe BuildBeefAndPotatoBake(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Ground Beef and Potato Bake",
                cuisine: "Continental",
                description: "A comforting casserole with layers of potato, seasoned ground beef, and cheesy goodness.",
                servings: 4,
                prepMinutes: 15,
                cookMinutes: 45,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Ground Beef",       400, "400g", l),
                Ing(_guidGenerator, "Potato",            500, "4 medium", l),
                Ing(_guidGenerator, "Cheddar Cheese",    100, "100g grated", l),
                Ing(_guidGenerator, "Onion",             100, "1 large", l),
                Ing(_guidGenerator, "Garlic",            10,  "4 cloves", l),
                Ing(_guidGenerator, "Olive Oil",         15,  "1 tbsp", l),
                }
            );

        private Recipe BuildYogurtParfait(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Greek Yogurt Parfait",
                cuisine: "Breakfast",
                description: "A quick and healthy snack or breakfast with Greek yogurt and honey.",
                servings: 1,
                prepMinutes: 5,
                cookMinutes: 0,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Greek Yogurt",      200, "200g", l),
                Ing(_guidGenerator, "Honey",             15,  "1 tbsp", l),
                Ing(_guidGenerator, "Banana",            118, "1 medium", l),
                Ing(_guidGenerator, "Oats (Dry)",        20,  "2 tbsp", l),
                }
            );

        private Recipe BuildPeanutButterToast(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Peanut Butter Banana Toast",
                cuisine: "Breakfast",
                description: "Simple and delicious toast topped with peanut butter and sliced bananas.",
                servings: 1,
                prepMinutes: 5,
                cookMinutes: 2,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Bread (White)",     70,  "2 slices", l),
                Ing(_guidGenerator, "Peanut Butter",     30,  "2 tbsp", l),
                Ing(_guidGenerator, "Banana",            118, "1 medium", l),
                }
            );

        private Recipe BuildBeefStirFry(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Beef and Bell Pepper Stir Fry",
                cuisine: "Asian",
                description: "A quick stir fry with ground beef, vibrant bell peppers, and savory garlic.",
                servings: 2,
                prepMinutes: 10,
                cookMinutes: 15,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Ground Beef",       250, "250g", l),
                Ing(_guidGenerator, "Bell Pepper",       150, "1 large", l),
                Ing(_guidGenerator, "Onion",             80,  "1 medium", l),
                Ing(_guidGenerator, "Garlic",            10,  "4 cloves", l),
                Ing(_guidGenerator, "Olive Oil",         15,  "1 tbsp", l),
                Ing(_guidGenerator, "White Rice (Dry)",  120, "120g dry", l),
                }
            );

        private Recipe BuildChickenSalad(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(
                _guidGenerator.Create(),
                name: "Chicken and Spinach Salad",
                cuisine: "American",
                description: "A light salad featuring grilled chicken, fresh spinach, and sweet cherry tomatoes.",
                servings: 2,
                prepMinutes: 15,
                cookMinutes: 15,
                ingredients: new[]
                {
                Ing(_guidGenerator, "Chicken Breast",    200, "200g", l),
                Ing(_guidGenerator, "Spinach",           100, "2 large handfuls", l),
                Ing(_guidGenerator, "Tomato",            150, "1 cup cherry tomatoes", l),
                Ing(_guidGenerator, "Olive Oil",         30,  "2 tbsp", l),
                }
            );

        private Recipe BuildAvocadoToast(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Avocado Toast", "Australian", "Trendy and nutritious avocado squash on toast.", 1, 5, 0, new[] {
                Ing(_guidGenerator, "Bread (White)", 70, "2 slices", l),
                Ing(_guidGenerator, "Avocado", 100, "1/2 avocado", l),
                Ing(_guidGenerator, "Egg", 50, "1 egg", l),
                Ing(_guidGenerator, "Olive Oil", 5, "1 tsp", l) });

        private Recipe BuildMushroomOmelette(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Mushroom Omelette", "Breakfast", "Savory mushroom and cheese omelette.", 1, 5, 10, new[] {
                Ing(_guidGenerator, "Egg", 150, "3 eggs", l),
                Ing(_guidGenerator, "Mushrooms", 100, "1 cup sliced", l),
                Ing(_guidGenerator, "Cheddar Cheese", 30, "1/4 cup", l),
                Ing(_guidGenerator, "Butter", 10, "1 tsp", l),
                Ing(_guidGenerator, "Onion", 20, "1/4 onion", l) });

        private Recipe BuildTofuStirFry(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Tofu Stir Fry", "Asian", "Quick and healthy vegetarian stir fry.", 2, 10, 15, new[] {
                Ing(_guidGenerator, "Tofu", 200, "200g", l),
                Ing(_guidGenerator, "Soy Sauce", 30, "2 tbsp", l),
                Ing(_guidGenerator, "Bell Pepper", 100, "1 large", l),
                Ing(_guidGenerator, "Broccoli", 150, "1 cup", l),
                Ing(_guidGenerator, "Olive Oil", 15, "1 tbsp", l),
                Ing(_guidGenerator, "White Rice (Dry)", 120, "120g", l) });

        private Recipe BuildPorkChopSweetPotato(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Pork Chop with Sweet Potato", "American", "Pan-seared pork chop with baked sweet potato.", 2, 10, 30, new[] {
                Ing(_guidGenerator, "Pork Chop", 300, "2 chops", l),
                Ing(_guidGenerator, "Sweet Potato", 300, "2 medium", l),
                Ing(_guidGenerator, "Olive Oil", 15, "1 tbsp", l),
                Ing(_guidGenerator, "Garlic", 5, "2 cloves", l) });

        private Recipe BuildLentilSoup(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Hearty Lentil Soup", "Mediterranean", "A warm, comforting lentil and veggie soup.", 4, 15, 45, new[] {
                Ing(_guidGenerator, "Lentils (Dry)", 200, "1 cup", l),
                Ing(_guidGenerator, "Carrot", 150, "2 medium", l),
                Ing(_guidGenerator, "Onion", 100, "1 large", l),
                Ing(_guidGenerator, "Tomato", 200, "2 large", l),
                Ing(_guidGenerator, "Olive Oil", 15, "1 tbsp", l),
                Ing(_guidGenerator, "Garlic", 10, "4 cloves", l) });

        private Recipe BuildGreekSalad(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Greek Salad", "Greek", "Crisp cucumber and tomato salad with cheese.", 2, 10, 0, new[] {
                Ing(_guidGenerator, "Cucumber", 200, "1 large", l),
                Ing(_guidGenerator, "Tomato", 200, "2 large", l),
                Ing(_guidGenerator, "Onion", 50, "1/2 medium", l),
                Ing(_guidGenerator, "Olive Oil", 30, "2 tbsp", l),
                Ing(_guidGenerator, "Cheddar Cheese", 50, "50g", l) });

        private Recipe BuildAppleAlmondSnack(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Apple & Almond Snack", "Snack", "Simple raw snack.", 1, 2, 0, new[] {
                Ing(_guidGenerator, "Apple", 150, "1 medium", l),
                Ing(_guidGenerator, "Almonds", 30, "1 small handful", l) });

        private Recipe BuildChickenRiceBowl(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Chicken Rice Bowl", "Asian", "Lean chicken and avocado over rice.", 2, 10, 20, new[] {
                Ing(_guidGenerator, "Chicken Breast", 250, "250g", l),
                Ing(_guidGenerator, "White Rice (Dry)", 120, "120g", l),
                Ing(_guidGenerator, "Avocado", 100, "1/2 avocado", l),
                Ing(_guidGenerator, "Soy Sauce", 15, "1 tbsp", l) });

        private Recipe BuildBeefBurgerSalad(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Beef Burger Patty Salad", "American", "Keto-friendly burger patties on lettuce.", 2, 10, 15, new[] {
                Ing(_guidGenerator, "Ground Beef", 300, "300g", l),
                Ing(_guidGenerator, "Lettuce", 100, "4 leaves", l),
                Ing(_guidGenerator, "Tomato", 100, "1 medium", l),
                Ing(_guidGenerator, "Onion", 50, "1/2 medium", l) });

        private Recipe BuildPeanutButterAppleToast(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Peanut Butter Apple Toast", "Breakfast", "Sweet and crunchy toast.", 1, 5, 2, new[] {
                Ing(_guidGenerator, "Bread (White)", 35, "1 slice", l),
                Ing(_guidGenerator, "Peanut Butter", 15, "1 tbsp", l),
                Ing(_guidGenerator, "Apple", 75, "1/2 apple", l) });

        private Recipe BuildVeggieFriedRice(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Vegetable Fried Rice", "Asian", "Quick fried rice loaded with veggies.", 2, 10, 15, new[] {
                Ing(_guidGenerator, "White Rice (Dry)", 120, "120g", l),
                Ing(_guidGenerator, "Carrot", 100, "1 medium", l),
                Ing(_guidGenerator, "Broccoli", 100, "1 cup", l),
                Ing(_guidGenerator, "Egg", 100, "2 eggs", l),
                Ing(_guidGenerator, "Soy Sauce", 30, "2 tbsp", l) });

        private Recipe BuildMushroomPasta(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Mushroom Pasta", "Italian", "Garlicky mushroom pasta with spinach.", 2, 10, 20, new[] {
                Ing(_guidGenerator, "Pasta (Dry)", 160, "160g", l),
                Ing(_guidGenerator, "Mushrooms", 200, "2 cups", l),
                Ing(_guidGenerator, "Garlic", 10, "4 cloves", l),
                Ing(_guidGenerator, "Olive Oil", 30, "2 tbsp", l),
                Ing(_guidGenerator, "Spinach", 100, "2 handfuls", l) });

        private Recipe BuildSpicyTofuScramble(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Spicy Tofu Scramble", "American", "Vegan scramble with veggies.", 2, 10, 15, new[] {
                Ing(_guidGenerator, "Tofu", 300, "1 block", l),
                Ing(_guidGenerator, "Onion", 50, "1/2 medium", l),
                Ing(_guidGenerator, "Tomato", 100, "1 medium", l),
                Ing(_guidGenerator, "Spinach", 50, "1 handful", l),
                Ing(_guidGenerator, "Olive Oil", 15, "1 tbsp", l) });

        private Recipe BuildGarlicButterPorkChops(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Garlic Butter Pork Chops", "American", "Rich and tender pork chops with potatoes.", 2, 10, 25, new[] {
                Ing(_guidGenerator, "Pork Chop", 300, "2 chops", l),
                Ing(_guidGenerator, "Garlic", 10, "4 cloves", l),
                Ing(_guidGenerator, "Butter", 20, "2 tbsp", l),
                Ing(_guidGenerator, "Potato", 300, "2 medium", l) });

        private Recipe BuildBreakfastBurritoBowl(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Breakfast Burrito Bowl", "Mexican", "Deconstructed burrito for breakfast.", 2, 10, 15, new[] {
                Ing(_guidGenerator, "Black Beans (Cooked)", 150, "1/2 can", l),
                Ing(_guidGenerator, "Egg", 100, "2 eggs", l),
                Ing(_guidGenerator, "Avocado", 100, "1/2 avocado", l),
                Ing(_guidGenerator, "Tomato", 100, "1 medium", l),
                Ing(_guidGenerator, "Brown Rice (Dry)", 100, "100g", l) });

        private Recipe BuildChickenMushroomSaute(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Chicken & Mushroom Sauté", "French", "Simple pan-fried chicken and mushrooms.", 2, 10, 20, new[] {
                Ing(_guidGenerator, "Chicken Breast", 250, "250g", l),
                Ing(_guidGenerator, "Mushrooms", 200, "2 cups", l),
                Ing(_guidGenerator, "Onion", 100, "1 medium", l),
                Ing(_guidGenerator, "Olive Oil", 15, "1 tbsp", l) });

        private Recipe BuildCarrotLentilMash(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Carrot & Lentil Mash", "British", "Soft comforting mash.", 2, 10, 30, new[] {
                Ing(_guidGenerator, "Carrot", 200, "2 large", l),
                Ing(_guidGenerator, "Lentils (Dry)", 100, "1/2 cup", l),
                Ing(_guidGenerator, "Butter", 20, "2 tbsp", l) });

        private Recipe BuildYogurtAlmonds(Dictionary<string, Guid> l) =>
            Recipe.CreateSeed(_guidGenerator.Create(), "Yogurt with Almonds", "Snack", "Protein-rich yogurt snack.", 1, 2, 0, new[] {
                Ing(_guidGenerator, "Greek Yogurt", 200, "200g", l),
                Ing(_guidGenerator, "Almonds", 30, "1 handful", l),
                Ing(_guidGenerator, "Honey", 15, "1 tbsp", l) });

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static (string Name, float Grams, string Display, Guid? NutritionId) Ing(
            IGuidGenerator gen,
            string name,
            float grams,
            string display,
            Dictionary<string, Guid> lookup)
            => (name, grams, display, lookup.TryGetValue(name, out var id) ? id : null);
    }
}
