using MealPlannerAPI.Dashboard;
using MealPlannerAPI.Recipes.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Caching;
namespace MealPlannerAPI.Recipes
{
    public class TrendingRecipeCache
    {
        private const string CacheKey = "trending-recipes";
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

        private readonly IDistributedCache<TrendingRecipeCacheItem> _cache;

        public TrendingRecipeCache(IDistributedCache<TrendingRecipeCacheItem> cache)
        {
            _cache = cache;
        }

        public Task<TrendingRecipeCacheItem?> GetAsync()
            => _cache.GetAsync(CacheKey);

        public Task SetAsync(List<TrendingRecipeDto> items)
            => _cache.SetAsync(CacheKey, new TrendingRecipeCacheItem { Items = items },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = Ttl
                });

        public Task InvalidateAsync()
            => _cache.RemoveAsync(CacheKey);
    }
}
