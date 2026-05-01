using MealPlannerAPI.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace MealPlannerAPI.Identity
{
    public class ExtendedIdentityUserManager : IdentityUserManager
    {
        public ExtendedIdentityUserManager(IdentityUserStore store, IIdentityRoleRepository roleRepository, IIdentityUserRepository userRepository, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<IdentityUser> passwordHasher, IEnumerable<IUserValidator<IdentityUser>> userValidators, IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<IdentityUserManager> logger, ICancellationTokenProvider cancellationTokenProvider, IOrganizationUnitRepository organizationUnitRepository, ISettingProvider settingProvider, IDistributedEventBus distributedEventBus, IIdentityLinkUserRepository identityLinkUserRepository, IDistributedCache<AbpDynamicClaimCacheItem> dynamicClaimCache, IOptions<AbpMultiTenancyOptions> multiTenancyOptions, ICurrentTenant currentTenant, IDataFilter dataFilter) : base(store, roleRepository, userRepository, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, cancellationTokenProvider, organizationUnitRepository, settingProvider, distributedEventBus, identityLinkUserRepository, dynamicClaimCache, multiTenancyOptions, currentTenant, dataFilter)
        {
        }

        public override async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await base.CreateAsync(user);

            if (result.Succeeded)
                await AddToRoleAsync(user, ExtendedRoleConsts.CommunityUser);

            return result;
        }

        public override async Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {
            var result = await base.CreateAsync(user, password);

            if (result.Succeeded)
                await AddToRoleAsync(user, ExtendedRoleConsts.CommunityUser);

            return result;
        }
    }
}
