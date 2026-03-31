using MealPlannerAPI.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace MealPlannerAPI.MealPlans.BackgroundJobs
{
    public class MealPlanHardDeleteWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly HardDeleteOptions _options;

        public MealPlanHardDeleteWorker(AbpAsyncTimer timer,
                                        IServiceScopeFactory serviceScopeFactory,
                                         IOptions<HardDeleteOptions> options,
                                        IMealPlanRepository mealPlanRepository) : base(timer, serviceScopeFactory)
        {
            _mealPlanRepository = mealPlanRepository;
            _options = options.Value;
            Timer.Period = _options.PeriodHours * 60 * 60 * 1000;

        }
        [UnitOfWork]
        protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("[HardDeleteWorker] Starting cleanup...");

            var dataFilter = workerContext.ServiceProvider.GetRequiredService<IDataFilter>();
            var cutoff = DateTime.UtcNow.AddDays(-_options.RetentionDays);

            using (dataFilter.Disable<ISoftDelete>())
            {
                await PurgeAsync<MealPlan, Guid>(workerContext, cutoff);
                await PurgeAsync<MealPlanEntry, Guid>(workerContext, cutoff);
            }
        }
        private async Task PurgeAsync<TEntity, TKey>(PeriodicBackgroundWorkerContext ctx, DateTime cutoff)
            where TEntity : class, IEntity<TKey>, ISoftDelete, IHasDeletionTime
        {
            var repo = ctx.ServiceProvider
                .GetRequiredService<IRepository<TEntity, TKey>>();

            var toDelete = await repo.GetListAsync(
                x => x.IsDeleted 
                && x.DeletionTime != null 
                && x.DeletionTime < cutoff
            );

            foreach (var entity in toDelete)
            {
                await repo.HardDeleteAsync(x => x.Id!.Equals(entity.Id));
            }

            Logger.LogInformation(
                "[HardDeleteWorker] Purged {Count} {Entity} records.",
                toDelete.Count, typeof(TEntity).Name
            );
        }
    }
}
