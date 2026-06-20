using MealPlannerAPI.DataSeeder.Helpers;
using static MealPlannerAPI.DataSeeder.Helpers.SeedDemoUserDefs;
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

        public int Order => 4;

        public NutritionRecipeDataSeedContributor(
            IIngredientNutritionRepository ingredientNutritionRepository,
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
            // Idempotency guard — use the first recipe in the list as sentinel.
            if (await _recipeRepository.AnyAsync(r => r.Name == RecipeBuilders.All[0].Name)) return;

            var lookup = await BuildLookupAsync();
            var admin = await ResolveAdminAsync();
            var demoAuthors = await ResolveDemoAuthorsAsync();

            for (int i = 0; i < RecipeBuilders.All.Count; i++)
            {
                var def = RecipeBuilders.All[i];

                var exists = await _recipeRepository.AnyAsync(r => r.Name == def.Name);
                if (exists) continue;

                var (authorId, authorUsername) = PickAuthor(i, admin, demoAuthors);
                var recipe = RecipeBuilders.Build(def, lookup, _guidGenerator, authorId, authorUsername);
                await _recipeRepository.InsertAsync(recipe, autoSave: true);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private async Task<Dictionary<string, Guid>> BuildLookupAsync()
        {
            var all = await _ingredientNutritionRepository.GetListAsync();
            return all.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);
        }

        private async Task<(Guid Id, string UserName)?> ResolveAdminAsync()
        {
            var admin = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");
            return admin is null ? null : (admin.Id, admin.UserName);
        }

        private async Task<List<(Guid Id, string UserName)>> ResolveDemoAuthorsAsync()
        {
            var authors = new List<(Guid Id, string UserName)>();
            foreach (var def in SeedDemoUserDefs.All)
            {
                var user = await _identityUserRepository.FindByNormalizedUserNameAsync(def.UserName.ToUpperInvariant());
                if (user != null) authors.Add((user.Id, user.UserName));
            }
            return authors;
        }

        /// <summary>Every fifth recipe is authored by admin (when available); others round-robin demo users.</summary>
        private static (Guid? Id, string? UserName) PickAuthor(
            int index,
            (Guid Id, string UserName)? admin,
            IReadOnlyList<(Guid Id, string UserName)> demoAuthors)
        {
            if (admin.HasValue && index % 5 == 0) return (admin.Value.Id, admin.Value.UserName);
            if (demoAuthors.Count > 0)
            {
                var a = demoAuthors[index % demoAuthors.Count];
                return (a.Id, a.UserName);
            }
            return admin.HasValue ? (admin.Value.Id, admin.Value.UserName) : (null, null);
        }
    }
}