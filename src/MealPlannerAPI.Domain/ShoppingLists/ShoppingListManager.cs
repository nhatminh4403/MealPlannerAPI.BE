using MealPlannerAPI.Enums;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Recipes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Domain.Services;

namespace MealPlannerAPI.ShoppingLists
{
    public class ShoppingListManager : DomainService
    {
        private readonly IShoppingListRepository? _shoppingListRepository;

        public ShoppingListManager(IShoppingListRepository? shoppingListRepository)
        {
            _shoppingListRepository = shoppingListRepository;
        }


        public ShoppingList CreateFromMealPlan(MealPlan mealPlan, IEnumerable<Recipe> recipes, Guid userId)
        {
            var name = $"Shopping List for Week of {mealPlan.WeekStartDate:yyyy-MM-dd}";

            var shoppingList = new ShoppingList(GuidGenerator.Create(), userId, name);

            var recipeMap = recipes.ToDictionary(r => r.Id);

            // Get recipe IDs referenced in the meal plan
            var recipeIds = mealPlan.Entries
                .Where(e => e.RecipeId.HasValue)
                .Select(e => e.RecipeId!.Value)
                .Distinct();

            var aggregatedIngredients = recipeIds
                .Where(recipeMap.ContainsKey)
                .SelectMany(id => recipeMap[id].Ingredients)
                .GroupBy(i => i.Name.Trim().ToLowerInvariant())
                .Select(g => new
                {
                    Id = GuidGenerator.Create(),
                    Name = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    Unit = g.First().Unit,
                    Category = ShoppingItemCategory.Other,
                });

            foreach (var item in aggregatedIngredients)
            {
                var itemId = GuidGenerator.Create();
                shoppingList.AddItem(itemId, item.Name, item.Quantity, false, item.Unit, item.Category);
            }

            return shoppingList;
        }
    }
}
