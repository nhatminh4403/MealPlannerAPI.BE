using MealPlannerAPI.Enums;
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
using Volo.Abp.Identity;
namespace MealPlannerAPI.DataSeeder
{
    public class NutritionDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IIngredientNutritionRepository _ingredientNutritionRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IIdentityUserRepository _identityUserRepository;

        public int Order => 1;
        public NutritionDataSeedContributor(IIngredientNutritionRepository ingredientNutritionRepository,
                                            IRecipeRepository recipeRepository,
                                            IGuidGenerator guidGenerator,
                                            IIdentityUserRepository identityUserRepository)
        {
            _ingredientNutritionRepository = ingredientNutritionRepository;
            _recipeRepository = recipeRepository;
            _guidGenerator = guidGenerator;
            _identityUserRepository = identityUserRepository;
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
            var adminId = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");

            var recipes = new[]
            {
            BuildGrilledChicken(lookup,adminId.Id),
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

        private Recipe BuildGrilledChicken(Dictionary<string, Guid> l, Guid id)
        {

            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Grilled Chicken with Broccoli",
                cuisine: "American",
                description: "A clean high-protein meal with grilled chicken breast and steamed broccoli.",
                servings: 2, prepMinutes: 10, cookMinutes: 20,
                difficulty: DifficultyLevel.Medium,
                authorId: id,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Chicken Breast", 300, "300g", l),
                    Ing(_guidGenerator, "Broccoli",       200, "200g", l),
                    Ing(_guidGenerator, "Olive Oil",       10, "1 tbsp", l),
                    Ing(_guidGenerator, "Garlic",           5, "2 cloves", l),
                });
            recipe.SetInstructions(new[]
            {
                "Season chicken breasts with salt, pepper, and minced garlic.",
                "Heat olive oil in a grill pan over medium-high heat.",
                "Grill chicken for 6-7 minutes per side until cooked through and juices run clear.",
                "Meanwhile, steam broccoli florets for 5-6 minutes until tender-crisp.",
                "Rest chicken for 3 minutes, then slice and serve alongside broccoli.",
            });
            return recipe;
        }

