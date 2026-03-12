using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.Users.Dtos
{
    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = null!;
    }
}
