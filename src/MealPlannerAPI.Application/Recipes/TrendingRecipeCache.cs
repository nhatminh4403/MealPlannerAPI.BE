using MealPlannerAPI.Recipes.Caching;
using MealPlannerAPI.Recipes.Dtos;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
namespace MealPlannerAPI.Recipes
{
    public class TrendingRecipeCache : ISingletonDependency
    {

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

        private readonly IDistributedCache<TrendingRecipeCacheItem> _cache;

        public TrendingRecipeCache(IDistributedCache<TrendingRecipeCacheItem> cache)
        {
            _cache = cache;
        }

        public async Task<List<TrendingRecipeDto>> GetOrSetAsync(Func<Task<List<TrendingRecipeDto>>> factory)
        {
            var cached = await _cache.GetOrAddAsync(
                RecipeConsts.CacheKey, async () => new TrendingRecipeCacheItem
                {
                    Items = await factory()
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = Ttl
                });

            return cached?.Items ?? new List<TrendingRecipeDto>();
        }

        public Task SetAsync(List<TrendingRecipeDto> items)
        {
            return _cache.SetAsync(
                        RecipeConsts.CacheKey,
                        new TrendingRecipeCacheItem { Items = items },
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Ttl });
        }
        public async Task<List<TrendingRecipeDto>?> GetAsync()
        {
            var cached = await _cache.GetAsync(RecipeConsts.CacheKey);
            return cached?.Items;
        }
        public Task InvalidateAsync()
            => _cache.RemoveAsync(RecipeConsts.CacheKey);
    }
}
