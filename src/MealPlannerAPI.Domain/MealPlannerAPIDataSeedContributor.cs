using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.ShoppingLists;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI
{
    public class MealPlannerAPIDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserNotificationRepository _notificationRepository;

        public MealPlannerAPIDataSeedContributor(IMealPlanRepository mealPlanRepository,
                                                 IShoppingListRepository shoppingListRepository,
                                                 IRecipeRepository recipeRepository,
                                                 IUserNotificationRepository notificationRepository)
        {
            _mealPlanRepository = mealPlanRepository;
            _shoppingListRepository = shoppingListRepository;
            _recipeRepository = recipeRepository;
            _notificationRepository = notificationRepository;
        }

        public Task SeedAsync(DataSeedContext context)
        {
            throw new NotImplementedException();
        }
    }
}
