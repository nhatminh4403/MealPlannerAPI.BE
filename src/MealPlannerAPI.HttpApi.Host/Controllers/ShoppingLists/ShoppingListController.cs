using MealPlannerAPI.Models;
using MealPlannerAPI.Routes;
using MealPlannerAPI.ShoppingLists.Dtos;
using MealPlannerAPI.ShoppingLists.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;

namespace MealPlannerAPI.Controllers.ShoppingLists
{
    [ApiController]
    //[Authorize]
    [Route(APIRoute.APIApp + "shopping-lists")]
    public class ShoppingListController : AbpControllerBase
    {
        private readonly IShoppingListAppService _shoppingListAppService;

        public ShoppingListController(IShoppingListAppService shoppingListAppService)
        {
            _shoppingListAppService = shoppingListAppService;
        }

        /// <summary>Get a shopping list by ID including all items.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShoppingListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            try
            {
                var result = await _shoppingListAppService.GetAsync(id);
                return Ok(new ApiResponse<ShoppingListDto>(true, "Success", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get the current user's shopping lists.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<ShoppingListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListAsync([FromQuery] GetShoppingListsInput input)
        {
            try
            {
                var result = await _shoppingListAppService.GetListAsync(input);
                return Ok(new ApiResponse<PagedResultDto<ShoppingListDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Create a new shopping list, optionally with an initial set of items.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ShoppingListDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateShoppingListDto input)
        {
            try
            {
                var result = await _shoppingListAppService.CreateAsync(input);
                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<ShoppingListDto>(true, "Shopping list created successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Replace the name and all items on an existing shopping list.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShoppingListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateUpdateShoppingListDto input)
        {
            try
            {
                var result = await _shoppingListAppService.UpdateAsync(id, input);
                return Ok(new ApiResponse<ShoppingListDto>(true, "Shopping list updated successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Delete a shopping list and all its items.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _shoppingListAppService.DeleteAsync(id);
                return Ok(new ApiResponse(true, "Shopping list deleted successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Add a single item to an existing shopping list.</summary>
        [HttpPost("{id:guid}/items")]
        [ProducesResponseType(typeof(ApiResponse<ShoppingListItemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddItemAsync(Guid id, [FromBody] CreateUpdateShoppingListItemDto input)
        {
            try
            {
                var result = await _shoppingListAppService.AddItemAsync(id, input);
                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<ShoppingListItemDto>(true, "Item added successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Update a single item on a shopping list.</summary>
        [HttpPut("{id:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShoppingListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItemAsync(Guid id, Guid itemId, [FromBody] CreateUpdateShoppingListItemDto input)
        {
            try
            {
                var result = await _shoppingListAppService.UpdateItemAsync(id, itemId, input);
                return Ok(new ApiResponse<ShoppingListItemDto>(true, "Item updated successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list item not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Remove a single item from a shopping list.</summary>
        [HttpDelete("{id:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveItemAsync(Guid id, Guid itemId)
        {
            try
            {
                await _shoppingListAppService.RemoveItemAsync(id, itemId);
                return Ok(new ApiResponse(true, "Item removed successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list item not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Toggle the completed state of a single item.</summary>
        [HttpPatch("{id:guid}/items/{itemId:guid}/toggle")]
        [ProducesResponseType(typeof(ApiResponse<ShoppingListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleItemAsync(Guid id, Guid itemId)
        {
            try
            {
                var result = await _shoppingListAppService.ToggleItemAsync(id, itemId);
                return Ok(new ApiResponse<ShoppingListItemDto>(true, "Item toggled successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list item not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Mark every item on the list as completed.</summary>
        [HttpPatch("{id:guid}/complete-all")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAllCompletedAsync(Guid id)
        {
            try
            {
                await _shoppingListAppService.MarkAllCompletedAsync(id);
                return Ok(new ApiResponse(true, "All items marked as completed"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Shopping list not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>
        /// Generate a shopping list from a meal plan.
        /// Aggregates all recipe ingredients across every planned meal for the week,
        /// merging duplicates by summing quantities.
        /// </summary>
        [HttpPost("from-meal-plan/{mealPlanId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShoppingListDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateFromMealPlanAsync(Guid mealPlanId)
        {
            try
            {
                var result = await _shoppingListAppService.GenerateFromMealPlanAsync(mealPlanId);
                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<ShoppingListDto>(true, "Shopping list generated successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Meal plan not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }
    }
}
