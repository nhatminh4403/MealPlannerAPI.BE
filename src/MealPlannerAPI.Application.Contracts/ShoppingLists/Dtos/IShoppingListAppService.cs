using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.ShoppingLists.Dtos
{
    public interface IShoppingListAppService : IApplicationService
    {
        Task<ShoppingListDto> GetAsync(Guid id);
        Task<PagedResultDto<ShoppingListDto>> GetListAsync(GetShoppingListsInput input);
        Task<ShoppingListDto> CreateAsync(CreateUpdateShoppingListDto input);
        Task<ShoppingListDto> UpdateAsync(Guid id, CreateUpdateShoppingListDto input);
        Task DeleteAsync(Guid id);
        Task ToggleItemCompletionAsync(ToggleShoppingItemDto input);
    }
}
