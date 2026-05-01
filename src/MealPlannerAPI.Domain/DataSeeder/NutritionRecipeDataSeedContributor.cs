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
        private readonly IIngredientNutritionRepository _ingredientNutritionRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IIdentityUserRepository _identityUserRepository;

        public int Order => 3;
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

            var lookup = await BuildLookupAsync();
            await SeedSampleRecipesAsync(lookup);
        }
        private async Task<Dictionary<string, Guid>> BuildLookupAsync()
        {
            var all = await _ingredientNutritionRepository.GetListAsync();
            return all.ToDictionary(x => x.Name, x => x.Id);
        }
        private async Task SeedSampleRecipesAsync(Dictionary<string, Guid> lookup)
        {
            var adminId = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");

            var recipes = new[]
            {
                RecipeBuilders.BuildGrilledChicken(lookup,_guidGenerator,adminId.Id),
                RecipeBuilders.BuildSalmonRice(lookup,_guidGenerator),
                RecipeBuilders.BuildPastaPomodoro(lookup,_guidGenerator),
                RecipeBuilders.         BuildSpinachOmelette(lookup,_guidGenerator),
                RecipeBuilders.      BuildSweetPotatoBowl(lookup,_guidGenerator),
                RecipeBuilders.       BuildChickenFriedRice(lookup,_guidGenerator),
                RecipeBuilders.  BuildBananaOatmeal(lookup,_guidGenerator),
                RecipeBuilders.     BuildBeefAndPotatoBake(lookup,_guidGenerator),
                RecipeBuilders.    BuildYogurtParfait(lookup,_guidGenerator),
                RecipeBuilders.    BuildPeanutButterToast(lookup,_guidGenerator),
                RecipeBuilders.    BuildBeefStirFry(lookup,_guidGenerator),
                RecipeBuilders.    BuildChickenSalad(lookup,_guidGenerator),
                RecipeBuilders.   BuildAvocadoToast(lookup,_guidGenerator),
                RecipeBuilders.   BuildMushroomOmelette(lookup,_guidGenerator),
                RecipeBuilders.   BuildTofuStirFry(lookup,_guidGenerator),
                RecipeBuilders.   BuildPorkChopSweetPotato(lookup,_guidGenerator),
                RecipeBuilders.  BuildLentilSoup(lookup,_guidGenerator),
                RecipeBuilders.    BuildGreekSalad(lookup,_guidGenerator),
                RecipeBuilders.  BuildAppleAlmondSnack(lookup,_guidGenerator),
                RecipeBuilders.   BuildChickenRiceBowl(lookup,_guidGenerator),
                RecipeBuilders.   BuildBeefBurgerSalad(lookup,_guidGenerator),
                RecipeBuilders.  BuildPeanutButterAppleToast(lookup,_guidGenerator),
                RecipeBuilders.   BuildVeggieFriedRice(lookup,_guidGenerator),
                RecipeBuilders.  BuildMushroomPasta(lookup,_guidGenerator),
                RecipeBuilders.   BuildSpicyTofuScramble(lookup,_guidGenerator),
                RecipeBuilders.   BuildGarlicButterPorkChops(lookup,_guidGenerator),
                RecipeBuilders.   BuildBreakfastBurritoBowl(lookup,_guidGenerator),
                RecipeBuilders.   BuildChickenMushroomSaute(lookup,_guidGenerator),
                RecipeBuilders.   BuildCarrotLentilMash(lookup,_guidGenerator),
                RecipeBuilders.  BuildYogurtAlmonds(lookup,_guidGenerator),
            };

            foreach (var recipe in recipes)
            {
                var exists = await _recipeRepository.AnyAsync(r => r.Name == recipe.Name);
                if (!exists)
                    await _recipeRepository.InsertAsync(recipe, autoSave: true);
            }
        }


    }
}
