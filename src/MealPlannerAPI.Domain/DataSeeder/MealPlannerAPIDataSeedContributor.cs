using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.ShoppingLists;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealPlannerAPI.Enums;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.DataSeeder
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

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _mealPlanRepository.GetCountAsync() > 0)
            {
                return;
            }

            var userId = Guid.NewGuid();

            // 1. Get Existing Recipes to use for Meal Plan
            var existingRecipes = await _recipeRepository.GetListAsync(skipCount: 0, maxResultCount: 2);
            Guid? recipe1Id = existingRecipes.Count > 0 ? existingRecipes[0].Id : null;
            Guid? recipe2Id = existingRecipes.Count > 1 ? existingRecipes[1].Id : null;

            // 2. Seed Meal Plan
            var mealPlanId = Guid.NewGuid();
            var nextMonday = MealPlan.GetWeekStart(DateTime.UtcNow.AddDays(7));
            var mealPlan = new MealPlan(id: mealPlanId, userId: userId, weekStartDate: nextMonday);

            mealPlan.AddEntry(
                id: Guid.NewGuid(),
                dayOfWeek: DayOfWeek.Monday,
                mealName: "Monday Dinner",
                mealType: MealType.Dinner,
                scheduledTime: "19:00",
                recipeId: recipe1Id);

            mealPlan.AddEntry(
                id: Guid.NewGuid(),
                dayOfWeek: DayOfWeek.Tuesday,
                mealName: "Tuesday Lunch",
                mealType: MealType.Lunch,
                scheduledTime: "12:30",
                recipeId: recipe2Id);

            await _mealPlanRepository.InsertAsync(mealPlan, autoSave: true);

            // 3. Seed Shopping List
            if (await _shoppingListRepository.GetCountAsync() == 0)
            {
                var shoppingListId = Guid.NewGuid();
                var shoppingList = new ShoppingList(id: shoppingListId, userId: userId, name: "Weekly Groceries");

                shoppingList.AddItem(
                    id: Guid.NewGuid(),
                    name: "Spaghetti",
                    quantity: 1,
                    isCompleted: false,
                    unit: "pack",
                    category: ShoppingItemCategory.Pantry);

                shoppingList.AddItem(
                    id: Guid.NewGuid(),
                    name: "Minced Beef",
                    quantity: 500,
                    isCompleted: false,
                    unit: "g",
                    category: ShoppingItemCategory.Meat);

                shoppingList.AddItem(
                    id: Guid.NewGuid(),
                    name: "Mixed Greens",
                    quantity: 1,
                    isCompleted: true,
                    unit: "bag",
                    category: ShoppingItemCategory.Vegetables);

                await _shoppingListRepository.InsertAsync(shoppingList, autoSave: true);
            }

            // 4. Seed User Notifications
            if (await _notificationRepository.GetCountAsync() == 0)
            {
                var notification1 = new UserNotification(
                    id: Guid.NewGuid(),
                    userId: userId,
                    type: NotificationType.MealReminder,
                    title: "Upcoming Meal",
                    message: "Don't forget to prepare your planned meal for dinner today!",
                    avatarUrl: null);

                var notification2 = new UserNotification(
                    id: Guid.NewGuid(),
                    userId: userId,
                    type: NotificationType.ShoppingListAlert,
                    title: "Shopping List Reminder",
                    message: "You have uncompleted items in your Weekly Groceries list.",
                    avatarUrl: null);

                await _notificationRepository.InsertAsync(notification1, autoSave: true);
                await _notificationRepository.InsertAsync(notification2, autoSave: true);
            }
        }
    }
}
