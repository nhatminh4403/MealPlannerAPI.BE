using MealPlannerAPI.DataSeeder.Helpers;
using MealPlannerAPI.Nutritions;
using MealPlannerAPI.Recipes;
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
    public class NutritionRecipeDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private static readonly string[] DemoUserNames =
        {
            "chef_maria",
            "healthy_john",
            "baker_sarah",
            "asian_fusion_mike",
            "vegan_emma",
        };

        private readonly IIngredientNutritionRepository _ingredientNutritionRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IIdentityUserRepository _identityUserRepository;

        public int Order => 4;
        public NutritionRecipeDataSeedContributor(IIngredientNutritionRepository ingredientNutritionRepository,
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
            if (await _recipeRepository.AnyAsync(r => r.Name == "Grilled Chicken with Broccoli")) return;

            // Try to get lookup from context (passed from IngredientNutritionDataSeedContributor)
            var lookup = context.Properties.TryGetValue("IngredientNutritionLookup", out var lookupObj) 
                ? lookupObj as Dictionary<string, Guid> 
                : null;

            // If not available in context, build it from repository
            lookup ??= await BuildLookupAsync();
            
            await SeedSampleRecipesAsync(lookup);
        }
        private async Task<Dictionary<string, Guid>> BuildLookupAsync()
        {
            var all = await _ingredientNutritionRepository.GetListAsync();
            return all.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
        }

        private async Task<Guid?> ResolveAdminIdAsync()
        {
            var admin = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");
            return admin?.Id;
        }

        private async Task<List<Guid>> ResolveDemoAuthorIdsAsync()
        {
            var authorIds = new List<Guid>();
            foreach (var userName in DemoUserNames)
            {
                var user = await _identityUserRepository.FindByNormalizedUserNameAsync(userName.ToUpperInvariant());
                if (user != null)
                {
                    authorIds.Add(user.Id);
                }
            }

            return authorIds;
        }

        private static Guid? AuthorAt(IReadOnlyList<Guid> demoIds, int index)
        {
            if (demoIds.Count == 0) return null;
            return demoIds[index % demoIds.Count];
        }

        /// <summary>Every fifth nutrition recipe is authored by admin when available.</summary>
        private static Guid? AuthorFor(int index, Guid? adminId, IReadOnlyList<Guid> demoIds)
        {
            if (adminId.HasValue && index % 5 == 0)
            {
                return adminId;
            }

            var author = AuthorAt(demoIds, index);
            return author ?? adminId;
        }

        private async Task SeedSampleRecipesAsync(Dictionary<string, Guid> lookup)
        {
            var adminId = await ResolveAdminIdAsync();
            var demoIds = await ResolveDemoAuthorIdsAsync();

            var recipes = new[]
            {
                RecipeBuilders.BuildGrilledChicken(lookup, _guidGenerator, AuthorFor(0, adminId, demoIds)),
                RecipeBuilders.BuildSalmonRice(lookup, _guidGenerator, AuthorFor(1, adminId, demoIds)),
                RecipeBuilders.BuildPastaPomodoro(lookup, _guidGenerator, AuthorFor(2, adminId, demoIds)),
                RecipeBuilders.BuildSpinachOmelette(lookup, _guidGenerator, AuthorFor(3, adminId, demoIds)),
                RecipeBuilders.BuildSweetPotatoBowl(lookup, _guidGenerator, AuthorFor(4, adminId, demoIds)),
                RecipeBuilders.BuildChickenFriedRice(lookup, _guidGenerator, AuthorFor(5, adminId, demoIds)),
                RecipeBuilders.BuildBananaOatmeal(lookup, _guidGenerator, AuthorFor(6, adminId, demoIds)),
                RecipeBuilders.BuildBeefAndPotatoBake(lookup, _guidGenerator, AuthorFor(7, adminId, demoIds)),
                RecipeBuilders.BuildYogurtParfait(lookup, _guidGenerator, AuthorFor(8, adminId, demoIds)),
                RecipeBuilders.BuildPeanutButterToast(lookup, _guidGenerator, AuthorFor(9, adminId, demoIds)),
                RecipeBuilders.BuildBeefStirFry(lookup, _guidGenerator, AuthorFor(10, adminId, demoIds)),
                RecipeBuilders.BuildChickenSalad(lookup, _guidGenerator, AuthorFor(11, adminId, demoIds)),
                RecipeBuilders.BuildAvocadoToast(lookup, _guidGenerator, AuthorFor(12, adminId, demoIds)),
                RecipeBuilders.BuildMushroomOmelette(lookup, _guidGenerator, AuthorFor(13, adminId, demoIds)),
                RecipeBuilders.BuildTofuStirFry(lookup, _guidGenerator, AuthorFor(14, adminId, demoIds)),
                RecipeBuilders.BuildPorkChopSweetPotato(lookup, _guidGenerator, AuthorFor(15, adminId, demoIds)),
                RecipeBuilders.BuildLentilSoup(lookup, _guidGenerator, AuthorFor(16, adminId, demoIds)),
                RecipeBuilders.BuildGreekSalad(lookup, _guidGenerator, AuthorFor(17, adminId, demoIds)),
                RecipeBuilders.BuildAppleAlmondSnack(lookup, _guidGenerator, AuthorFor(18, adminId, demoIds)),
                RecipeBuilders.BuildChickenRiceBowl(lookup, _guidGenerator, AuthorFor(19, adminId, demoIds)),
                RecipeBuilders.BuildBeefBurgerSalad(lookup, _guidGenerator, AuthorFor(20, adminId, demoIds)),
                RecipeBuilders.BuildPeanutButterAppleToast(lookup, _guidGenerator, AuthorFor(21, adminId, demoIds)),
                RecipeBuilders.BuildVeggieFriedRice(lookup, _guidGenerator, AuthorFor(22, adminId, demoIds)),
                RecipeBuilders.BuildMushroomPasta(lookup, _guidGenerator, AuthorFor(23, adminId, demoIds)),
                RecipeBuilders.BuildSpicyTofuScramble(lookup, _guidGenerator, AuthorFor(24, adminId, demoIds)),
                RecipeBuilders.BuildGarlicButterPorkChops(lookup, _guidGenerator, AuthorFor(25, adminId, demoIds)),
                RecipeBuilders.BuildBreakfastBurritoBowl(lookup, _guidGenerator, AuthorFor(26, adminId, demoIds)),
                RecipeBuilders.BuildChickenMushroomSaute(lookup, _guidGenerator, AuthorFor(27, adminId, demoIds)),
                RecipeBuilders.BuildCarrotLentilMash(lookup, _guidGenerator, AuthorFor(28, adminId, demoIds)),
                RecipeBuilders.BuildYogurtAlmonds(lookup, _guidGenerator, AuthorFor(29, adminId, demoIds)),
            };

            foreach (var recipe in recipes)
            {
                recipe.ImageUrl = RecipeSeedImages.TryGet(recipe.Name);

                var exists = await _recipeRepository.AnyAsync(r => r.Name == recipe.Name);
                if (!exists)
                    await _recipeRepository.InsertAsync(recipe, autoSave: true);
            }
        }


    }
}
