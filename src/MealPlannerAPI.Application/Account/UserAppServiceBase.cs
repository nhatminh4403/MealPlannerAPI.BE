using MealPlannerAPI.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace MealPlannerAPI.Account
{
    public abstract class UserAppServiceBase : MealPlannerAPIAppService
    {
        protected readonly IIdentityUserRepository IdentityUserRepository;

        protected UserAppServiceBase(IIdentityUserRepository identityUserRepository)
            => IdentityUserRepository = identityUserRepository;

        protected async Task<UserProfile> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var identityUser = await IdentityUserRepository.GetAsync(
                userId, cancellationToken: cancellationToken);

            if (identityUser is UserProfile profile) return profile;

            return new UserProfile(identityUser.Id, identityUser.UserName, identityUser.Email, identityUser.TenantId)
            {
                Name = identityUser.Name,
                Surname = identityUser.Surname
            };
        }
    }
}
