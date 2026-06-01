using MealPlannerAPI.DataSeeder.Helpers;
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

namespace MealPlannerAPI.DataSeeder
{
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
            // Check if data already exists
            // Use a demo user as the idempotency sentinel
            var alreadySeeded = await _identityUserRepository.FindByNormalizedUserNameAsync("CHEF_MARIA");

            if (alreadySeeded != null) return;
            var lookup = context.Properties.TryGetValue("IngredientNutritionLookup", out var lookupObj)
                                                ? lookupObj as Dictionary<string, Guid>
                                                : null;

            if (lookup == null)
            {
                var allNutritions = await _ingredientNutritionRepository.GetListAsync();
                lookup = allNutritions.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
            }
            var demoUsers = await SeedDemoUsersAsync();

            // 2. Seed Recipes for each user
            var allRecipes = await SeedRecipesAsync(demoUsers, lookup);

            // 3. Seed Meal Plans
            await SeedMealPlansAsync(demoUsers, allRecipes);

            // 4. Seed Shopping Lists
            await SeedShoppingListsAsync(demoUsers);

            // 5. Seed Notifications
            await SeedNotificationsAsync(demoUsers);
        }

        private async Task<List<UserProfile>> SeedDemoUsersAsync()
        {
            var users = new List<UserProfile>();

            // Get admin user
            var admin = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");
            if (admin != null)
            {
                var adminProfile = await _userProfileRepository.FindAsync(admin.Id);
                if (adminProfile != null)
                {
                    users.Add(adminProfile);
                }
            }

            // Demo user data
            var demoUserData = new[]
            {
                new { UserName = "chef_maria", Email = "maria@mealplanner.demo", Name = "Maria", Surname = "Rodriguez", Specialty = "Italian Cuisine", Followers = 1250, Following = 340 },
                new { UserName = "healthy_john", Email = "john@mealplanner.demo", Name = "John", Surname = "Smith", Specialty = "Healthy Eating", Followers = 890, Following = 210 },
                new { UserName = "baker_sarah", Email = "sarah@mealplanner.demo", Name = "Sarah", Surname = "Johnson", Specialty = "Baking & Desserts", Followers = 2100, Following = 450 },
                new { UserName = "asian_fusion_mike", Email = "mike@mealplanner.demo", Name = "Mike", Surname = "Chen", Specialty = "Asian Fusion", Followers = 1560, Following = 380 },
                new { UserName = "vegan_emma", Email = "emma@mealplanner.demo", Name = "Emma", Surname = "Williams", Specialty = "Vegan Cooking", Followers = 3200, Following = 520 }
            };
            foreach (var userData in demoUserData)
            {
                var existingUser = await _identityUserRepository
                    .FindByNormalizedUserNameAsync(userData.UserName.ToUpperInvariant());

                if (existingUser != null)
                {
                    // Already seeded — just collect the profile
                    var profile = await _userProfileRepository.FindAsync(existingUser.Id);
                    if (profile != null) users.Add(profile);
                    continue;
                }

                // Build the UserProfile (which extends IdentityUser) with all properties SET
                var userId = _guidGenerator.Create();
                var user = new UserProfile(userId, userData.UserName, userData.Email)
                {
                    Name = userData.Name,
                    Surname = userData.Surname,
                    Specialty = userData.Specialty,
                    Followers = userData.Followers,
                    Following = userData.Following,
                    ProfileVisibility = VisibilityLevel.Public,
                    RecipesVisibility = VisibilityLevel.Public
                };

                var passwordBuilder = user.Name + ".123";

                var result = await _identityUserManager.CreateAsync(user, passwordBuilder);
                if (result.Succeeded)
                {
                    users.Add(user);
                    //await _identityUserManager.AddToRoleAsync(user, ExtendedRoleConsts.CommunityUser);
                }
                // If it failed (e.g. race condition on re-run), just skip — don't throw
            }

            return users;
        }

        private async Task<List<Recipe>> SeedRecipesAsync(List<UserProfile> users, Dictionary<string, Guid> lookup)
        {
            var recipes = new List<Recipe>();

            if (users.Count == 0) return recipes;

            // Recipe data with ratings and review counts for trending calculation
            var recipeData = new[]
            {
                // Chef Maria's Italian recipes (Top Rated)
                new { Name = "Classic Spaghetti Carbonara", Cuisine = "Italian", Difficulty = DifficultyLevel.Medium, PrepTime = 10, CookTime = 20, Servings = 4, Rating = 4.9, Reviews = 850, Description = "Authentic Roman carbonara with eggs, pecorino, and guanciale", UserIndex = 1 },
                new { Name = "Margherita Pizza", Cuisine = "Italian", Difficulty = DifficultyLevel.Easy, PrepTime = 90, CookTime = 12, Servings = 2, Rating = 4.8, Reviews = 1200, Description = "Classic Neapolitan pizza with fresh mozzarella and basil", UserIndex = 1 },
                new { Name = "Tiramisu", Cuisine = "Italian", Difficulty = DifficultyLevel.Medium, PrepTime = 30, CookTime = 0, Servings = 8, Rating = 4.7, Reviews = 650, Description = "Traditional Italian coffee-flavored dessert", UserIndex = 1 },
                
                // Healthy John's recipes (Trending)
                new { Name = "Quinoa Buddha Bowl", Cuisine = "Healthy", Difficulty = DifficultyLevel.Easy, PrepTime = 15, CookTime = 20, Servings = 2, Rating = 4.6, Reviews = 920, Description = "Nutritious bowl with quinoa, roasted vegetables, and tahini dressing", UserIndex = 2 },
                new { Name = "Grilled Salmon with Asparagus", Cuisine = "Healthy", Difficulty = DifficultyLevel.Easy, PrepTime = 10, CookTime = 15, Servings = 2, Rating = 4.7, Reviews = 780, Description = "Omega-3 rich salmon with fresh asparagus", UserIndex = 2 },
                new { Name = "Green Smoothie Bowl", Cuisine = "Healthy", Difficulty = DifficultyLevel.Easy, PrepTime = 10, CookTime = 0, Servings = 1, Rating = 4.5, Reviews = 540, Description = "Energizing breakfast bowl with spinach, banana, and berries", UserIndex = 2 },
                
                // Baker Sarah's recipes (Top Rated)
                new { Name = "Chocolate Chip Cookies", Cuisine = "American", Difficulty = DifficultyLevel.Easy, PrepTime = 15, CookTime = 12, Servings = 24, Rating = 4.9, Reviews = 1500, Description = "Perfectly chewy chocolate chip cookies", UserIndex = 3 },
                new { Name = "New York Cheesecake", Cuisine = "American", Difficulty = DifficultyLevel.Hard, PrepTime = 30, CookTime = 60, Servings = 12, Rating = 4.8, Reviews = 890, Description = "Rich and creamy classic cheesecake", UserIndex = 3 },
                new { Name = "Sourdough Bread", Cuisine = "Artisan", Difficulty = DifficultyLevel.Hard, PrepTime = 240, CookTime = 45, Servings = 1, Rating = 4.7, Reviews = 420, Description = "Homemade sourdough with crispy crust", UserIndex = 3 },
                
                // Mike's Asian Fusion recipes (Trending)
                new { Name = "Pad Thai", Cuisine = "Thai", Difficulty = DifficultyLevel.Medium, PrepTime = 20, CookTime = 15, Servings = 2, Rating = 4.6, Reviews = 1100, Description = "Classic Thai stir-fried noodles with tamarind sauce", UserIndex = 4 },
                new { Name = "Korean Bibimbap", Cuisine = "Korean", Difficulty = DifficultyLevel.Medium, PrepTime = 30, CookTime = 20, Servings = 4, Rating = 4.7, Reviews = 680, Description = "Mixed rice bowl with vegetables and gochujang", UserIndex = 4 },
                new { Name = "Japanese Ramen", Cuisine = "Japanese", Difficulty = DifficultyLevel.Hard, PrepTime = 60, CookTime = 180, Servings = 4, Rating = 4.8, Reviews = 950, Description = "Rich tonkotsu ramen with homemade broth", UserIndex = 4 },
                
                // Emma's Vegan recipes (Trending - High reviews)
                new { Name = "Vegan Lentil Curry", Cuisine = "Indian", Difficulty = DifficultyLevel.Easy, PrepTime = 15, CookTime = 30, Servings = 6, Rating = 4.7, Reviews = 1350, Description = "Creamy coconut lentil curry with aromatic spices", UserIndex = 5 },
                new { Name = "Chickpea Tacos", Cuisine = "Mexican", Difficulty = DifficultyLevel.Easy, PrepTime = 10, CookTime = 20, Servings = 4, Rating = 4.6, Reviews = 890, Description = "Spiced chickpeas in soft tortillas with avocado", UserIndex = 5 },
                new { Name = "Vegan Chocolate Cake", Cuisine = "Dessert", Difficulty = DifficultyLevel.Medium, PrepTime = 20, CookTime = 35, Servings = 10, Rating = 4.8, Reviews = 1120, Description = "Moist and rich chocolate cake without eggs or dairy", UserIndex = 5 }
            };

            foreach (var data in recipeData)
            {
                var authorId = data.UserIndex < users.Count ? users[data.UserIndex].Id : users[0].Id;

                var recipe = Recipe.CreateSeed(id: _guidGenerator.Create(),
                                               name: data.Name,
                                               cuisine: data.Cuisine,
                                               description: data.Description,
                                               servings: data.Servings,
                                               prepMinutes: data.PrepTime,
                                               cookMinutes: data.CookTime,
                                               authorId: authorId,
                                               difficulty: data.Difficulty,
                                               ingredients: GetSampleIngredients(data.Name, lookup),
                                               instructions: GetSampleInstructions(data.Name));

                recipe.Rating = data.Rating;
                recipe.ReviewCount = data.Reviews;
                recipe.ImageUrl = RecipeSeedImages.TryGet(data.Name);

                await _recipeRepository.InsertAsync(recipe, autoSave: true);
                recipes.Add(recipe);
            }

            return recipes;
        }

        private List<(string Name, float Grams, string Display, Guid? NutritionId)> GetSampleIngredients(string recipeName, Dictionary<string, Guid> lookup)
        {
            Guid? GetId(string name)
            {
                if (lookup.TryGetValue(name, out var id)) return id;
                if (name == "Eggs" && lookup.TryGetValue("Egg", out id)) return id;
                if (name == "Spaghetti" && lookup.TryGetValue("Pasta (Dry)", out id)) return id;
                if (name == "Rice Noodles" && lookup.TryGetValue("Pasta (Dry)", out id)) return id;
                if (name == "Mozzarella" && lookup.TryGetValue("Cheddar Cheese", out id)) return id;
                if (name == "Chickpeas" && lookup.TryGetValue("Black Beans (Cooked)", out id)) return id;
                if (name == "Asparagus" && lookup.TryGetValue("Broccoli", out id)) return id;
                if (name == "Quinoa" && lookup.TryGetValue("Brown Rice (Dry)", out id)) return id;
                if (name == "Kale" && lookup.TryGetValue("Spinach", out id)) return id;
                if (name == "Tahini" && lookup.TryGetValue("Peanut Butter", out id)) return id;
                if (name == "All-Purpose Flour" && lookup.TryGetValue("Oats (Dry)", out id)) return id;
                if (name == "Brown Sugar" && lookup.TryGetValue("Honey", out id)) return id;
                if (name == "Chocolate Chips" && lookup.TryGetValue("Almonds", out id)) return id;
                return null;
            }

            return recipeName switch
            {

                "Classic Spaghetti Carbonara" => new List<(string, float, string, Guid?)>
                {
                    ("Spaghetti", 400, "400g", GetId("Spaghetti")),
                    ("Eggs", 150, "3 large", GetId("Eggs")),
                    ("Pecorino Romano", 100, "100g grated", null),
                    ("Guanciale", 150, "150g diced", null),
                    ("Black Pepper", 5, "to taste", null),
                    ("Salt", 5, "for pasta water", null),
                },
                "Margherita Pizza" => new List<(string, float, string, Guid?)>
                {
                    ("Bread (White)", 300, "pizza dough", GetId("Bread (White)")),
                    ("Tomato", 400, "1 can crushed", GetId("Tomato")),
                    ("Mozzarella", 250, "250g fresh", GetId("Mozzarella")),
                    ("Olive Oil", 20, "2 tbsp", GetId("Olive Oil")),
                    ("Garlic", 5, "2 cloves minced", GetId("Garlic")),
                    ("Salt", 3, "to taste", null),
                },
                "Tiramisu" => new List<(string, float, string, Guid?)>
                {
                    ("Eggs", 100, "2 large yolks", GetId("Eggs")),
                    ("Cheddar Cheese", 250, "mascarpone substitute", GetId("Cheddar Cheese")),
                    ("Honey", 80, "1/3 cup sugar substitute", GetId("Honey")),
                    ("Bread (White)", 200, "ladyfinger substitute", GetId("Bread (White)")),
                    ("Whole Milk", 120, "1/2 cup brewed coffee", GetId("Whole Milk")),
                    ("Cocoa Powder", 10, "for dusting", null),
                },
                "Quinoa Buddha Bowl" => new List<(string, float, string, Guid?)>
                {
                    ("Quinoa", 180, "1 cup dry", GetId("Quinoa")),
                    ("Sweet Potato", 300, "1 large cubed", GetId("Sweet Potato")),
                    ("Chickpeas", 240, "1 can drained", GetId("Chickpeas")),
                    ("Kale", 80, "2 cups chopped", GetId("Kale")),
                    ("Tahini", 30, "2 tbsp dressing", GetId("Tahini")),
                    ("Olive Oil", 15, "1 tbsp", GetId("Olive Oil")),
                    ("Lemon Juice", 15, "1 tbsp", null),
                },
                "Grilled Salmon with Asparagus" => new List<(string, float, string, Guid?)>
                {
                    ("Salmon", 400, "2 fillets", GetId("Salmon")),
                    ("Asparagus", 250, "1 bunch", GetId("Asparagus")),
                    ("Olive Oil", 20, "2 tbsp", GetId("Olive Oil")),
                    ("Garlic", 5, "2 cloves", GetId("Garlic")),
                    ("Lemon Juice", 15, "1 tbsp", null),
                    ("Salt", 3, "to taste", null),
                    ("Black Pepper", 3, "to taste", null),
                },
                "Green Smoothie Bowl" => new List<(string, float, string, Guid?)>
                {
                    ("Spinach", 60, "2 cups", GetId("Spinach")),
                    ("Banana", 120, "1 frozen", GetId("Banana")),
                    ("Greek Yogurt", 150, "2/3 cup", GetId("Greek Yogurt")),
                    ("Apple", 100, "1 small", GetId("Apple")),
                    ("Honey", 15, "1 tbsp", GetId("Honey")),
                    ("Almonds", 20, "for topping", GetId("Almonds")),
                },
                "Chocolate Chip Cookies" => new List<(string, float, string, Guid?)>
                {
                    ("All-Purpose Flour", 280, "2 1/4 cups", GetId("All-Purpose Flour")),
                    ("Butter", 225, "1 cup softened", GetId("Butter")),
                    ("Brown Sugar", 200, "1 cup packed", GetId("Brown Sugar")),
                    ("Eggs", 100, "2 large", GetId("Eggs")),
                    ("Chocolate Chips", 340, "2 cups", GetId("Chocolate Chips")),
                    ("Vanilla Extract", 5, "1 tsp", null),
                    ("Baking Soda", 5, "1 tsp", null),
                    ("Salt", 5, "1/2 tsp", null),
                },
                "New York Cheesecake" => new List<(string, float, string, Guid?)>
                {
                    ("Cheddar Cheese", 900, "cream cheese substitute", GetId("Cheddar Cheese")),
                    ("Eggs", 150, "3 large", GetId("Eggs")),
                    ("Honey", 150, "3/4 cup sugar substitute", GetId("Honey")),
                    ("Butter", 120, "1/2 cup melted", GetId("Butter")),
                    ("Bread (White)", 200, "graham cracker substitute", GetId("Bread (White)")),
                    ("Whole Milk", 60, "1/4 cup sour cream substitute", GetId("Whole Milk")),
                    ("Vanilla Extract", 5, "1 tsp", null),
                },
                "Sourdough Bread" => new List<(string, float, string, Guid?)>
                {
                    ("Bread (White)", 500, "500g flour substitute", GetId("Bread (White)")),
                    ("Whole Milk", 350, "warm water substitute", GetId("Whole Milk")),
                    ("Salt", 10, "2 tsp", null),
                    ("Oats (Dry)", 100, "starter feed", GetId("Oats (Dry)")),
                },
                "Pad Thai" => new List<(string, float, string, Guid?)>
                {
                    ("Rice Noodles", 200, "200g flat", GetId("Rice Noodles")),
                    ("Egg", 100, "2 eggs", GetId("Egg")),
                    ("Tofu", 150, "150g firm", GetId("Tofu")),
                    ("Peanut Butter", 30, "2 tbsp sauce", GetId("Peanut Butter")),
                    ("Soy Sauce", 30, "2 tbsp", GetId("Soy Sauce")),
                    ("Bell Pepper", 100, "1 sliced", GetId("Bell Pepper")),
                    ("Garlic", 5, "2 cloves", GetId("Garlic")),
                    ("Lime Juice", 15, "1 tbsp", null),
                },
                "Korean Bibimbap" => new List<(string, float, string, Guid?)>
                {
                    ("White Rice (Dry)", 200, "1 cup dry", GetId("White Rice (Dry)")),
                    ("Ground Beef", 250, "250g", GetId("Ground Beef")),
                    ("Spinach", 100, "2 cups blanched", GetId("Spinach")),
                    ("Carrot", 80, "1 julienned", GetId("Carrot")),
                    ("Egg", 50, "1 fried", GetId("Egg")),
                    ("Soy Sauce", 30, "2 tbsp", GetId("Soy Sauce")),
                    ("Sesame Oil", 10, "2 tsp", null),
                    ("Gochujang", 20, "1 tbsp", null),
                },
                "Japanese Ramen" => new List<(string, float, string, Guid?)>
                {
                    ("Pasta (Dry)", 400, "4 servings noodles", GetId("Pasta (Dry)")),
                    ("Pork Chop", 300, "chashu substitute", GetId("Pork Chop")),
                    ("Egg", 100, "2 soft-boiled", GetId("Egg")),
                    ("Spinach", 80, "1 cup", GetId("Spinach")),
                    ("Mushrooms", 100, "1 cup sliced", GetId("Mushrooms")),
                    ("Soy Sauce", 40, "broth seasoning", GetId("Soy Sauce")),
                    ("Garlic", 10, "4 cloves", GetId("Garlic")),
                    ("Green Onion", 20, "2 stalks", null),
                },
                "Vegan Lentil Curry" => new List<(string, float, string, Guid?)>
                {
                    ("Lentils (Dry)", 200, "1 cup", GetId("Lentils (Dry)")),
                    ("Tomato", 400, "1 can", GetId("Tomato")),
                    ("Onion", 120, "1 large", GetId("Onion")),
                    ("Garlic", 10, "3 cloves", GetId("Garlic")),
                    ("Spinach", 100, "2 cups", GetId("Spinach")),
                    ("Olive Oil", 20, "2 tbsp", GetId("Olive Oil")),
                    ("Curry Powder", 10, "2 tbsp", null),
                    ("Coconut Milk", 200, "1 can", null),
                },
                "Chickpea Tacos" => new List<(string, float, string, Guid?)>
                {
                    ("Chickpeas", 240, "1 can drained", GetId("Chickpeas")),
                    ("Avocado", 150, "1 ripe", GetId("Avocado")),
                    ("Tomato", 150, "2 diced", GetId("Tomato")),
                    ("Lettuce", 80, "shredded", GetId("Lettuce")),
                    ("Bell Pepper", 100, "1 sliced", GetId("Bell Pepper")),
                    ("Olive Oil", 15, "1 tbsp", GetId("Olive Oil")),
                    ("Taco Seasoning", 10, "1 packet", null),
                    ("Corn Tortillas", 200, "8 small", null),
                },
                "Vegan Chocolate Cake" => new List<(string, float, string, Guid?)>
                {
                    ("Oats (Dry)", 250, "2 cups flour substitute", GetId("Oats (Dry)")),
                    ("Honey", 200, "1 cup sugar substitute", GetId("Honey")),
                    ("Olive Oil", 80, "1/3 cup", GetId("Olive Oil")),
                    ("Banana", 120, "2 mashed", GetId("Banana")),
                    ("Almonds", 50, "cocoa substitute", GetId("Almonds")),
                    ("Baking Powder", 10, "2 tsp", null),
                    ("Salt", 3, "pinch", null),
                    ("Vanilla Extract", 5, "1 tsp", null),
                },
                _ => new List<(string, float, string, Guid?)>
                {
                    ("Salt", 5, "to taste", null),
                    ("Black Pepper", 3, "to taste", null),
                    ("Olive Oil", 30, "2 tbsp", GetId("Olive Oil")),
                }
            };
        }

        private List<string> GetSampleInstructions(string recipeName)
        {
            return recipeName switch
            {
                "Classic Spaghetti Carbonara" => new List<string>
                {
                    "Bring a large pot of salted water to boil and cook spaghetti according to package directions",
                    "While pasta cooks, dice guanciale and cook in a large pan until crispy",
                    "Beat eggs with grated pecorino and black pepper in a bowl",
                    "Drain pasta, reserving 1 cup pasta water",
                    "Add hot pasta to the pan with guanciale, remove from heat",
                    "Quickly stir in egg mixture, adding pasta water to create a creamy sauce",
                    "Serve immediately with extra pecorino and black pepper"
                },
                "Margherita Pizza" => new List<string>
                {
                    "Stretch dough into two rounds on a floured surface.",
                    "Spread crushed tomato, garlic, and olive oil over each base.",
                    "Top with mozzarella and bake at 250°C (480°F) for 10-12 minutes until bubbly.",
                    "Finish with fresh basil and a drizzle of olive oil.",
                },
                "Tiramisu" => new List<string>
                {
                    "Whisk egg yolks with honey until pale; fold in mascarpone.",
                    "Briefly dip ladyfingers in coffee and layer in a dish.",
                    "Spread half the cream, repeat layers, and chill 4 hours.",
                    "Dust with cocoa before serving.",
                },
                "Quinoa Buddha Bowl" => new List<string>
                {
                     "Rinse quinoa and cook according to package directions",
                    "Cube sweet potato and roast at 400°F for 25 minutes",
                    "Drain and rinse chickpeas, season and roast for 20 minutes",
                    "Massage kale with a bit of olive oil and lemon juice",
                    "Assemble bowl with quinoa, roasted vegetables, chickpeas, and kale",
                    "Drizzle with tahini dressing and serve"
                },
                "Grilled Salmon with Asparagus" => new List<string>
                {
                    "Season salmon with salt, pepper, and lemon juice.",
                    "Toss asparagus with olive oil and garlic.",
                    "Grill salmon 4 minutes per side over medium-high heat.",
                    "Grill asparagus until tender-crisp, about 6 minutes.",
                    "Serve salmon over asparagus with lemon wedges.",
                },
                "Green Smoothie Bowl" => new List<string>
                {
                    "Blend spinach, banana, yogurt, apple, and honey until smooth.",
                    "Pour into a bowl and top with sliced fruit and almonds.",
                    "Serve immediately while cold.",
                },
                "Chocolate Chip Cookies" => new List<string>
                {
                    "Preheat oven to 375°F (190°C)",
                    "Cream together butter and sugars until fluffy",
                    "Beat in eggs one at a time",
                    "Mix in flour, baking soda, and salt",
                    "Fold in chocolate chips",
                    "Drop rounded tablespoons onto baking sheets",
                    "Bake for 10-12 minutes until golden brown",
                    "Cool on baking sheet for 5 minutes before transferring"
                },
                "New York Cheesecake" => new List<string>
                {
                    "Mix crushed crackers with melted butter; press into a springform pan.",
                    "Beat cream cheese, honey, eggs, and vanilla until smooth.",
                    "Pour over crust and bake in a water bath at 160°C (325°F) for 55 minutes.",
                    "Cool completely, then chill at least 4 hours before slicing.",
                },
                "Sourdough Bread" => new List<string>
                {
                    "Feed starter with flour and water; let rise until bubbly.",
                    "Mix dough, autolyse 30 minutes, then add salt.",
                    "Stretch and fold every 30 minutes for 2 hours.",
                    "Shape, proof overnight, then bake in a Dutch oven at 230°C (450°F) for 45 minutes.",
                },
                "Pad Thai" => new List<string>
                {
                    "Soak rice noodles until pliable; drain well.",
                    "Stir-fry tofu and garlic; push aside and scramble eggs in the pan.",
                    "Add noodles, soy sauce, and peanut butter; toss to coat.",
                    "Finish with bell pepper and lime juice; serve hot.",
                },
                "Korean Bibimbap" => new List<string>
                {
                    "Cook rice and keep warm.",
                    "Sauté seasoned ground beef until browned.",
                    "Blanch spinach and sauté carrot separately.",
                    "Fry an egg sunny-side up.",
                    "Arrange rice in bowls with vegetables, beef, egg, soy sauce, and gochujang.",
                },
                "Japanese Ramen" => new List<string>
                {
                    "Simmer broth with garlic, soy sauce, and mushrooms for 30 minutes.",
                    "Cook noodles separately until al dente.",
                    "Slice pork and soft-boil eggs.",
                    "Divide noodles into bowls, ladle hot broth, and top with pork, egg, spinach, and green onion.",
                },
                "Vegan Lentil Curry" => new List<string>
                {
                    "Sauté onion and garlic in olive oil until fragrant.",
                    "Stir in curry powder, then lentils, tomato, and coconut milk.",
                    "Simmer 25-30 minutes until lentils are tender.",
                    "Fold in spinach; season and serve with rice.",
                },
                "Chickpea Tacos" => new List<string>
                {
                    "Warm tortillas in a dry pan.",
                    "Sauté chickpeas with taco seasoning until heated through.",
                    "Fill tortillas with chickpeas, lettuce, tomato, and bell pepper.",
                    "Top with sliced avocado and serve.",
                },
                "Vegan Chocolate Cake" => new List<string>
                {
                    "Preheat oven to 180°C (350°F) and grease a cake pan.",
                    "Whisk dry ingredients; stir in mashed banana, oil, honey, and vanilla.",
                    "Bake 30-35 minutes until a toothpick comes out clean.",
                    "Cool completely before slicing.",
                },
                _ => new List<string>
                {
                    "Prepare and measure all ingredients.",
                    "Cook using the method suited to the dish.",
                    "Season with salt and pepper to taste.",
                    "Serve warm and enjoy.",
                }
            };
        }

        private async Task SeedMealPlansAsync(List<UserProfile> users, List<Recipe> recipes)
        {
            if (users.Count == 0 || recipes.Count == 0) return;

            var nextMonday = MealPlan.GetWeekStart(DateTime.UtcNow.AddDays(7));

            for (int i = 0; i < Math.Min(3, users.Count); i++)
            {
                var user = users[i];
                var userRecipes = recipes.Where(r => r.AuthorId == user.Id).Take(5).ToList();

                if (userRecipes.Count == 0)
                {
                    userRecipes = recipes.Take(5).ToList();
                }

                var mealPlan = new MealPlan(
                    id: _guidGenerator.Create(),
                    userId: user.Id,
                    weekStartDate: nextMonday
                );

                // Add some meal entries
                if (userRecipes.Count > 0)
                {
                    mealPlan.AddEntry(
                        id: _guidGenerator.Create(),
                        dayOfWeek: DayOfWeek.Monday,
                        mealName: "Monday Dinner",
                        mealType: MealType.Dinner,
                        recipeName: userRecipes[0].Name,
                        scheduledTime: "19:00",
                        recipeId: userRecipes[0].Id
                    );
                }

                if (userRecipes.Count > 1)
                {
                    mealPlan.AddEntry(
                        id: _guidGenerator.Create(),
                        dayOfWeek: DayOfWeek.Wednesday,
                        mealName: "Wednesday Lunch",
                        mealType: MealType.Lunch,
                        recipeName: userRecipes[1].Name,
                        scheduledTime: "12:30",
                        recipeId: userRecipes[1].Id
                    );
                }

                if (userRecipes.Count > 2)
                {
                    mealPlan.AddEntry(
                        id: _guidGenerator.Create(),
                        dayOfWeek: DayOfWeek.Friday,
                        mealName: "Friday Dinner",
                        mealType: MealType.Dinner,
                        recipeName: userRecipes[2].Name,
                        scheduledTime: "18:30",
                        recipeId: userRecipes[2].Id
                    );
                }

                await _mealPlanRepository.InsertAsync(mealPlan, autoSave: true);
            }

        }

        private async Task SeedShoppingListsAsync(List<UserProfile> users)
        {
            if (users.Count == 0) return;

            var shoppingListData = new[]
            {
                new { Name = "Weekly Groceries", Items = new[] {
                    ("Spaghetti", 1m, "pack", ShoppingItemCategory.Pantry, false),
                    ("Minced Beef", 500m, "g", ShoppingItemCategory.Meat, false),
                    ("Mixed Greens", 1m, "bag", ShoppingItemCategory.Vegetables, true),
                    ("Parmesan Cheese", 200m, "g", ShoppingItemCategory.Dairy, false)
                }},
                new { Name = "Healthy Week Prep", Items = new[] {
                    ("Quinoa", 500m, "g", ShoppingItemCategory.Pantry, false),
                    ("Salmon Fillets", 400m, "g", ShoppingItemCategory.Meat, false),
                    ("Asparagus", 1m, "bunch", ShoppingItemCategory.Vegetables, false),
                    ("Spinach", 200m, "g", ShoppingItemCategory.Vegetables, true)
                }},
                new { Name = "Baking Supplies", Items = new[] {
                    ("All-Purpose Flour", 1m, "kg", ShoppingItemCategory.Pantry, false),
                    ("Chocolate Chips", 300m, "g", ShoppingItemCategory.Pantry, false),
                    ("Butter", 250m, "g", ShoppingItemCategory.Dairy, true),
                    ("Eggs", 12m, "pcs", ShoppingItemCategory.Dairy, false)
                }}
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
                {
                    shoppingList.AddItem(
                        id: _guidGenerator.Create(),
                        name: name,
                        quantity: quantity,
                        isCompleted: isCompleted,
                        unit: unit,
                        category: category
                    );
                }

                await _shoppingListRepository.InsertAsync(shoppingList, autoSave: true);
            }
        }

        private async Task SeedNotificationsAsync(List<UserProfile> users)
        {
            if (users.Count == 0) return;

            var notificationData = new[]
            {
                new { Type = NotificationType.MealReminder, Title = "Upcoming Meal", Message = "Don't forget to prepare your planned meal for dinner today!" },
                new { Type = NotificationType.ShoppingListAlert, Title = "Shopping List Reminder", Message = "You have uncompleted items in your shopping list." },
                new { Type = NotificationType.RecipeLiked, Title = "Recipe Liked", Message = "Someone liked your recipe!" },
                new { Type = NotificationType.NewFollower, Title = "New Follower", Message = "You have a new follower!" }
            };

            // Add notifications for first 2 users
            for (int i = 0; i < Math.Min(2, users.Count); i++)
            {
                foreach (var data in notificationData.Take(2))
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
}
