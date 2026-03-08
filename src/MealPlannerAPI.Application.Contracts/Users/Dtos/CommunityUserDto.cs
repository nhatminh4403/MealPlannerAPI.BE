using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Users.Dtos
{
    public class CommunityUserDto : EntityDto<Guid>
    {
        public string Name { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public int RecipesCreated { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public string? Specialty { get; set; }
    }
}
