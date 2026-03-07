using MealPlannerAPI.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.ShoppingLists
{
    internal class EfCoreShoppingListRepository : EfCoreRepository<MealPlannerAPIDbContext, ShoppingList, Guid>, IShoppingListRepository
    {
        public EfCoreShoppingListRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<ShoppingList> GetShoppingListWithItems(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            
            var queryable = dbSet.AsQueryable();

            return await queryable.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}