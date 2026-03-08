using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.ShoppingLists.Dtos
{
    public class ShoppingListDto : FullAuditedEntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public List<ShoppingListItemDto> Items { get; set; } = new();
        public int TotalItems => Items.Count;
        public int CompletedItems => Items.FindAll(i => i.IsCompleted).Count;
    }
}
