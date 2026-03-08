using MealPlannerAPI.ShoppingLists.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.ShoppingLists.Services
{
    public interface IShoppingListAppService
        : ICrudAppService<
            ShoppingListDto,
            ShoppingListDto,
            Guid,
            GetShoppingListsInput,
            CreateUpdateShoppingListDto,
            CreateUpdateShoppingListDto>
    {
        /// <summary>Add a single item to an existing shopping list.</summary>
        Task<ShoppingListItemDto> AddItemAsync(Guid shoppingListId, CreateUpdateShoppingListItemDto input);

        /// <summary>Update a single item within a shopping list.</summary>
        Task<ShoppingListItemDto> UpdateItemAsync(Guid shoppingListId, Guid itemId, CreateUpdateShoppingListItemDto input);

        /// <summary>Remove a single item from a shopping list.</summary>
        Task RemoveItemAsync(Guid shoppingListId, Guid itemId);

        /// <summary>Toggle the completed state of a single item.</summary>
        Task<ShoppingListItemDto> ToggleItemAsync(Guid shoppingListId, Guid itemId);

        /// <summary>Mark all items in a shopping list as completed.</summary>
        Task MarkAllCompletedAsync(Guid shoppingListId);

        /// <summary>Generate a shopping list from a given meal plan.</summary>
        Task<ShoppingListDto> GenerateFromMealPlanAsync(Guid mealPlanId);
    }
}
