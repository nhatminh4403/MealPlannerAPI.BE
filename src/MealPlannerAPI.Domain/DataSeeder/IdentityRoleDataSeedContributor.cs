using MealPlannerAPI.Permissions;
using MealPlannerAPI.Users;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace MealPlannerAPI.DataSeeder
{
    public class IdentityRoleDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IdentityUserManager _identityUserManager;
        private readonly ILookupNormalizer _lookupNormalizer;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IPermissionDataSeeder _permissionDataSeeder;
        private readonly IGuidGenerator _guidGenerator;

        public int Order => 1;
        public IdentityRoleDataSeedContributor(IdentityUserManager identityUserManager, ILookupNormalizer lookupNormalizer, IIdentityRoleRepository identityRoleRepository, IIdentityUserRepository identityUserRepository, IPermissionDataSeeder permissionDataSeeder, IGuidGenerator guidGenerator)
        {
            _identityUserManager = identityUserManager;
            _lookupNormalizer = lookupNormalizer;
            _identityRoleRepository = identityRoleRepository;
            _identityUserRepository = identityUserRepository;
            _permissionDataSeeder = permissionDataSeeder;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var communityUser = await CreateRoleIfNotExistsAsync(ExtendedRoleConsts.CommunityUser, isPublic: true);

            await GrantPermissionsToRoleAsync(communityUser.Name, new[]
            {
                MealPlannerAPIPermissions.MealPlans.Default,
                MealPlannerAPIPermissions.MealPlans.Create,
                MealPlannerAPIPermissions.MealPlans.Update,
                MealPlannerAPIPermissions.MealPlans.Delete,
                MealPlannerAPIPermissions.Recipes.Default,
                MealPlannerAPIPermissions.Recipes.Create,
                MealPlannerAPIPermissions.Recipes.Update,
                MealPlannerAPIPermissions.Recipes.Delete,
                MealPlannerAPIPermissions.ShoppingLists.Default,
                MealPlannerAPIPermissions.ShoppingLists.Create,
                MealPlannerAPIPermissions.ShoppingLists.Update,
                MealPlannerAPIPermissions.ShoppingLists.Delete,
                MealPlannerAPIPermissions.Notifications.Default,
                MealPlannerAPIPermissions.Notifications.Delete,
                MealPlannerAPIPermissions.Dashboard.Default,
                MealPlannerAPIPermissions.UserProfiles.Default,
                MealPlannerAPIPermissions.UserProfiles.UpdateOthers
            });


            var normalizedAdminName = _lookupNormalizer.NormalizeName("admin");
            var adminRole = await _identityRoleRepository.FindByNormalizedNameAsync(normalizedAdminName);
            if (adminRole == null) return;
            if (adminRole.IsPublic)
            {
                adminRole.IsPublic = false;
                await _identityRoleRepository.UpdateAsync(adminRole);
            }
        }
        private async Task<IdentityRole> CreateRoleIfNotExistsAsync(string roleName,
                                                                              bool isDefault = false,
                                                                              bool isPublic = false)
        {
            var normalizedRoleName = _lookupNormalizer.NormalizeName(roleName);
            var existingRole = await _identityRoleRepository.FindByNormalizedNameAsync(normalizedRoleName);
            if (existingRole != null) 
            {
                if(existingRole.IsPublic != isPublic || existingRole.IsDefault != isDefault)
                {
                    existingRole.IsPublic = isPublic;
                    existingRole.IsDefault = isDefault;
                    await _identityRoleRepository.UpdateAsync(existingRole);
                }
                return existingRole;
            }


            var role = new IdentityRole(_guidGenerator.Create(), roleName)
            {
                IsDefault = isDefault,
                IsPublic = isPublic
            };
            role.ChangeName(roleName);

            await _identityRoleRepository.InsertAsync(role);
            return role;
        }

        private async Task GrantPermissionsToRoleAsync(string roleName, string[] permissions)
        {
            await _permissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                roleName,
                permissions
            );
        }
    }
}
