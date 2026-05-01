using MealPlannerAPI.Hubs;
using MealPlannerAPI.Mappings.Recipes;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace MealPlannerAPI.Dashboard
{
    public class TrendingRecipeRefreshService : MealPlannerAPIAppService, ITrendingRecipeRefreshService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly TrendingRecipeCache _trendingCache;
        private readonly IMealPlannerHubPublisher _hub;
        private readonly RecipeToTrendingRecipeDtoMapper _mapper;

        public TrendingRecipeRefreshService(IRecipeRepository recipeRepository,
                                            TrendingRecipeCache trendingCache,
                                            IMealPlannerHubPublisher hub,
                                            RecipeToTrendingRecipeDtoMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _trendingCache = trendingCache;
            _hub = hub;
            _mapper = mapper;
        }

        public async Task<List<TrendingRecipeDto>> FetchFromDbAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-RecipeConsts.TrendingWindowDays);
            var query = await _recipeRepository.GetQueryableAsync();

            var recipes = await AsyncExecuter.ToListAsync(
                query
                    .Where(r => r.LastModificationTime >= cutoff || r.CreationTime >= cutoff)
                    .OrderByDescending(r => r.Rating)
                    .ThenByDescending(r => r.ReviewCount)
                    .Take(RecipeConsts.TrendingMaxResults));

            return recipes.Select(r =>
            {
                var dto = _mapper.Map(r);
                dto.TrendingScore = r.CalculateTrendingScore();
                dto.TrendingSince = FormatTrendingSince(r.LastModificationTime ?? r.CreationTime);
                return dto;
            }).ToList();
        }

        public async Task RefreshAsync()
        {
            await _trendingCache.InvalidateAsync();
            var fresh = await FetchFromDbAsync();
            await _trendingCache.SetAsync(fresh);
            await _hub.NotifyTrendingUpdatedAsync();
        }
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
