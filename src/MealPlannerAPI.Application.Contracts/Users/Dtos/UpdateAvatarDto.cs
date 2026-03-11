namespace MealPlannerAPI.Users.Dtos
{
    public class UpdateAvatarDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(2048)]
        public string AvatarUrl { get; set; } = null!;
    }
}
