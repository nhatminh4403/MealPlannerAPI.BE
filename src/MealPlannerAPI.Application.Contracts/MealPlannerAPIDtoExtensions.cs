using System.ComponentModel.DataAnnotations;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;
using Volo.Abp.Validation;

namespace MealPlannerAPI;

public static class MealPlannerAPIDtoExtensions
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();
    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* You can add extension properties to DTOs
             * defined in the depended modules.
             *
             * Example:
             *
             * ObjectExtensionManager.Instance
             *   .AddOrUpdateProperty<IdentityRoleDto, string>("Title");
             *
             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Object-Extensions
             */


            ObjectExtensionManager.Instance
               .AddOrUpdateProperty<RegisterDto, string>(
                   propertyName: "Name".ToLower()
                   ,
                         property =>
                         {
                             property.Attributes.Add(new RequiredAttribute());
                             property.Attributes.Add(
                                 new DynamicStringLengthAttribute(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength)));
                         }
               );

            ObjectExtensionManager.Instance
                .AddOrUpdateProperty<RegisterDto, string>(
                   propertyName: "Surname".ToLower()
                   ,
                    property =>
                    {
                        property.Attributes.Add(new RequiredAttribute());
                        property.Attributes.Add(
                            new DynamicStringLengthAttribute(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxSurnameLength)));
                    }
                );
        });
    }
}
