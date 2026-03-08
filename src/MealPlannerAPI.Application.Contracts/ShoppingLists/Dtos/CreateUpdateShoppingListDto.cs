using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.ShoppingLists.Dtos
{
    public class CreateUpdateShoppingListDto
    {
        [MaxLength(1000)]
        public string Name { get; set; }
        public List<CreateUpdateShoppingListItemDto> Items { get; set; } = new();
    }
}
