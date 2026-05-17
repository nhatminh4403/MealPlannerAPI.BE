using System;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.ShoppingLists
{
    public interface IShoppingListRepository : IRepository<ShoppingList, Guid>
    {
    }
}
