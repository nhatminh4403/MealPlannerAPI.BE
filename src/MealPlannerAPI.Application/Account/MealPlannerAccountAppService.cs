using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace MealPlannerAPI.Account
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(AccountAppService), typeof(IAccountAppService))]
    [RemoteService(IsEnabled = false)]
    public class MealPlannerAccountAppService : AccountAppService, ITransientDependency
    {
        public MealPlannerAccountAppService(IdentityUserManager userManager, IIdentityRoleRepository roleRepository, IAccountEmailer accountEmailer, IdentitySecurityLogManager identitySecurityLogManager, IOptions<IdentityOptions> identityOptions) : base(userManager, roleRepository, accountEmailer, identitySecurityLogManager, identityOptions)
        {
        }

        public override async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
        {
            Logger.LogWarning("ExtraProperties count: {Count}", input.ExtraProperties.Count);
            foreach (var kvp in input.ExtraProperties)
            {
                Logger.LogWarning("ExtraProp: {Key} = {ValueType} | {Value}",
                    kvp.Key,
                    kvp.Value?.GetType().FullName ?? "NULL_TYPE",
                    kvp.Value?.ToString() ?? "NULL_VALUE");
            }

            var result = await base.RegisterAsync(input);

            var name = input.GetProperty<string>("name");
            var surname = input.GetProperty<string>("surname");
            Logger.LogWarning("Surname: {Surname}", surname);
            Logger.LogWarning("Name: {Name}", name);

            if (name.IsNullOrWhiteSpace() && surname.IsNullOrWhiteSpace())
            {
                return result;
            }

            var user = await UserManager.GetByIdAsync(result.Id);
            if (!name.IsNullOrWhiteSpace())
            {
                user.Name = name;
            }

            if (!surname.IsNullOrWhiteSpace())
            {
                user.Surname = surname;
            }
            (await UserManager.UpdateAsync(user)).CheckErrors();

            return ObjectMapper.Map<Volo.Abp.Identity.IdentityUser, IdentityUserDto>(user);
        }
    }
}
