using MealPlannerAPI.Hubs;
using MealPlannerAPI.Mappings.ShoppingLists;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Permissions;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.ShoppingLists.Dtos;
using MealPlannerAPI.ShoppingLists.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;

namespace MealPlannerAPI.ShoppingLists
{
    [RemoteService(false)]
    [Authorize(MealPlannerAPIPermissions.ShoppingLists.Default)]
    public class ShoppingListAppService :
                CrudAppService<ShoppingList,
                                ShoppingListDto,
                                ShoppingListDto,
                                Guid,
                                GetShoppingListsInput,
                                CreateUpdateShoppingListDto,
                                CreateUpdateShoppingListDto>, IShoppingListAppService
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly ShoppingListManager _shoppingListManager;
        private readonly ShoppingListToShoppingListDtoMapper _toShoppingListDtoMapper;
        private readonly ShoppingItemToShoppingItemDtoMapper _toItemDtoMapper;
        private readonly IMealPlannerHubPublisher _hub;
        public ShoppingListAppService(IShoppingListRepository shoppingListRepository,
                                      IMealPlanRepository mealPlanRepository,
                                      IRecipeRepository recipeRepository,
                                      ShoppingListManager shoppingListManager,
                                      ShoppingListToShoppingListDtoMapper toShoppingListDtoMapper,
                                      ShoppingItemToShoppingItemDtoMapper toItemDtoMapper,
                                      IMealPlannerHubPublisher hub) : base(repository: shoppingListRepository)
        {
            _shoppingListRepository = shoppingListRepository;
            _mealPlanRepository = mealPlanRepository;
            _recipeRepository = recipeRepository;
            _shoppingListManager = shoppingListManager;
            _toShoppingListDtoMapper = toShoppingListDtoMapper;
            _toItemDtoMapper = toItemDtoMapper;
            _hub = hub;

            ConfigurePolicies();
        }


        private void ConfigurePolicies()
        {
            GetPolicyName = null;
            GetListPolicyName = MealPlannerAPIPermissions.ShoppingLists.Default;
            CreatePolicyName = MealPlannerAPIPermissions.ShoppingLists.Create;
            UpdatePolicyName = MealPlannerAPIPermissions.ShoppingLists.Update;
            DeletePolicyName = MealPlannerAPIPermissions.ShoppingLists.Delete;
        }
        [Authorize(MealPlannerAPIPermissions.ShoppingLists.ManageItems)]
        public async Task<ShoppingListItemDto> AddItemAsync(Guid shoppingListId, CreateUpdateShoppingListItemDto input)
        {
            var list = await _shoppingListRepository.GetAsync(shoppingListId);
            var item = list.AddItem(GuidGenerator.Create(),
                                    input.Name,
                                    input.Quantity,
                                    isCompleted: false,
                                    input.Unit,
                                    input.Category);

            await _shoppingListRepository.UpdateAsync(list, autoSave: true);

            await _hub.NotifyShoppingListUpdatedAsync(shoppingListId, MapToDto(list));
            return _toItemDtoMapper.Map(item);
        }
        [Authorize(MealPlannerAPIPermissions.ShoppingLists.Create)]
        public async override Task<ShoppingListDto> CreateAsync(CreateUpdateShoppingListDto input)
        {
            var list = new ShoppingList(GuidGenerator.Create(), CurrentUser.GetId(), input.Name);

            foreach (var i in input.Items)
                list.AddItem(GuidGenerator.Create(),
                                    i.Name,
                                    i.Quantity,
                                    isCompleted: false,
                                    i.Unit,
                                    i.Category);

            await _shoppingListRepository.InsertAsync(list, autoSave: true);
            return MapToDto(list);
        }

        [Authorize(MealPlannerAPIPermissions.ShoppingLists.Delete)]
        public override Task DeleteAsync(Guid id)
        {
            var list = _shoppingListRepository.GetAsync(id);
            if (list == null)
                throw new EntityNotFoundException(typeof(ShoppingList), id);
            return Repository.DeleteAsync(id);
        }
        [Authorize(MealPlannerAPIPermissions.ShoppingLists.ManageItems)]
        public async Task<ShoppingListDto> GenerateFromMealPlanAsync(Guid mealPlanId)
        {
            var mealPlan = await _mealPlanRepository.GetAsync(mealPlanId);

            var recipeIds = mealPlan.Entries
                .Where(e => e.RecipeId.HasValue)
                .Select(e => e.RecipeId!.Value)
                .Distinct()
                .ToList();

            var recipeQuery = await _recipeRepository.GetQueryableAsync();
            var recipes = await AsyncExecuter.ToListAsync(
                recipeQuery.Where(r => recipeIds.Contains(r.Id)));

            var list = _shoppingListManager.CreateFromMealPlan(mealPlan, recipes, CurrentUser.GetId());

            await _shoppingListRepository.InsertAsync(list, autoSave: true);
            return MapToDto(list);
        }
        [AllowAnonymous]
        public async override Task<ShoppingListDto> GetAsync(Guid id)
        {
            var list = await _shoppingListRepository.GetAsync(id);
            return MapToDto(list);
        }

