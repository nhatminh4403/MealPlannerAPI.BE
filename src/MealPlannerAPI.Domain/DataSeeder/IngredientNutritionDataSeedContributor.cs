using MealPlannerAPI.Nutritions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace MealPlannerAPI.DataSeeder
{
    public class IngredientNutritionDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        public int Order => 2;
        private readonly IIngredientNutritionRepository _ingredientNutritionRepository;
        private readonly IGuidGenerator _guidGenerator;
        public IngredientNutritionDataSeedContributor(IIngredientNutritionRepository ingredientNutritionRepository, IGuidGenerator guidGenerator)
        {
            _ingredientNutritionRepository = ingredientNutritionRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _ingredientNutritionRepository.GetCountAsync() > 0) return;
            await SeedIngredientNutritionsAsync();
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
    }
}
