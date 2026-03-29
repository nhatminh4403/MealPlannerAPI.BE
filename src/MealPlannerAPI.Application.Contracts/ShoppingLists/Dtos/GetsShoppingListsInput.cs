using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.ShoppingLists.Dtos
{
    public class GetShoppingListsInput : PagedAndSortedResultRequestDto
    {
        public bool? ShowCompleted { get; set; }
        public Guid UserId { get; set; }
    }
}
