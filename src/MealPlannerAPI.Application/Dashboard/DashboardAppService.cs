using MealPlannerAPI.Mappings.Recipes;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using MealPlannerAPI.Recipes.Services;
using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.Users;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace MealPlannerAPI.Dashboard
{

    [RemoteService(false)]
    public class DashboardAppService : MealPlannerAPIAppService, IDashboardAppService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly RecipeToRecipeSummaryDtoMapper _toSummaryDtoMapper;
        private readonly IRecipeAppService _recipeAppService;
        private readonly IRepository<UserProfile, Guid> _userProfileRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        public DashboardAppService(IRecipeRepository recipeRepository,
                                   IMealPlanRepository mealPlanRepository,
                                   IShoppingListRepository shoppingListRepository,
                                   RecipeToRecipeSummaryDtoMapper toSummaryDtoMapper,
                                   IRecipeAppService recipeAppService,
                                   IRepository<UserProfile, Guid> userProfileRepository,
                                   IIdentityUserRepository identityUserRepository)
        {
            _recipeRepository = recipeRepository;
            _mealPlanRepository = mealPlanRepository;
            _shoppingListRepository = shoppingListRepository;
            _toSummaryDtoMapper = toSummaryDtoMapper;
            _userProfileRepository = userProfileRepository;
            _recipeAppService = recipeAppService;
            _identityUserRepository = identityUserRepository;
        }

        public async Task<DashboardDto> GetAsync()
        {
            var stats = await GetStatsAsync();
            var recentRecipes = await GetRecentRecipesAsync();
            var trendingRecipes = await _recipeAppService.GetTrendingAsync();
            return new DashboardDto
            {
                Stats = stats,
                RecentRecipes = recentRecipes.Items.ToList(),
                TrendingRecipes = trendingRecipes.Items.ToList()
            };
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            var userId = CurrentUser.GetId();
            var weekStart = MealPlan.GetWeekStart(DateTime.UtcNow);

            var mealPlanQuery = await _mealPlanRepository.GetQueryableAsync();
            var recipeQuery = await _recipeRepository.GetQueryableAsync();
            var shoppingQuery = await _shoppingListRepository.GetQueryableAsync();

            var thisWeekMeals = await AsyncExecuter.CountAsync(
                mealPlanQuery
                    .Where(mp => mp.UserId == userId && mp.WeekStartDate == weekStart)
                    .SelectMany(mp => mp.Entries));

            var recipesSaved = await AsyncExecuter.CountAsync(
                recipeQuery.Where(r => r.AuthorId == userId));

            var mealPlansTotal = await AsyncExecuter.CountAsync(
                mealPlanQuery.Where(mp => mp.UserId == userId));

            var shoppingListsCount = await AsyncExecuter.CountAsync(
                shoppingQuery.Where(sl => sl.UserId == userId));

            var totalRecipes = await AsyncExecuter.CountAsync(recipeQuery);

            var weeklyRecipeIds = await AsyncExecuter.ToListAsync(
                mealPlanQuery
                    .Where(mp => mp.UserId == userId && mp.WeekStartDate == weekStart)
                    .SelectMany(mp => mp.Entries)
                    .Where(e => e.RecipeId.HasValue)
                    .Select(e => e.RecipeId!.Value));

            var totalCookingMinutes = 0;
            if (weeklyRecipeIds.Any())
            {
                totalCookingMinutes = await AsyncExecuter.SumAsync(
                    recipeQuery
                        .Where(r => weeklyRecipeIds.Contains(r.Id))
                        .Select(r => r.CookingTimeMinutes + r.PrepTimeMinutes));
            }

            var followerCount = await GetCurrentUserFollower(userId);
            var followingCount = await GetCurrentUserFollowing(userId);
            return new DashboardStatsDto
            {
                ThisWeekMeals = thisWeekMeals,
                RecipesSaved = recipesSaved,
                TotalCookingMinutes = totalCookingMinutes,
                TotalRecipes = totalRecipes,
                MealPlans = mealPlansTotal,
                ShoppingLists = shoppingListsCount,
                FollowerCount = followerCount,
                FollowingCount = followingCount,
            };
        }

        private async Task<ListResultDto<RecipeSummaryDto>> GetRecentRecipesAsync()
        {
            var query = await _recipeRepository.GetQueryableAsync();

            var recipes = await AsyncExecuter
                .ToListAsync(query.OrderByDescending(r => r.CreationTime)
                .Take(RecipeConsts.RecentRecipesMaxResults));

            var summaries = recipes.Select(r =>
            {
                var dto = _toSummaryDtoMapper.Map(r);
                dto.TotalTimeMinutes = r.GetTotalTimeMinutes();
                dto.Tags = r.GetTags();
                return dto;
            }).ToList();

            return new ListResultDto<RecipeSummaryDto>(summaries);
        }

        private async Task<int> GetCurrentUserFollower(Guid id)
        {
            var queryable = await _userProfileRepository.GetQueryableAsync();
            var result = await AsyncExecuter.FirstOrDefaultAsync(
                queryable.Where(u => u.Id == id)
                         .Select(u => u.Followers)
                );

            return result;
        }
        private async Task<int> GetCurrentUserFollowing(Guid id)
        {
            var queryable = await _userProfileRepository.GetQueryableAsync();
            var result = await AsyncExecuter.FirstOrDefaultAsync(
                queryable.Where(u => u.Id == id)
                         .Select(u => u.Followers)
                );

            return result;
        }
    }
}