        private Recipe BuildSalmonRice(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Salmon with Brown Rice",
                cuisine: "Japanese",
                description: "Omega-3 rich salmon fillet served over nutty brown rice.",
                servings: 2, prepMinutes: 10, cookMinutes: 25,
                difficulty: DifficultyLevel.Medium,
                authorId: null,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Salmon",           250, "250g", l),
                    Ing(_guidGenerator, "Brown Rice (Dry)",  80, "80g dry", l),
                    Ing(_guidGenerator, "Olive Oil",          8, "1 tbsp", l),
                    Ing(_guidGenerator, "Spinach",           50, "handful", l),
                });
            recipe.SetInstructions(new[]
            {
                "Cook brown rice according to package directions (about 20-25 minutes).",
                "Pat salmon dry and season with salt and pepper.",
                "Heat olive oil in a non-stick pan over medium-high heat.",
                "Cook salmon skin-side up for 4 minutes, then flip and cook for another 3-4 minutes.",
                "Wilt spinach in the same pan for 1-2 minutes.",
                "Serve salmon over rice with spinach on the side.",
            });
            return recipe;
        }

        private Recipe BuildPastaPomodoro(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Pasta Pomodoro",
                cuisine: "Italian",
                description: "Classic Italian pasta with a simple fresh tomato sauce.",
                servings: 4, prepMinutes: 10, cookMinutes: 20,
                difficulty: DifficultyLevel.Easy, authorId: null,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Pasta (Dry)",  320, "320g", l),
                    Ing(_guidGenerator, "Tomato",       400, "4 large", l),
                    Ing(_guidGenerator, "Garlic",        10, "4 cloves", l),
                    Ing(_guidGenerator, "Olive Oil",     20, "2 tbsp", l),
                    Ing(_guidGenerator, "Onion",         80, "1 medium", l),
                });
            recipe.SetInstructions(new[]
            {
                "Bring a large pot of salted water to a boil and cook pasta until al dente.",
                "Dice tomatoes and finely chop onion and garlic.",
                "Heat olive oil in a saucepan over medium heat. Sauté onion for 3 minutes until soft.",
                "Add garlic and cook for 1 minute until fragrant.",
                "Add diced tomatoes and simmer for 10 minutes, stirring occasionally.",
                "Season sauce with salt and pepper. Drain pasta and toss with the sauce. Serve immediately.",
            });
            return recipe;
        }

        private Recipe BuildSpinachOmelette(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Spinach and Cheese Omelette",
                cuisine: "French",
                description: "Fluffy omelette loaded with wilted spinach and melted cheddar.",
                servings: 1, prepMinutes: 5, cookMinutes: 8,
                difficulty: DifficultyLevel.Easy, authorId: null,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Egg",            150, "3 eggs", l),
                    Ing(_guidGenerator, "Spinach",         60, "handful", l),
                    Ing(_guidGenerator, "Cheddar Cheese",  30, "30g", l),
                    Ing(_guidGenerator, "Butter",          10, "1 tsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Crack eggs into a bowl, season with salt and pepper, and whisk until smooth.",
                "Melt butter in a non-stick pan over medium heat.",
                "Add spinach and sauté for 1-2 minutes until wilted. Set aside.",
                "Pour eggs into the pan and cook, gently pulling edges toward the center.",
                "When eggs are just set, add spinach and shredded cheddar to one half.",
                "Fold omelette in half and slide onto a plate. Serve immediately.",
            });
            return recipe;
        }

        private Recipe BuildSweetPotatoBowl(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Sweet Potato & Black Bean Bowl",
                cuisine: "Mexican",
                description: "Hearty plant-based bowl with roasted sweet potato and spiced black beans.",
                servings: 2, prepMinutes: 10, cookMinutes: 30,
                difficulty: DifficultyLevel.Medium, authorId: null,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Sweet Potato",         300, "2 medium", l),
                    Ing(_guidGenerator, "Black Beans (Cooked)", 200, "1 can drained", l),
                    Ing(_guidGenerator, "Olive Oil",             15, "1 tbsp", l),
                    Ing(_guidGenerator, "Onion",                 80, "1 medium", l),
                    Ing(_guidGenerator, "Garlic",                 5, "2 cloves", l),
                });
            recipe.SetInstructions(new[]
            {
                "Preheat oven to 200°C (400°F). Cube sweet potatoes and toss with olive oil, salt, and pepper.",
                "Roast sweet potatoes on a baking sheet for 25-30 minutes, flipping halfway, until golden.",
                "Meanwhile, sauté diced onion in a pan over medium heat for 3 minutes.",
                "Add minced garlic and cook for 1 minute.",
                "Add drained black beans, season with cumin and salt, and cook for 5 minutes until heated through.",
                "Assemble bowls with roasted sweet potato and spiced black beans. Serve warm.",
            });
            return recipe;
        }

        private Recipe BuildChickenFriedRice(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Chicken Fried Rice",
                cuisine: "Chinese",
                description: "Classic homemade fried rice with chicken breast, eggs, and onions.",
                servings: 2, prepMinutes: 10, cookMinutes: 15,
                difficulty: DifficultyLevel.Medium, authorId: null,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "White Rice (Dry)",  100, "100g dry", l),
                    Ing(_guidGenerator, "Chicken Breast",    150, "150g", l),
                    Ing(_guidGenerator, "Egg",               100, "2 eggs", l),
                    Ing(_guidGenerator, "Onion",              50, "half onion", l),
                    Ing(_guidGenerator, "Olive Oil",          15, "1 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Cook rice and let it cool (preferably overnight in the fridge for best texture).",
                "Dice chicken and season with salt and pepper.",
                "Heat oil in a wok or large pan over high heat. Stir-fry chicken for 5-6 minutes until cooked.",
                "Push chicken to the side, add a little oil, and scramble the eggs in the pan.",
                "Add diced onion and cook for 2 minutes, then add the cold rice and stir-fry everything together.",
                "Season with soy sauce (optional) and stir-fry for another 2-3 minutes. Serve hot.",
            });
            return recipe;
        }

        private Recipe BuildBananaOatmeal(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Banana Peanut Butter Oatmeal",
                cuisine: "Breakfast", authorId: null,
                description: "A hearty breakfast bowl of oats topped with fresh banana and peanut butter.",
                servings: 1, prepMinutes: 5, cookMinutes: 10,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Oats (Dry)",        50,  "50g", l),
                    Ing(_guidGenerator, "Whole Milk",        150, "150ml", l),
                    Ing(_guidGenerator, "Banana",            118, "1 medium", l),
                    Ing(_guidGenerator, "Peanut Butter",      32, "2 tbsp", l),
                    Ing(_guidGenerator, "Honey",              10, "1 tsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Combine oats and milk in a small saucepan over medium heat.",
                "Cook, stirring frequently, for 5-7 minutes until oats are creamy and milk is absorbed.",
                "Pour oatmeal into a bowl and top with sliced banana.",
                "Add a dollop of peanut butter and drizzle with honey.",
                "Serve immediately.",
            });
            return recipe;
        }

        private Recipe BuildBeefAndPotatoBake(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Ground Beef and Potato Bake",
                cuisine: "Continental", authorId: null,
                description: "A comforting casserole with layers of potato, seasoned ground beef, and cheesy goodness.",
                servings: 4, prepMinutes: 15, cookMinutes: 45,
                difficulty: DifficultyLevel.Hard,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Ground Beef",       400, "400g", l),
                    Ing(_guidGenerator, "Potato",            500, "4 medium", l),
                    Ing(_guidGenerator, "Cheddar Cheese",    100, "100g grated", l),
                    Ing(_guidGenerator, "Onion",             100, "1 large", l),
                    Ing(_guidGenerator, "Garlic",             10, "4 cloves", l),
                    Ing(_guidGenerator, "Olive Oil",          15, "1 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Preheat oven to 190°C (375°F). Thinly slice potatoes and set aside.",
                "Heat olive oil in a pan and brown ground beef with onion and garlic over medium-high heat.",
                "Season beef mixture with salt, pepper, and your choice of herbs. Drain excess fat.",
                "Layer half the potato slices in a greased baking dish. Season with salt and pepper.",
                "Spread the beef mixture over the potatoes, then top with remaining potato slices.",
                "Cover with grated cheddar and bake for 40-45 minutes until potatoes are tender and cheese is golden.",
            });
            return recipe;
        }

        private Recipe BuildYogurtParfait(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Greek Yogurt Parfait", authorId: null,
                cuisine: "Breakfast",
                description: "A quick and healthy snack or breakfast with Greek yogurt and honey.",
                servings: 1, prepMinutes: 5, cookMinutes: 0,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Greek Yogurt", 200, "200g", l),
                    Ing(_guidGenerator, "Honey",         15, "1 tbsp", l),
                    Ing(_guidGenerator, "Banana",        118, "1 medium", l),
                    Ing(_guidGenerator, "Oats (Dry)",    20, "2 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Spoon Greek yogurt into a glass or bowl.",
                "Slice banana and layer on top of the yogurt.",
                "Sprinkle dry oats over the banana for crunch.",
                "Drizzle with honey and serve immediately.",
            });
            return recipe;
        }

        private Recipe BuildPeanutButterToast(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Peanut Butter Banana Toast",
                cuisine: "Breakfast", authorId: null,
                description: "Simple and delicious toast topped with peanut butter and sliced bananas.",
                servings: 1, prepMinutes: 5, cookMinutes: 2,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Bread (White)",  70, "2 slices", l),
                    Ing(_guidGenerator, "Peanut Butter",  30, "2 tbsp", l),
                    Ing(_guidGenerator, "Banana",        118, "1 medium", l),
                });
            recipe.SetInstructions(new[]
            {
                "Toast bread slices until golden.",
                "Spread peanut butter generously over each slice.",
                "Slice banana and arrange on top of the peanut butter.",
                "Serve immediately, optionally drizzled with a little honey.",
            });
            return recipe;
        }

        private Recipe BuildBeefStirFry(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Beef and Bell Pepper Stir Fry",
                cuisine: "Asian", authorId: null,
                description: "A quick stir fry with ground beef, vibrant bell peppers, and savory garlic.",
                servings: 2, prepMinutes: 10, cookMinutes: 15,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Ground Beef",       250, "250g", l),
                    Ing(_guidGenerator, "Bell Pepper",       150, "1 large", l),
                    Ing(_guidGenerator, "Onion",              80, "1 medium", l),
                    Ing(_guidGenerator, "Garlic",             10, "4 cloves", l),
                    Ing(_guidGenerator, "Olive Oil",          15, "1 tbsp", l),
                    Ing(_guidGenerator, "White Rice (Dry)",  120, "120g dry", l),
                });
            recipe.SetInstructions(new[]
            {
                "Cook rice according to package instructions.",
                "Slice bell pepper and dice onion and garlic.",
                "Heat oil in a wok over high heat. Add ground beef and cook until browned, breaking it up.",
                "Add onion and bell pepper, stir-fry for 3-4 minutes until slightly softened.",
                "Add garlic and cook for 1 more minute. Season with soy sauce, salt, and pepper.",
                "Serve stir-fry over steamed rice.",
            });
            return recipe;
        }

        private Recipe BuildChickenSalad(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Chicken and Spinach Salad",
                cuisine: "American", authorId: null,
                description: "A light salad featuring grilled chicken, fresh spinach, and sweet cherry tomatoes.",
                servings: 2, prepMinutes: 15, cookMinutes: 15,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Chicken Breast", 200, "200g", l),
                    Ing(_guidGenerator, "Spinach",        100, "2 large handfuls", l),
                    Ing(_guidGenerator, "Tomato",         150, "1 cup cherry tomatoes", l),
                    Ing(_guidGenerator, "Olive Oil",       30, "2 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Season chicken with salt, pepper, and a drizzle of olive oil.",
                "Grill or pan-fry chicken over medium-high heat for 6-7 minutes per side until cooked through.",
                "Rest chicken for 3 minutes, then slice thinly.",
                "Arrange spinach and halved tomatoes in a bowl.",
                "Top with sliced chicken. Drizzle remaining olive oil over the salad and season to taste.",
            });
            return recipe;
        }

        private Recipe BuildAvocadoToast(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Avocado Toast",
                cuisine: "Australian", authorId: null,
                description: "Trendy and nutritious avocado squash on toast.",
                servings: 1, prepMinutes: 5, cookMinutes: 0,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Bread (White)", 70,  "2 slices", l),
                    Ing(_guidGenerator, "Avocado",       100, "1/2 avocado", l),
                    Ing(_guidGenerator, "Egg",            50, "1 egg", l),
                    Ing(_guidGenerator, "Olive Oil",       5, "1 tsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Toast bread slices until golden and crispy.",
                "Halve the avocado, remove the pit, and scoop flesh into a bowl.",
                "Mash avocado with a fork and season with salt, pepper, and a squeeze of lemon.",
                "Heat olive oil in a small pan and fry the egg to your liking (sunny-side up or poached).",
                "Spread mashed avocado on toast, top with the fried egg, and serve immediately.",
            });
            return recipe;
        }

        private Recipe BuildMushroomOmelette(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Mushroom Omelette",
                cuisine: "Breakfast", authorId: null,
                description: "Savory mushroom and cheese omelette.",
                servings: 1, prepMinutes: 5, cookMinutes: 10,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Egg",            150, "3 eggs", l),
                    Ing(_guidGenerator, "Mushrooms",      100, "1 cup sliced", l),
                    Ing(_guidGenerator, "Cheddar Cheese",  30, "1/4 cup", l),
                    Ing(_guidGenerator, "Butter",          10, "1 tsp", l),
                    Ing(_guidGenerator, "Onion",           20, "1/4 onion", l),
                });
            recipe.SetInstructions(new[]
            {
                "Whisk eggs with a pinch of salt and pepper in a bowl.",
                "Melt butter in a non-stick pan over medium heat. Sauté diced onion for 2 minutes.",
                "Add sliced mushrooms and cook for 3-4 minutes until golden and moisture has evaporated.",
                "Remove mushroom mixture from pan and set aside.",
                "Pour beaten eggs into the pan. Cook gently, pulling edges inward as they set.",
                "When eggs are almost set, add mushrooms and shredded cheese to one half. Fold and serve.",
            });
            return recipe;
        }

        private Recipe BuildTofuStirFry(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Tofu Stir Fry",
                cuisine: "Asian", authorId: null,
                description: "Quick and healthy vegetarian stir fry.",
                servings: 2, prepMinutes: 10, cookMinutes: 15,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Tofu",              200, "200g", l),
                    Ing(_guidGenerator, "Soy Sauce",          30, "2 tbsp", l),
                    Ing(_guidGenerator, "Bell Pepper",        100, "1 large", l),
                    Ing(_guidGenerator, "Broccoli",           150, "1 cup", l),
                    Ing(_guidGenerator, "Olive Oil",           15, "1 tbsp", l),
                    Ing(_guidGenerator, "White Rice (Dry)",   120, "120g", l),
                });
            recipe.SetInstructions(new[]
            {
                "Press tofu between paper towels for 10 minutes to remove excess moisture, then cube.",
                "Cook rice according to package instructions.",
                "Heat oil in a wok over high heat. Fry tofu cubes for 3-4 minutes per side until golden.",
                "Add broccoli florets and sliced bell pepper. Stir-fry for 4-5 minutes.",
                "Pour soy sauce over the stir fry and toss to coat everything.",
                "Serve over steamed rice.",
            });
            return recipe;
        }

        private Recipe BuildPorkChopSweetPotato(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Pork Chop with Sweet Potato",
                cuisine: "American", authorId: null,
                description: "Pan-seared pork chop with baked sweet potato.",
                servings: 2, prepMinutes: 10, cookMinutes: 30,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Pork Chop",   300, "2 chops", l),
                    Ing(_guidGenerator, "Sweet Potato", 300, "2 medium", l),
                    Ing(_guidGenerator, "Olive Oil",     15, "1 tbsp", l),
                    Ing(_guidGenerator, "Garlic",         5, "2 cloves", l),
                });
            recipe.SetInstructions(new[]
            {
                "Preheat oven to 200°C (400°F). Pierce sweet potatoes with a fork and bake for 30 minutes.",
                "Season pork chops with salt, pepper, and minced garlic.",
                "Heat olive oil in a pan over medium-high heat.",
                "Sear pork chops for 4-5 minutes per side until golden brown and cooked through (internal temp 63°C/145°F).",
                "Rest pork chops for 3 minutes before serving alongside the baked sweet potato.",
            });
            return recipe;
        }

        private Recipe BuildLentilSoup(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Hearty Lentil Soup", authorId: null,
                cuisine: "Mediterranean",
                description: "A warm, comforting lentil and veggie soup.",
                servings: 4, prepMinutes: 15, cookMinutes: 45,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Lentils (Dry)", 200, "1 cup", l),
                    Ing(_guidGenerator, "Carrot",         150, "2 medium", l),
                    Ing(_guidGenerator, "Onion",          100, "1 large", l),
                    Ing(_guidGenerator, "Tomato",         200, "2 large", l),
                    Ing(_guidGenerator, "Olive Oil",       15, "1 tbsp", l),
                    Ing(_guidGenerator, "Garlic",          10, "4 cloves", l),
                });
            recipe.SetInstructions(new[]
            {
                "Rinse lentils under cold water and drain.",
                "Dice onion, carrot, and tomatoes. Mince garlic.",
                "Heat olive oil in a large pot over medium heat. Sauté onion and carrot for 5 minutes.",
                "Add garlic and cook for 1 minute. Stir in diced tomatoes and cook for 3 minutes.",
                "Add lentils and enough water or stock to cover by 5cm. Bring to a boil.",
                "Reduce heat and simmer for 35-40 minutes until lentils are soft. Season and serve.",
            });
            return recipe;
        }

        private Recipe BuildGreekSalad(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Greek Salad",
                cuisine: "Greek", authorId: null,
                description: "Crisp cucumber and tomato salad with cheese.",
                servings: 2, prepMinutes: 10, cookMinutes: 0,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Cucumber",       200, "1 large", l),
                    Ing(_guidGenerator, "Tomato",         200, "2 large", l),
                    Ing(_guidGenerator, "Onion",           50, "1/2 medium", l),
                    Ing(_guidGenerator, "Olive Oil",       30, "2 tbsp", l),
                    Ing(_guidGenerator, "Cheddar Cheese",  50, "50g", l),
                });
            recipe.SetInstructions(new[]
            {
                "Chop cucumber and tomatoes into chunky pieces.",
                "Thinly slice the red onion.",
                "Combine cucumber, tomato, and onion in a large bowl.",
                "Crumble cheese over the top.",
                "Drizzle with olive oil, season with salt and oregano, and toss gently. Serve chilled.",
            });
            return recipe;
        }

        private Recipe BuildAppleAlmondSnack(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Apple & Almond Snack",
                cuisine: "Snack", authorId: null,
                description: "Simple raw snack.",
                servings: 1, prepMinutes: 2, cookMinutes: 0,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Apple",   150, "1 medium", l),
                    Ing(_guidGenerator, "Almonds",  30, "1 small handful", l),
                });
            recipe.SetInstructions(new[]
            {
                "Wash and slice the apple into wedges.",
                "Portion out a small handful of almonds.",
                "Serve apple slices alongside almonds. Enjoy as a snack.",
            });
            return recipe;
        }

        private Recipe BuildChickenRiceBowl(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Chicken Rice Bowl",
                cuisine: "Asian", authorId: null,
                description: "Lean chicken and avocado over rice.",
                servings: 2, prepMinutes: 10, cookMinutes: 20,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Chicken Breast",   250, "250g", l),
                    Ing(_guidGenerator, "White Rice (Dry)", 120, "120g", l),
                    Ing(_guidGenerator, "Avocado",          100, "1/2 avocado", l),
                    Ing(_guidGenerator, "Soy Sauce",         15, "1 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Cook rice according to package instructions.",
                "Slice chicken breast and marinate in soy sauce for 5 minutes.",
                "Heat a pan over medium-high heat and cook chicken for 5-6 minutes per side until cooked through.",
                "Slice avocado.",
                "Divide rice into bowls, top with sliced chicken and avocado. Drizzle with extra soy sauce.",
            });
            return recipe;
        }

        private Recipe BuildBeefBurgerSalad(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Beef Burger Patty Salad",
                cuisine: "American", authorId: null,
                description: "Keto-friendly burger patties on lettuce.",
                servings: 2, prepMinutes: 10, cookMinutes: 15,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Ground Beef", 300, "300g", l),
                    Ing(_guidGenerator, "Lettuce",     100, "4 leaves", l),
                    Ing(_guidGenerator, "Tomato",      100, "1 medium", l),
                    Ing(_guidGenerator, "Onion",        50, "1/2 medium", l),
                });
            recipe.SetInstructions(new[]
            {
                "Season ground beef with salt, pepper, and onion powder. Shape into 2 patties.",
                "Cook patties in a pan over medium-high heat for 4-5 minutes per side until cooked through.",
                "Slice tomato and thinly slice raw onion.",
                "Arrange lettuce leaves on plates as a base.",
                "Place patties on lettuce, top with tomato and onion. Serve with your choice of sauce.",
            });
            return recipe;
        }

        private Recipe BuildPeanutButterAppleToast(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Peanut Butter Apple Toast",
                cuisine: "Breakfast", authorId: null,
                description: "Sweet and crunchy toast.",
                servings: 1, prepMinutes: 5, cookMinutes: 2,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Bread (White)",  35, "1 slice", l),
                    Ing(_guidGenerator, "Peanut Butter",  15, "1 tbsp", l),
                    Ing(_guidGenerator, "Apple",          75, "1/2 apple", l),
                });
            recipe.SetInstructions(new[]
            {
                "Toast bread until golden.",
                "Spread peanut butter on the toast.",
                "Thinly slice half an apple and layer on top.",
                "Optionally drizzle with honey or sprinkle with cinnamon and serve.",
            });
            return recipe;
        }

        private Recipe BuildVeggieFriedRice(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Vegetable Fried Rice",
                cuisine: "Asian", authorId: null,
                description: "Quick fried rice loaded with veggies.",
                servings: 2, prepMinutes: 10, cookMinutes: 15,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "White Rice (Dry)", 120, "120g", l),
                    Ing(_guidGenerator, "Carrot",           100, "1 medium", l),
                    Ing(_guidGenerator, "Broccoli",         100, "1 cup", l),
                    Ing(_guidGenerator, "Egg",              100, "2 eggs", l),
                    Ing(_guidGenerator, "Soy Sauce",         30, "2 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Cook rice and allow to cool completely (day-old rice works best).",
                "Dice carrot and cut broccoli into small florets.",
                "Heat oil in a wok over high heat. Stir-fry carrot and broccoli for 3-4 minutes.",
                "Push vegetables to the side and scramble eggs in the pan until just set.",
                "Add cold rice and toss everything together over high heat for 2-3 minutes.",
                "Add soy sauce, toss to combine, and serve immediately.",
            });
            return recipe;
        }

        private Recipe BuildMushroomPasta(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Mushroom Pasta", authorId: null,
                cuisine: "Italian",
                description: "Garlicky mushroom pasta with spinach.",
                servings: 2, prepMinutes: 10, cookMinutes: 20,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Pasta (Dry)", 160, "160g", l),
                    Ing(_guidGenerator, "Mushrooms",   200, "2 cups", l),
                    Ing(_guidGenerator, "Garlic",       10, "4 cloves", l),
                    Ing(_guidGenerator, "Olive Oil",    30, "2 tbsp", l),
                    Ing(_guidGenerator, "Spinach",     100, "2 handfuls", l),
                });
            recipe.SetInstructions(new[]
            {
                "Bring salted water to a boil and cook pasta until al dente. Reserve 1/4 cup pasta water.",
                "Slice mushrooms and mince garlic.",
                "Heat olive oil in a large pan over medium-high heat. Sauté mushrooms for 5-6 minutes until golden.",
                "Add garlic and cook for 1 minute. Add spinach and toss until wilted.",
                "Add drained pasta to the pan with a splash of pasta water. Toss well to combine.",
                "Season with salt and pepper. Serve with grated parmesan if desired.",
            });
            return recipe;
        }

        private Recipe BuildSpicyTofuScramble(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Spicy Tofu Scramble", authorId: null,
                cuisine: "American",
                description: "Vegan scramble with veggies.",
                servings: 2, prepMinutes: 10, cookMinutes: 15,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Tofu",      300, "1 block", l),
                    Ing(_guidGenerator, "Onion",      50, "1/2 medium", l),
                    Ing(_guidGenerator, "Tomato",    100, "1 medium", l),
                    Ing(_guidGenerator, "Spinach",    50, "1 handful", l),
                    Ing(_guidGenerator, "Olive Oil",  15, "1 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Press tofu dry with paper towels and crumble into a bowl.",
                "Dice onion and tomato.",
                "Heat olive oil in a pan over medium heat. Sauté onion for 3 minutes.",
                "Add crumbled tofu and cook for 4-5 minutes, stirring to break it up.",
                "Add tomato, spinach, and season with chili flakes, salt, and turmeric.",
                "Cook for 2-3 minutes until spinach is wilted. Serve warm.",
            });
            return recipe;
        }

        private Recipe BuildGarlicButterPorkChops(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Garlic Butter Pork Chops",
                cuisine: "American", authorId: null,
                description: "Rich and tender pork chops with potatoes.",
                servings: 2, prepMinutes: 10, cookMinutes: 25,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Pork Chop", 300, "2 chops", l),
                    Ing(_guidGenerator, "Garlic",     10, "4 cloves", l),
                    Ing(_guidGenerator, "Butter",     20, "2 tbsp", l),
                    Ing(_guidGenerator, "Potato",    300, "2 medium", l),
                });
            recipe.SetInstructions(new[]
            {
                "Boil or microwave potatoes until just tender. Slice into thick rounds.",
                "Season pork chops generously with salt and pepper.",
                "Melt 1 tbsp butter in a pan over medium-high heat. Sear chops for 4 minutes per side.",
                "Reduce heat to medium. Add remaining butter and minced garlic to the pan.",
                "Baste chops with the garlic butter for 2 minutes. Rest for 3 minutes.",
                "Add potato rounds to the pan and toss in remaining butter. Serve alongside pork chops.",
            });
            return recipe;
        }

        private Recipe BuildBreakfastBurritoBowl(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Breakfast Burrito Bowl",
                cuisine: "Mexican", authorId: null,
                description: "Deconstructed burrito for breakfast.",
                servings: 2, prepMinutes: 10, cookMinutes: 15,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Black Beans (Cooked)", 150, "1/2 can", l),
                    Ing(_guidGenerator, "Egg",                  100, "2 eggs", l),
                    Ing(_guidGenerator, "Avocado",              100, "1/2 avocado", l),
                    Ing(_guidGenerator, "Tomato",               100, "1 medium", l),
                    Ing(_guidGenerator, "Brown Rice (Dry)",     100, "100g", l),
                });
            recipe.SetInstructions(new[]
            {
                "Cook brown rice according to package instructions.",
                "Warm black beans in a small pan, season with cumin and salt.",
                "Scramble eggs in a non-stick pan over medium heat until just set.",
                "Dice tomato and slice avocado.",
                "Divide rice between bowls. Top with black beans, scrambled eggs, avocado, and tomato.",
                "Season with salt and lime juice. Serve immediately.",
            });
            return recipe;
        }

        private Recipe BuildChickenMushroomSaute(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Chicken & Mushroom Sauté",
                cuisine: "French", authorId: null,
                description: "Simple pan-fried chicken and mushrooms.",
                servings: 2, prepMinutes: 10, cookMinutes: 20,
                difficulty: DifficultyLevel.Medium,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Chicken Breast", 250, "250g", l),
                    Ing(_guidGenerator, "Mushrooms",      200, "2 cups", l),
                    Ing(_guidGenerator, "Onion",          100, "1 medium", l),
                    Ing(_guidGenerator, "Olive Oil",       15, "1 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Slice chicken breast into strips. Slice mushrooms and dice onion.",
                "Heat olive oil in a pan over medium-high heat.",
                "Cook chicken strips for 5-6 minutes until golden and cooked through. Set aside.",
                "In the same pan, sauté onion for 3 minutes. Add mushrooms and cook until golden, about 5 minutes.",
                "Return chicken to the pan, season with salt, pepper, and fresh thyme if available.",
                "Toss everything together and serve. Pairs well with crusty bread or rice.",
            });
            return recipe;
        }

        private Recipe BuildCarrotLentilMash(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Carrot & Lentil Mash",
                cuisine: "British", authorId: null,
                description: "Soft comforting mash.",
                servings: 2, prepMinutes: 10, cookMinutes: 30,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Carrot",        200, "2 large", l),
                    Ing(_guidGenerator, "Lentils (Dry)", 100, "1/2 cup", l),
                    Ing(_guidGenerator, "Butter",         20, "2 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Rinse lentils under cold water. Peel and chop carrots into chunks.",
                "Combine lentils and carrots in a pot, cover with water, and bring to a boil.",
                "Reduce heat and simmer for 25-30 minutes until both are completely soft.",
                "Drain well. Add butter and mash together until smooth.",
                "Season with salt and pepper. Serve warm as a side dish.",
            });
            return recipe;
        }

        private Recipe BuildYogurtAlmonds(Dictionary<string, Guid> l)
        {
            var recipe = Recipe.CreateSeed(_guidGenerator.Create(),
                name: "Yogurt with Almonds",
                cuisine: "Snack", authorId: null,
                description: "Protein-rich yogurt snack.",
                servings: 1, prepMinutes: 2, cookMinutes: 0,
                difficulty: DifficultyLevel.Easy,
                ingredients: new[]
                {
                    Ing(_guidGenerator, "Greek Yogurt", 200, "200g", l),
                    Ing(_guidGenerator, "Almonds",       30, "1 handful", l),
                    Ing(_guidGenerator, "Honey",         15, "1 tbsp", l),
                });
            recipe.SetInstructions(new[]
            {
                "Spoon Greek yogurt into a bowl.",
                "Scatter almonds over the yogurt.",
                "Drizzle with honey and serve immediately.",
            });
            return recipe;
        }


        // ── Helpers ───────────────────────────────────────────────────────────────

        private static (string Name, float Grams, string Display, Guid? NutritionId) Ing(
            IGuidGenerator gen,
            string name,
            float grams,
            string display,
            Dictionary<string, Guid> lookup)
        {
            return (name, grams, display, lookup.TryGetValue(name, out var id) ? id : null);
        }
    }
}
