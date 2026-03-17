using MealPlannerAPI.Enums;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.ShoppingLists
{
    public class ShoppingListItem : AuditedEntity<Guid>
    {
        public Guid ShoppingListId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public ShoppingItemCategory Category { get; set; }
        public ShoppingListItem(Guid id, Guid shoppingListId,
                                string name,
                                bool isCompleted,
                                decimal quantity,
                                string unit,
                                ShoppingItemCategory category) : base(id)
        {
            ShoppingListId = shoppingListId;
            Name = name;
            IsCompleted = isCompleted;
            Quantity = quantity;
            Unit = unit;
            Category = category;
        }



    }
}
