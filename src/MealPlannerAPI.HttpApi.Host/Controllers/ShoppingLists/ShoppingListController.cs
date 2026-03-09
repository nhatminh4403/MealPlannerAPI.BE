using MealPlannerAPI.ShoppingLists.Dtos;
using MealPlannerAPI.ShoppingLists.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers.ShoppingLists
{
    public class ShoppingListController : AbpControllerBase
    {
        private readonly IShoppingListAppService _shoppingListAppService;

        public ShoppingListController(IShoppingListAppService shoppingListAppService)
        {
            _shoppingListAppService = shoppingListAppService;
        }

        /// <summary>Get a shopping list by ID including all items.</summary>
        [HttpGet("{id:guid}")]
        public Task<ShoppingListDto> GetAsync(Guid id)
            => _shoppingListAppService.GetAsync(id);

        /// <summary>Get the current user's shopping lists.</summary>
        [HttpGet]
        public Task<PagedResultDto<ShoppingListDto>> GetListAsync([FromQuery] GetShoppingListsInput input)
            => _shoppingListAppService.GetListAsync(input);

        /// <summary>Create a new shopping list, optionally with an initial set of items.</summary>
        [HttpPost]
        public Task<ShoppingListDto> CreateAsync([FromBody] CreateUpdateShoppingListDto input)
            => _shoppingListAppService.CreateAsync(input);

        /// <summary>Replace the name and all items on an existing shopping list.</summary>
        [HttpPut("{id:guid}")]
        public Task<ShoppingListDto> UpdateAsync(Guid id, [FromBody] CreateUpdateShoppingListDto input)
            => _shoppingListAppService.UpdateAsync(id, input);

        /// <summary>Delete a shopping list and all its items.</summary>
        [HttpDelete("{id:guid}")]
        public Task DeleteAsync(Guid id)
            => _shoppingListAppService.DeleteAsync(id);

        /// <summary>Add a single item to an existing shopping list.</summary>
        [HttpPost("{id:guid}/items")]
        public Task<ShoppingListItemDto> AddItemAsync(Guid id, [FromBody] CreateUpdateShoppingListItemDto input)
            => _shoppingListAppService.AddItemAsync(id, input);

        /// <summary>Update a single item on a shopping list.</summary>
        [HttpPut("{id:guid}/items/{itemId:guid}")]
        public Task<ShoppingListItemDto> UpdateItemAsync(Guid id, Guid itemId, [FromBody] CreateUpdateShoppingListItemDto input)
            => _shoppingListAppService.UpdateItemAsync(id, itemId, input);

        /// <summary>Remove a single item from a shopping list.</summary>
        [HttpDelete("{id:guid}/items/{itemId:guid}")]
        public Task RemoveItemAsync(Guid id, Guid itemId)
            => _shoppingListAppService.RemoveItemAsync(id, itemId);

        /// <summary>Toggle the completed state of a single item.</summary>
        [HttpPatch("{id:guid}/items/{itemId:guid}/toggle")]
        public Task<ShoppingListItemDto> ToggleItemAsync(Guid id, Guid itemId)
            => _shoppingListAppService.ToggleItemAsync(id, itemId);

        /// <summary>Mark every item on the list as completed.</summary>
        [HttpPatch("{id:guid}/complete-all")]
        public Task MarkAllCompletedAsync(Guid id)
            => _shoppingListAppService.MarkAllCompletedAsync(id);

        /// <summary>
        /// Generate a shopping list from a meal plan.
        /// Aggregates all recipe ingredients across every planned meal for the week,
        /// merging duplicates by summing quantities.
        /// </summary>
        [HttpPost("from-meal-plan/{mealPlanId:guid}")]
        public Task<ShoppingListDto> GenerateFromMealPlanAsync(Guid mealPlanId)
            => _shoppingListAppService.GenerateFromMealPlanAsync(mealPlanId);
    }
}
