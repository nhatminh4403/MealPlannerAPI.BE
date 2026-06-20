using MealPlannerAPI.DataSeeder.Helpers;
using static MealPlannerAPI.DataSeeder.Helpers.SeedDemoUserDefs;
using MealPlannerAPI.Enums;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.Nutritions;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace MealPlannerAPI.DataSeeder;

public class MealPlannerAPIDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IIngredientNutritionRepository _ingredientNutritionRepository;

    public int Order => 3;

    public MealPlannerAPIDataSeedContributor(
        IMealPlanRepository mealPlanRepository,
        IShoppingListRepository shoppingListRepository,
        IRecipeRepository recipeRepository,
        IUserNotificationRepository notificationRepository,
        IRepository<UserProfile, Guid> userProfileRepository,
        IIdentityUserRepository identityUserRepository,
        IGuidGenerator guidGenerator,
        IdentityUserManager identityUserManager,
        IIngredientNutritionRepository ingredientNutritionRepository)
    {
        _mealPlanRepository = mealPlanRepository;
        _shoppingListRepository = shoppingListRepository;
        _recipeRepository = recipeRepository;
        _notificationRepository = notificationRepository;
        _userProfileRepository = userProfileRepository;
        _identityUserRepository = identityUserRepository;
        _guidGenerator = guidGenerator;
        _identityUserManager = identityUserManager;
        _ingredientNutritionRepository = ingredientNutritionRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Idempotency: use a demo user as sentinel
        if (await _identityUserRepository.FindByNormalizedUserNameAsync("CHEF_MARIA") != null) return;

        var ingredients = await _ingredientNutritionRepository.GetListAsync();
        var lookup = ingredients.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

        var demoUsers = await SeedDemoUsersAsync();
        var allRecipes = await SeedRecipesAsync(demoUsers, lookup);

        await SeedMealPlansAsync(demoUsers, allRecipes);
        await SeedShoppingListsAsync(demoUsers);
        await SeedNotificationsAsync(demoUsers);
    }

    // ── Demo users ────────────────────────────────────────────────────────────

    private async Task<List<UserProfile>> SeedDemoUsersAsync()
    {
        var users = new List<UserProfile>();

        var admin = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");
        if (admin != null)
        {
            var adminProfile = await _userProfileRepository.FindAsync(admin.Id);
            if (adminProfile != null) users.Add(adminProfile);
        }

        // To add a new demo user: append one entry to SeedDemoUserDefs.All.
        foreach (var data in SeedDemoUserDefs.All)
        {
            var existing = await _identityUserRepository.FindByNormalizedUserNameAsync(data.UserName.ToUpperInvariant());
            if (existing != null)
            {
                var profile = await _userProfileRepository.FindAsync(existing.Id);
                if (profile != null) users.Add(profile);
                continue;
            }

            var userId = _guidGenerator.Create();
            var user = new UserProfile(userId, data.UserName, data.Email)
            {
                Name = data.Name,
                Surname = data.Surname,
                Specialty = data.Specialty,
                Followers = data.Followers,
                Following = data.Following,
                ProfileVisibility = VisibilityLevel.Public,
                RecipesVisibility = VisibilityLevel.Public,
            };

            var result = await _identityUserManager.CreateAsync(user, data.Name + ".123");
            if (result.Succeeded) users.Add(user);
            // If it failed (e.g. race condition on re-run), skip — don't throw.
        }

        return users;
    }

    // ── Demo recipes ──────────────────────────────────────────────────────────

    private sealed record DemoRecipeProfile(
        string Name,
        string Cuisine,
        DifficultyLevel Difficulty,
        int PrepTime,
        int CookTime,
        int Servings,
        double Rating,
        int Reviews,
        string Description,
        int UserIndex,
        IReadOnlyList<(string Name, float Grams, string Display)> Ingredients,
        IReadOnlyList<string> Instructions
    );

    // UserIndex maps to the users list built by SeedDemoUsersAsync (0 = admin, 1 = chef_maria, …).
    private static readonly IReadOnlyList<DemoRecipeProfile> DemoRecipes = new[]
    {
        // ── Chef Maria — Italian ──────────────────────────────────────────────
        new DemoRecipeProfile(
            Name: "Classic Spaghetti Carbonara", Cuisine: "Italian",
            Difficulty: DifficultyLevel.Medium, PrepTime: 10, CookTime: 20, Servings: 4,
            Rating: 4.9, Reviews: 850, UserIndex: 1,
            Description: "Authentic Roman carbonara with eggs, pecorino, and guanciale.",
            Ingredients: new[]
            {
                ("Spaghetti",       400f, "400g"),
                ("Eggs",            150f, "3 large"),
                ("Pecorino Romano", 100f, "100g grated"),
                ("Guanciale",       150f, "150g diced"),
                ("Black Pepper",      5f, "to taste"),
                ("Salt",              5f, "for pasta water"),
            },
            Instructions: new[]
            {
                "Bring a large pot of salted water to boil and cook spaghetti according to package directions.",
                "While pasta cooks, dice guanciale and cook in a large pan until crispy.",
                "Beat eggs with grated pecorino and black pepper in a bowl.",
                "Drain pasta, reserving 1 cup pasta water.",
                "Add hot pasta to the pan with guanciale, remove from heat.",
                "Quickly stir in egg mixture, adding pasta water to create a creamy sauce.",
                "Serve immediately with extra pecorino and black pepper.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Margherita Pizza", Cuisine: "Italian",
            Difficulty: DifficultyLevel.Easy, PrepTime: 90, CookTime: 12, Servings: 2,
            Rating: 4.8, Reviews: 1200, UserIndex: 1,
            Description: "Classic Neapolitan pizza with fresh mozzarella and basil.",
            Ingredients: new[]
            {
                ("Bread (White)", 300f, "pizza dough"),
                ("Tomato",        400f, "1 can crushed"),
                ("Mozzarella",    250f, "250g fresh"),
                ("Olive Oil",      20f, "2 tbsp"),
                ("Garlic",          5f, "2 cloves minced"),
                ("Salt",            3f, "to taste"),
            },
            Instructions: new[]
            {
                "Stretch dough into two rounds on a floured surface.",
                "Spread crushed tomato, garlic, and olive oil over each base.",
                "Top with mozzarella and bake at 250°C (480°F) for 10-12 minutes until bubbly.",
                "Finish with fresh basil and a drizzle of olive oil.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Tiramisu", Cuisine: "Italian",
            Difficulty: DifficultyLevel.Medium, PrepTime: 30, CookTime: 0, Servings: 8,
            Rating: 4.7, Reviews: 650, UserIndex: 1,
            Description: "Traditional Italian coffee-flavored dessert.",
            Ingredients: new[]
            {
                ("Eggs",          100f, "2 large yolks"),
                ("Cheddar Cheese",250f, "mascarpone substitute"),
                ("Honey",          80f, "1/3 cup sugar substitute"),
                ("Bread (White)", 200f, "ladyfinger substitute"),
                ("Whole Milk",    120f, "1/2 cup brewed coffee"),
                ("Cocoa Powder",   10f, "for dusting"),
            },
            Instructions: new[]
            {
                "Whisk egg yolks with honey until pale; fold in mascarpone.",
                "Briefly dip ladyfingers in coffee and layer in a dish.",
                "Spread half the cream, repeat layers, and chill 4 hours.",
                "Dust with cocoa before serving.",
            }
        ),

        // ── Healthy John ──────────────────────────────────────────────────────
        new DemoRecipeProfile(
            Name: "Quinoa Buddha Bowl", Cuisine: "Healthy",
            Difficulty: DifficultyLevel.Easy, PrepTime: 15, CookTime: 20, Servings: 2,
            Rating: 4.6, Reviews: 920, UserIndex: 2,
            Description: "Nutritious bowl with quinoa, roasted vegetables, and tahini dressing.",
            Ingredients: new[]
            {
                ("Quinoa",       180f, "1 cup dry"),
                ("Sweet Potato", 300f, "1 large cubed"),
                ("Chickpeas",    240f, "1 can drained"),
                ("Kale",          80f, "2 cups chopped"),
                ("Tahini",        30f, "2 tbsp dressing"),
                ("Olive Oil",     15f, "1 tbsp"),
                ("Lemon Juice",   15f, "1 tbsp"),
            },
            Instructions: new[]
            {
                "Rinse quinoa and cook according to package directions.",
                "Cube sweet potato and roast at 400°F for 25 minutes.",
                "Drain and rinse chickpeas, season and roast for 20 minutes.",
                "Massage kale with a bit of olive oil and lemon juice.",
                "Assemble bowl with quinoa, roasted vegetables, chickpeas, and kale.",
                "Drizzle with tahini dressing and serve.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Grilled Salmon with Asparagus", Cuisine: "Healthy",
            Difficulty: DifficultyLevel.Easy, PrepTime: 10, CookTime: 15, Servings: 2,
            Rating: 4.7, Reviews: 780, UserIndex: 2,
            Description: "Omega-3 rich salmon with fresh asparagus.",
            Ingredients: new[]
            {
                ("Salmon",      400f, "2 fillets"),
                ("Asparagus",   250f, "1 bunch"),
                ("Olive Oil",    20f, "2 tbsp"),
                ("Garlic",        5f, "2 cloves"),
                ("Lemon Juice",  15f, "1 tbsp"),
                ("Salt",          3f, "to taste"),
                ("Black Pepper",  3f, "to taste"),
            },
            Instructions: new[]
            {
                "Season salmon with salt, pepper, and lemon juice.",
                "Toss asparagus with olive oil and garlic.",
                "Grill salmon 4 minutes per side over medium-high heat.",
                "Grill asparagus until tender-crisp, about 6 minutes.",
                "Serve salmon over asparagus with lemon wedges.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Green Smoothie Bowl", Cuisine: "Healthy",
            Difficulty: DifficultyLevel.Easy, PrepTime: 10, CookTime: 0, Servings: 1,
            Rating: 4.5, Reviews: 540, UserIndex: 2,
            Description: "Energizing breakfast bowl with spinach, banana, and berries.",
            Ingredients: new[]
            {
                ("Spinach",      60f, "2 cups"),
                ("Banana",      120f, "1 frozen"),
                ("Greek Yogurt",150f, "2/3 cup"),
                ("Apple",       100f, "1 small"),
                ("Honey",        15f, "1 tbsp"),
                ("Almonds",      20f, "for topping"),
            },
            Instructions: new[]
            {
                "Blend spinach, banana, yogurt, apple, and honey until smooth.",
                "Pour into a bowl and top with sliced fruit and almonds.",
                "Serve immediately while cold.",
            }
        ),

        // ── Baker Sarah ───────────────────────────────────────────────────────
        new DemoRecipeProfile(
            Name: "Chocolate Chip Cookies", Cuisine: "American",
            Difficulty: DifficultyLevel.Easy, PrepTime: 15, CookTime: 12, Servings: 24,
            Rating: 4.9, Reviews: 1500, UserIndex: 3,
            Description: "Perfectly chewy chocolate chip cookies.",
            Ingredients: new[]
            {
                ("All-Purpose Flour", 280f, "2 1/4 cups"),
                ("Butter",           225f, "1 cup softened"),
                ("Brown Sugar",      200f, "1 cup packed"),
                ("Eggs",             100f, "2 large"),
                ("Chocolate Chips",  340f, "2 cups"),
                ("Vanilla Extract",    5f, "1 tsp"),
                ("Baking Soda",        5f, "1 tsp"),
                ("Salt",               5f, "1/2 tsp"),
            },
            Instructions: new[]
            {
                "Preheat oven to 375°F (190°C).",
                "Cream together butter and sugars until fluffy.",
                "Beat in eggs one at a time.",
                "Mix in flour, baking soda, and salt.",
                "Fold in chocolate chips.",
                "Drop rounded tablespoons onto baking sheets.",
                "Bake for 10-12 minutes until golden brown.",
                "Cool on baking sheet for 5 minutes before transferring.",
            }
        ),

        new DemoRecipeProfile(
            Name: "New York Cheesecake", Cuisine: "American",
            Difficulty: DifficultyLevel.Hard, PrepTime: 30, CookTime: 60, Servings: 12,
            Rating: 4.8, Reviews: 890, UserIndex: 3,
            Description: "Rich and creamy classic cheesecake.",
            Ingredients: new[]
            {
                ("Cheddar Cheese", 900f, "cream cheese substitute"),
                ("Eggs",           150f, "3 large"),
                ("Honey",          150f, "3/4 cup sugar substitute"),
                ("Butter",         120f, "1/2 cup melted"),
                ("Bread (White)",  200f, "graham cracker substitute"),
                ("Whole Milk",      60f, "1/4 cup sour cream substitute"),
                ("Vanilla Extract",  5f, "1 tsp"),
            },
            Instructions: new[]
            {
                "Mix crushed crackers with melted butter; press into a springform pan.",
                "Beat cream cheese, honey, eggs, and vanilla until smooth.",
                "Pour over crust and bake in a water bath at 160°C (325°F) for 55 minutes.",
                "Cool completely, then chill at least 4 hours before slicing.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Sourdough Bread", Cuisine: "Artisan",
            Difficulty: DifficultyLevel.Hard, PrepTime: 240, CookTime: 45, Servings: 1,
            Rating: 4.7, Reviews: 420, UserIndex: 3,
            Description: "Homemade sourdough with crispy crust.",
            Ingredients: new[]
            {
                ("Bread (White)", 500f, "500g flour substitute"),
                ("Whole Milk",    350f, "warm water substitute"),
                ("Salt",           10f, "2 tsp"),
                ("Oats (Dry)",    100f, "starter feed"),
            },
            Instructions: new[]
            {
                "Feed starter with flour and water; let rise until bubbly.",
                "Mix dough, autolyse 30 minutes, then add salt.",
                "Stretch and fold every 30 minutes for 2 hours.",
                "Shape, proof overnight, then bake in a Dutch oven at 230°C (450°F) for 45 minutes.",
                "Cool completely before slicing.",
            }
        ),

        // ── Asian Fusion Mike ─────────────────────────────────────────────────
        new DemoRecipeProfile(
            Name: "Pad Thai", Cuisine: "Thai",
            Difficulty: DifficultyLevel.Medium, PrepTime: 20, CookTime: 15, Servings: 2,
            Rating: 4.6, Reviews: 1100, UserIndex: 4,
            Description: "Classic Thai stir-fried noodles with tamarind sauce.",
            Ingredients: new[]
            {
                ("Rice Noodles",  200f, "200g flat"),
                ("Egg",           100f, "2 eggs"),
                ("Tofu",          150f, "150g firm"),
                ("Peanut Butter",  30f, "2 tbsp sauce"),
                ("Soy Sauce",      30f, "2 tbsp"),
                ("Bell Pepper",   100f, "1 sliced"),
                ("Garlic",          5f, "2 cloves"),
                ("Lime Juice",     15f, "1 tbsp"),
            },
            Instructions: new[]
            {
                "Soak rice noodles until pliable; drain well.",
                "Stir-fry tofu and garlic; push aside and scramble eggs in the pan.",
                "Add noodles, soy sauce, and peanut butter; toss to coat.",
                "Finish with bell pepper and lime juice; serve hot.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Korean Bibimbap", Cuisine: "Korean",
            Difficulty: DifficultyLevel.Medium, PrepTime: 30, CookTime: 20, Servings: 4,
            Rating: 4.7, Reviews: 680, UserIndex: 4,
            Description: "Mixed rice bowl with vegetables and gochujang.",
            Ingredients: new[]
            {
                ("White Rice (Dry)", 200f, "1 cup dry"),
                ("Ground Beef",      250f, "250g"),
                ("Spinach",          100f, "2 cups blanched"),
                ("Carrot",            80f, "1 julienned"),
                ("Egg",               50f, "1 fried"),
                ("Soy Sauce",         30f, "2 tbsp"),
                ("Sesame Oil",        10f, "2 tsp"),
                ("Gochujang",         20f, "1 tbsp"),
            },
            Instructions: new[]
            {
                "Cook rice and keep warm.",
                "Sauté seasoned ground beef until browned.",
                "Blanch spinach and sauté carrot separately.",
                "Fry an egg sunny-side up.",
                "Arrange rice in bowls with vegetables, beef, egg, soy sauce, and gochujang.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Japanese Ramen", Cuisine: "Japanese",
            Difficulty: DifficultyLevel.Hard, PrepTime: 60, CookTime: 180, Servings: 4,
            Rating: 4.8, Reviews: 950, UserIndex: 4,
            Description: "Rich tonkotsu ramen with homemade broth.",
            Ingredients: new[]
            {
                ("Pasta (Dry)",  400f, "4 servings noodles"),
                ("Pork Chop",    300f, "chashu substitute"),
                ("Egg",          100f, "2 soft-boiled"),
                ("Spinach",       80f, "1 cup"),
                ("Mushrooms",    100f, "1 cup sliced"),
                ("Soy Sauce",     40f, "broth seasoning"),
                ("Garlic",        10f, "4 cloves"),
                ("Green Onion",   20f, "2 stalks"),
            },
            Instructions: new[]
            {
                "Simmer broth with garlic, soy sauce, and mushrooms for 30 minutes.",
                "Cook noodles separately until al dente.",
                "Slice pork and soft-boil eggs.",
                "Divide noodles into bowls, ladle hot broth, and top with pork, egg, spinach, and green onion.",
            }
        ),

        // ── Vegan Emma ────────────────────────────────────────────────────────
        new DemoRecipeProfile(
            Name: "Vegan Lentil Curry", Cuisine: "Indian",
            Difficulty: DifficultyLevel.Easy, PrepTime: 15, CookTime: 30, Servings: 6,
            Rating: 4.7, Reviews: 1350, UserIndex: 5,
            Description: "Creamy coconut lentil curry with aromatic spices.",
            Ingredients: new[]
            {
                ("Lentils (Dry)", 200f, "1 cup"),
                ("Tomato",        400f, "1 can"),
                ("Onion",         120f, "1 large"),
                ("Garlic",         10f, "3 cloves"),
                ("Spinach",       100f, "2 cups"),
                ("Olive Oil",      20f, "2 tbsp"),
                ("Curry Powder",   10f, "2 tbsp"),
                ("Coconut Milk",  200f, "1 can"),
            },
            Instructions: new[]
            {
                "Sauté onion and garlic in olive oil until fragrant.",
                "Stir in curry powder, then lentils, tomato, and coconut milk.",
                "Simmer 25-30 minutes until lentils are tender.",
                "Fold in spinach; season and serve with rice.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Chickpea Tacos", Cuisine: "Mexican",
            Difficulty: DifficultyLevel.Easy, PrepTime: 10, CookTime: 20, Servings: 4,
            Rating: 4.6, Reviews: 890, UserIndex: 5,
            Description: "Spiced chickpeas in soft tortillas with avocado.",
            Ingredients: new[]
            {
                ("Chickpeas",     240f, "1 can drained"),
                ("Avocado",       150f, "1 ripe"),
                ("Tomato",        150f, "2 diced"),
                ("Lettuce",        80f, "shredded"),
                ("Bell Pepper",   100f, "1 sliced"),
                ("Olive Oil",      15f, "1 tbsp"),
                ("Taco Seasoning", 10f, "1 packet"),
                ("Corn Tortillas",200f, "8 small"),
            },
            Instructions: new[]
            {
                "Warm tortillas in a dry pan.",
                "Sauté chickpeas with taco seasoning until heated through.",
                "Fill tortillas with chickpeas, lettuce, tomato, and bell pepper.",
                "Top with sliced avocado and serve.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Vegan Chocolate Cake", Cuisine: "Dessert",
            Difficulty: DifficultyLevel.Medium, PrepTime: 20, CookTime: 35, Servings: 10,
            Rating: 4.8, Reviews: 1120, UserIndex: 5,
            Description: "Moist and rich chocolate cake without eggs or dairy.",
            Ingredients: new[]
            {
                ("Oats (Dry)",      250f, "2 cups flour substitute"),
                ("Honey",           200f, "1 cup sugar substitute"),
                ("Olive Oil",        80f, "1/3 cup"),
                ("Banana",          120f, "2 mashed"),
                ("Almonds",          50f, "cocoa substitute"),
                ("Baking Powder",    10f, "2 tsp"),
                ("Salt",              3f, "pinch"),
                ("Vanilla Extract",   5f, "1 tsp"),
            },
            Instructions: new[]
            {
                "Preheat oven to 180°C (350°F) and grease a cake pan.",
                "Whisk dry ingredients; stir in mashed banana, oil, honey, and vanilla.",
                "Bake 30-35 minutes until a toothpick comes out clean.",
                "Cool completely before slicing.",
            }
        ),

        // ── Admin-authored showcase ───────────────────────────────────────────
        new DemoRecipeProfile(
            Name: "Admin Herb Roast Chicken", Cuisine: "Continental",
            Difficulty: DifficultyLevel.Medium, PrepTime: 20, CookTime: 90, Servings: 4,
            Rating: 4.8, Reviews: 600, UserIndex: 0,
            Description: "A classic whole roast chicken with fresh herbs and garlic.",
            Ingredients: new[]
            {
                ("Chicken Breast", 1200f, "whole chicken"),
                ("Garlic",           20f, "1 head"),
                ("Olive Oil",        30f, "2 tbsp"),
                ("Butter",           50f, "3 tbsp"),
                ("Onion",           100f, "1 large"),
            },
            Instructions: new[]
            {
                "Preheat oven to 200°C (400°F).",
                "Rub the chicken with butter, olive oil, minced garlic, and your choice of fresh herbs.",
                "Stuff the cavity with halved onion and garlic head.",
                "Roast for 80-90 minutes until juices run clear.",
                "Rest for 10 minutes before carving.",
            }
        ),

        new DemoRecipeProfile(
            Name: "Admin Garden Vegetable Soup", Cuisine: "Continental",
            Difficulty: DifficultyLevel.Easy, PrepTime: 15, CookTime: 30, Servings: 6,
            Rating: 4.5, Reviews: 320, UserIndex: 0,
            Description: "A simple and nourishing vegetable soup.",
            Ingredients: new[]
            {
                ("Carrot",       200f, "2 large"),
                ("Potato",       300f, "2 medium"),
                ("Onion",        100f, "1 large"),
                ("Tomato",       300f, "3 medium"),
                ("Spinach",       80f, "1 cup"),
                ("Olive Oil",     20f, "2 tbsp"),
            },
            Instructions: new[]
            {
                "Dice all vegetables into bite-sized pieces.",
                "Heat olive oil in a large pot and sauté onion for 3 minutes.",
                "Add carrot, potato, and tomato. Pour in 1.5 litres of water or broth.",
                "Bring to a boil and simmer for 20-25 minutes until vegetables are tender.",
                "Stir in spinach, season with salt and pepper, and serve hot.",
            }
        ),
    };

    private async Task<List<Recipe>> SeedRecipesAsync(List<UserProfile> users, Dictionary<string, Guid> lookup)
    {
        var recipes = new List<Recipe>();
        if (users.Count == 0) return recipes;

        // Alias map: ingredient names used in DemoRecipes that differ from the nutrition table keys.
        var aliasMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Eggs"] = "Egg",
            ["Spaghetti"] = "Pasta (Dry)",
            ["Rice Noodles"] = "Pasta (Dry)",
            ["Mozzarella"] = "Cheddar Cheese",
            ["Chickpeas"] = "Black Beans (Cooked)",
            ["Asparagus"] = "Broccoli",
            ["Quinoa"] = "Brown Rice (Dry)",
            ["Kale"] = "Spinach",
            ["Tahini"] = "Peanut Butter",
            ["All-Purpose Flour"] = "Oats (Dry)",
            ["Brown Sugar"] = "Honey",
            ["Chocolate Chips"] = "Almonds",
        };

        Guid? ResolveId(string name)
        {
            if (lookup.TryGetValue(name, out var id)) return id;
            if (aliasMap.TryGetValue(name, out var alias) && lookup.TryGetValue(alias, out id)) return id;
            return null;
        }

        foreach (var profile in DemoRecipes)
        {
            var authorId = profile.UserIndex < users.Count ? users[profile.UserIndex].Id : users[0].Id;
            var authorName = profile.UserIndex < users.Count ? users[profile.UserIndex].UserName : users[0].UserName;

            var ingredients = profile.Ingredients
                .Select(i => (i.Name, i.Grams, i.Display, ResolveId(i.Name)))
                .ToList();

            var recipe = Recipe.CreateSeed(
                id: _guidGenerator.Create(),
                name: profile.Name,
                cuisine: profile.Cuisine,
                description: profile.Description,
                servings: profile.Servings,
                prepMinutes: profile.PrepTime,
                cookMinutes: profile.CookTime,
                difficulty: profile.Difficulty,
                authorId: authorId,
                ingredients: ingredients,
                authorUsername: authorName

            );

            recipe.SetInstructions(profile.Instructions);
            recipe.Rating = profile.Rating;
            recipe.ReviewCount = profile.Reviews;
            recipe.ImageUrl = RecipeSeedImages.TryGet(profile.Name);

            await _recipeRepository.InsertAsync(recipe, autoSave: true);
            recipes.Add(recipe);
        }

        return recipes;
    }

    // ── Meal plans ────────────────────────────────────────────────────────────

    private async Task SeedMealPlansAsync(List<UserProfile> users, List<Recipe> recipes)
    {
        if (users.Count == 0 || recipes.Count == 0) return;

        var nextMonday = MealPlan.GetWeekStart(DateTime.UtcNow.AddDays(7));

        for (int i = 0; i < Math.Min(3, users.Count); i++)
        {
            var user = users[i];
            var userRecipes = recipes.Where(r => r.AuthorId == user.Id).Take(5).ToList();
            if (userRecipes.Count == 0) userRecipes = recipes.Take(5).ToList();

            var mealPlan = new MealPlan(
                id: _guidGenerator.Create(),
                userId: user.Id,
                weekStartDate: nextMonday
            );

            // To add more meal entries per user: append AddEntry calls here.
            if (userRecipes.Count > 0)
                mealPlan.AddEntry(_guidGenerator.Create(), DayOfWeek.Monday, "Monday Dinner", MealType.Dinner, userRecipes[0].Name, "19:00", userRecipes[0].Id);
            if (userRecipes.Count > 1)
                mealPlan.AddEntry(_guidGenerator.Create(), DayOfWeek.Wednesday, "Wednesday Lunch", MealType.Lunch, userRecipes[1].Name, "12:30", userRecipes[1].Id);
            if (userRecipes.Count > 2)
                mealPlan.AddEntry(_guidGenerator.Create(), DayOfWeek.Friday, "Friday Dinner", MealType.Dinner, userRecipes[2].Name, "18:30", userRecipes[2].Id);

            await _mealPlanRepository.InsertAsync(mealPlan, autoSave: true);
        }
    }

    // ── Shopping lists ────────────────────────────────────────────────────────

    private async Task SeedShoppingListsAsync(List<UserProfile> users)
    {
        if (users.Count == 0) return;

        // To add a new shopping list: append one entry here.
        var shoppingListData = new[]
        {
            new { Name = "Weekly Groceries", Items = new[] {
                ("Spaghetti",       1m,   "pack",  ShoppingItemCategory.Pantry,     false),
                ("Minced Beef",   500m,   "g",     ShoppingItemCategory.Meat,       false),
                ("Mixed Greens",    1m,   "bag",   ShoppingItemCategory.Vegetables, true),
                ("Parmesan Cheese",200m,  "g",     ShoppingItemCategory.Dairy,      false),
            }},
            new { Name = "Healthy Week Prep", Items = new[] {
                ("Quinoa",        500m,   "g",     ShoppingItemCategory.Pantry,     false),
                ("Salmon Fillets",400m,   "g",     ShoppingItemCategory.Meat,       false),
                ("Asparagus",       1m,   "bunch", ShoppingItemCategory.Vegetables, false),
                ("Spinach",       200m,   "g",     ShoppingItemCategory.Vegetables, true),
            }},
            new { Name = "Baking Supplies", Items = new[] {
                ("All-Purpose Flour",1m,  "kg",    ShoppingItemCategory.Pantry,     false),
                ("Chocolate Chips", 300m, "g",     ShoppingItemCategory.Pantry,     false),
                ("Butter",          250m, "g",     ShoppingItemCategory.Dairy,      true),
                ("Eggs",             12m, "pcs",   ShoppingItemCategory.Dairy,      false),
            }},
        };

        for (int i = 0; i < Math.Min(shoppingListData.Length, users.Count); i++)
        {
            var data = shoppingListData[i];
            var shoppingList = new ShoppingList(
                id: _guidGenerator.Create(),
                userId: users[i].Id,
                name: data.Name
            );

            foreach (var (name, quantity, unit, category, isCompleted) in data.Items)
                shoppingList.AddItem(_guidGenerator.Create(), name, quantity, isCompleted, unit, category);

            await _shoppingListRepository.InsertAsync(shoppingList, autoSave: true);
        }
    }

    // ── Notifications ─────────────────────────────────────────────────────────

    private async Task SeedNotificationsAsync(List<UserProfile> users)
    {
        if (users.Count == 0) return;

        var notificationData = new[]
        {
            new { Type = NotificationType.MealReminder,     Title = "Upcoming Meal",           Message = "Don't forget to prepare your planned meal for dinner today!" },
            new { Type = NotificationType.ShoppingListAlert,Title = "Shopping List Reminder",  Message = "You have uncompleted items in your shopping list." },
        };

        for (int i = 0; i < Math.Min(2, users.Count); i++)
        {
            foreach (var data in notificationData)
            {
                var notification = new UserNotification(
                    id: _guidGenerator.Create(),
                    userId: users[i].Id,
                    type: data.Type,
                    title: data.Title,
                    message: data.Message,
                    avatarUrl: null
                );
                await _notificationRepository.InsertAsync(notification, autoSave: true);
            }
        }
    }
}