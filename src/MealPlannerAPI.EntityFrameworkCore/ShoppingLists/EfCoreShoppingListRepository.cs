using MealPlannerAPI.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MealPlannerAPI.ShoppingLists
{
    internal class EfCoreShoppingListRepository : EfCoreRepository<MealPlannerAPIDbContext, ShoppingList, Guid>, IShoppingListRepository
    {
        public EfCoreShoppingListRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async override Task<ShoppingList> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryableAsync();

            if (!includeDetails)
            {
                query = (await GetDbSetAsync()).AsQueryable();
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ??
                throw new EntityNotFoundException(typeof(ShoppingList), id);


        }

        public async override Task<IQueryable<ShoppingList>> GetQueryableAsync()
        {
            var dbSet = await GetDbSetAsync();
            return dbSet.Include(x => x.Items).AsQueryable();
        }
    }
}