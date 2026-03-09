using MealPlannerAPI.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.ShoppingLists.Dtos
{
    public class CreateUpdateShoppingListItemDto
    {
        [Required, MaxLength(256)]
        public string Name { get; set; } = null!;

        [Range(0.001, double.MaxValue)]
        public decimal Quantity { get; set; }


        [Required, MaxLength(32)]
        public string Unit { get; set; } = null!;

        public ShoppingItemCategory Category { get; set; }
    }
}
