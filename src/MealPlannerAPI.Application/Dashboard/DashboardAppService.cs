using MealPlannerAPI.Mappings.Recipes;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using MealPlannerAPI.ShoppingLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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
        private readonly RecipeToTrendingRecipeDtoMapper _toTrendingDtoMapper;
        public DashboardAppService(IRecipeRepository recipeRepository,
                                   IMealPlanRepository mealPlanRepository,
                                   IShoppingListRepository shoppingListRepository,
                                   RecipeToRecipeSummaryDtoMapper toSummaryDtoMapper,
                                   RecipeToTrendingRecipeDtoMapper toTrendingDtoMapper)
        {
            _recipeRepository = recipeRepository;
            _mealPlanRepository = mealPlanRepository;
            _shoppingListRepository = shoppingListRepository;
            _toSummaryDtoMapper = toSummaryDtoMapper;
            _toTrendingDtoMapper = toTrendingDtoMapper;
        }
        public async Task<DashboardDto> GetAsync()
        {
            var stats = await GetStatsAsync();
            var recentRecipes = await GetRecentRecipesAsync();
            var trendingRecipes = await GetTrendingAsync();
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

            return new DashboardStatsDto
            {
                ThisWeekMeals = thisWeekMeals,
                RecipesSaved = recipesSaved,
                TotalCookingMinutes = totalCookingMinutes,
                TotalRecipes = totalRecipes,
                MealPlans = mealPlansTotal,
                ShoppingLists = shoppingListsCount
            };
        }     
        

        public async Task<ListResultDto<TrendingRecipeDto>> GetTrendingAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-RecipeConsts.TrendingWindowDays);
            var query = await _recipeRepository.GetQueryableAsync();

            var recipes = await AsyncExecuter.ToListAsync(
                query.Where(r => r.LastModificationTime >= cutoff || r.CreationTime >= cutoff)
                     .OrderByDescending(r => r.Rating)
                     .ThenByDescending(r => r.ReviewCount)
                     .Take(RecipeConsts.TrendingMaxResults));

            var dtos = recipes.Select(r =>
            {
                var dto = _toTrendingDtoMapper.Map(r);
                // Domain method owns the trending score formula
                dto.TrendingScore = r.CalculateTrendingScore();
                dto.TrendingSince = FormatTrendingSince(r.LastModificationTime ?? r.CreationTime);
                return dto;
            }).ToList();

            return new ListResultDto<TrendingRecipeDto>(dtos);
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

        /// <summary>
        /// Presentation-layer formatting of a relative time string.
        /// Lives here (not on the domain) because it is purely a display concern.
        /// </summary>
        private static string FormatTrendingSince(DateTime since)
        {
            var diff = DateTime.UtcNow - since;
            return diff.TotalDays switch
            {
                < 1 => "today",
                < 2 => "yesterday",
                < 7 => $"{(int)diff.TotalDays} days ago",
                < 14 => "1 week ago",
                < 30 => $"{(int)(diff.TotalDays / 7)} weeks ago",
                _ => $"{(int)(diff.TotalDays / 30)} months ago"
            };
        }
    }
}
