using MealPlannerAPI.Dashboard;
using MealPlannerAPI.Recipes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace MealPlannerAPI.BackgroundJobs
{
    public class TrendingRefreshWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public TrendingRefreshWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            Timer.Period = RecipeConsts.TrendingRefreshIntervalMs;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var refreshService = workerContext.ServiceProvider
                .GetRequiredService<ITrendingRecipeRefreshService>();

            await refreshService.RefreshAsync();
        }
    }
}