        [Authorize(MealPlannerAPIPermissions.ShoppingLists.ManageItems)]
        //[AllowAnonymous]
        public async override Task<PagedResultDto<ShoppingListDto>> GetListAsync(GetShoppingListsInput input)
        {
            var query = await _shoppingListRepository.GetQueryableAsync();

            var currentUserId = CurrentUser.Id;

            query = query.Where(sl => (currentUserId.HasValue && sl.UserId == currentUserId.Value) || sl.UserId == input.UserId);


            if (input.ShowCompleted == false)
                query = query.Where(sl => sl.Items.Any(i => !i.IsCompleted));

            var totalCount = await AsyncExecuter.CountAsync(query);

            var lists = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(sl => sl.CreationTime)
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount));

            return new PagedResultDto<ShoppingListDto>(totalCount, lists.Select(MapToDto).ToList());
        }
        [Authorize(MealPlannerAPIPermissions.ShoppingLists.ManageItems)]
        public async Task MarkAllCompletedAsync(Guid shoppingListId)
        {
            var list = await _shoppingListRepository.GetAsync(shoppingListId);
            list.MarkAllCompleted();
            await _shoppingListRepository.UpdateAsync(list, autoSave: true);
            await _hub.NotifyShoppingListUpdatedAsync(shoppingListId, MapToDto(list));
        }
        [Authorize(MealPlannerAPIPermissions.ShoppingLists.ManageItems)]
        public async Task RemoveItemAsync(Guid shoppingListId, Guid itemId)
        {
            var list = await _shoppingListRepository.GetAsync(shoppingListId);
            list.RemoveItem(itemId);
            await _shoppingListRepository.UpdateAsync(list, autoSave: true);
            await _hub.NotifyShoppingListUpdatedAsync(shoppingListId, MapToDto(list));

        }

        [Authorize(MealPlannerAPIPermissions.ShoppingLists.ManageItems)]
        public async Task<ShoppingListItemDto> ToggleItemAsync(Guid shoppingListId, Guid itemId)
        {
            var list = await _shoppingListRepository.GetAsync(shoppingListId);
            list.ToggleItem(itemId);

            await _shoppingListRepository.UpdateAsync(list, autoSave: true);

            var item = list.Items.First(i => i.Id == itemId);
            var itemDto = _toItemDtoMapper.Map(item);
            await _hub.NotifyShoppingItemToggledAsync(shoppingListId, itemDto);
            return itemDto;
        }

        [Authorize(MealPlannerAPIPermissions.ShoppingLists.Update)]
        public async override Task<ShoppingListDto> UpdateAsync(Guid id, CreateUpdateShoppingListDto input)
        {
            var list = await _shoppingListRepository.GetAsync(id);

            list.Name = input.Name;
            list.ReplaceItems(
                input.Items.Select(i => (GuidGenerator.Create(), i.Name, false, i.Quantity, i.Unit, i.Category)));

            await _shoppingListRepository.UpdateAsync(list, autoSave: true);
            var dto = MapToDto(list);
            await _hub.NotifyShoppingListUpdatedAsync(list.Id, dto);
            return dto;
        }

        [Authorize(MealPlannerAPIPermissions.ShoppingLists.ManageItems)]
        public async Task<ShoppingListItemDto> UpdateItemAsync(Guid shoppingListId, Guid itemId, CreateUpdateShoppingListItemDto input)
        {
            var list = await _shoppingListRepository.GetAsync(shoppingListId);
            list.UpdateItem(itemId, input.Name, input.Quantity, input.Unit, input.Category);

            await _shoppingListRepository.UpdateAsync(list, autoSave: true);

            var item = list.Items.First(i => i.Id == itemId);
            await _hub.NotifyShoppingListUpdatedAsync(shoppingListId, MapToDto(list));
            return _toItemDtoMapper.Map(list.Items.First(i => i.Id == itemId));
        }
        private ShoppingListDto MapToDto(ShoppingList list)
        {
            var dto = _toShoppingListDtoMapper.Map(list);
            dto.Items = _toItemDtoMapper.MapList(list.Items);
            return dto;
        }
    }
}
