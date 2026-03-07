using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.ShoppingLists
{
    public interface IShoppingListRepository : IRepository<ShoppingList, Guid>
    {
        Task<ShoppingList> GetShoppingListWithItems(Guid id);
    }
}
