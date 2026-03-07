using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.ShoppingLists
{
    public class ShoppingList : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
        protected ShoppingList() { }
        public ShoppingList(Guid id, Guid userId, string name) : base(id)
        {
            UserId = userId;
            Name = name;
        }
    }
}
