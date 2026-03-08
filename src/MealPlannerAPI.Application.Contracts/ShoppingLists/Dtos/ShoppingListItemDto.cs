using MealPlannerAPI.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.ShoppingLists.Dtos
{
    public class ShoppingListItemDto : AuditedEntityDto<Guid>
    {
        public Guid ShoppingListId { get; set; }
        public string ShoppingListName { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public ShoppingItemCategory Category { get; set; }
    }
}
