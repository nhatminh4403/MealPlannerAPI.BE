using MealPlannerAPI.Enums;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
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
        
        public int Order => 3;

        public MealPlannerAPIDataSeedContributor(
            IMealPlanRepository mealPlanRepository,
            IShoppingListRepository shoppingListRepository,
            IRecipeRepository recipeRepository,
            IUserNotificationRepository notificationRepository,
            IRepository<UserProfile, Guid> userProfileRepository,
            IIdentityUserRepository identityUserRepository,
            IGuidGenerator guidGenerator,
            IdentityUserManager identityUserManager)
        {
            _mealPlanRepository = mealPlanRepository;
            _shoppingListRepository = shoppingListRepository;
            _recipeRepository = recipeRepository;
            _notificationRepository = notificationRepository;
            _userProfileRepository = userProfileRepository;
            _identityUserRepository = identityUserRepository;
            _guidGenerator = guidGenerator;
            _identityUserManager = identityUserManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Check if data already exists
            // Use a demo user as the idempotency sentinel
            var alreadySeeded = await _identityUserRepository
                .FindByNormalizedUserNameAsync("CHEF_MARIA");
            if (alreadySeeded != null) return;

            var demoUsers = await SeedDemoUsersAsync();

            // 2. Seed Recipes for each user
            var allRecipes = await SeedRecipesAsync(demoUsers);

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

        private async Task<List<Recipe>> SeedRecipesAsync(List<UserProfile> users)
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
                
                var recipe = Recipe.CreateSeed(
                    id: _guidGenerator.Create(),
                    name: data.Name,
                    cuisine: data.Cuisine,
                    description: data.Description,
                    servings: data.Servings,
                    prepMinutes: data.PrepTime,
                    cookMinutes: data.CookTime,
                    authorId: authorId,
                    difficulty: data.Difficulty,
                    ingredients: GetSampleIngredients(data.Name),
                    instructions: GetSampleInstructions(data.Name)
                );

                recipe.Rating = data.Rating;
                recipe.ReviewCount = data.Reviews;

                await _recipeRepository.InsertAsync(recipe, autoSave: true);
                recipes.Add(recipe);
            }

            return recipes;
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

        private List<(string Name, float Grams, string Display, Guid? NutritionId)> GetSampleIngredients(string recipeName)
        {
            // Return sample ingredients based on recipe type
            return recipeName switch
            {
                "Classic Spaghetti Carbonara" => new List<(string, float, string, Guid?)>
                {
                    ("Spaghetti", 400, "400g", null),
                    ("Eggs", 150, "3 large", null),
                    ("Pecorino Romano", 100, "100g grated", null),
                    ("Guanciale", 150, "150g diced", null),
                    ("Black Pepper", 5, "to taste", null)
                },
                "Quinoa Buddha Bowl" => new List<(string, float, string, Guid?)>
                {
                    ("Quinoa", 200, "1 cup", null),
                    ("Sweet Potato", 300, "1 large", null),
                    ("Chickpeas", 240, "1 can", null),
                    ("Kale", 100, "2 cups", null),
                    ("Tahini", 30, "2 tbsp", null)
                },
                "Chocolate Chip Cookies" => new List<(string, float, string, Guid?)>
                {
                    ("All-Purpose Flour", 280, "2 1/4 cups", null),
                    ("Butter", 225, "1 cup", null),
                    ("Brown Sugar", 200, "1 cup", null),
                    ("Chocolate Chips", 340, "2 cups", null),
                    ("Eggs", 100, "2 large", null)
                },
                _ => new List<(string, float, string, Guid?)>
                {
                    ("Main Ingredient", 500, "500g", null),
                    ("Seasoning", 10, "to taste", null),
                    ("Oil", 30, "2 tbsp", null)
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
                "Quinoa Buddha Bowl" => new List<string>
                {
                    "Rinse quinoa and cook according to package directions",
                    "Cube sweet potato and roast at 400°F for 25 minutes",
                    "Drain and rinse chickpeas, season and roast for 20 minutes",
                    "Massage kale with a bit of olive oil and lemon juice",
                    "Assemble bowl with quinoa, roasted vegetables, chickpeas, and kale",
                    "Drizzle with tahini dressing and serve"
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
                _ => new List<string>
                {
                    "Prepare all ingredients",
                    "Follow cooking method appropriate for the dish",
                    "Season to taste",
                    "Serve and enjoy"
                }
            };
        }
    }
}
