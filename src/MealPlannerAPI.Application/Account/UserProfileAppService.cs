using MealPlannerAPI.Account;
using MealPlannerAPI.Dashboard;
using MealPlannerAPI.Hubs;
using MealPlannerAPI.Mappings.Users;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.Users.Dtos;
using MealPlannerAPI.Users.Interfaces;
using MealPlannerAPI.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace MealPlannerAPI.Users
{
    [Authorize]
    [RemoteService(false)]
    public class UserProfileAppService : UserAppServiceBase, IUserProfileAppService
    {
        #region Dependency Injections

        #region Interfaces & ABP Services
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IUserFollowAppService _userFollowAppService;
        #endregion
        #region Mapperly Mapper Classes
        private readonly UserProfileToCommunityUserDtoMapper _toCommunityDtoMapper;
        private readonly UserProfileToUserPreferencesDtoMapper _toPreferencesDtoMapper;
        private readonly UserProfileToUserStatsDtoMapper _toStatsDtoMapper;
        private readonly UserProfileToUserPrivacyDtoMapper _toPrivacyDtoMapper;
        private readonly UserProfileToUserNotificationPreferencesDtoMapper _toNotifPrefDtoMapper;
        private readonly CreateUpdateUserPreferencesDtoToUserProfileMapper _toUserProfilePreferencesMapper;
        private readonly CreateUpdateUserSettingsDtoToUserProfileMapper _toUserProfileSettingsMapper;
        #endregion

        #endregion

        #region Constructor
        public UserProfileAppService(IIdentityUserRepository identityUserRepository,
                                     UserProfileToCommunityUserDtoMapper toCommunityDtoMapper,
                                     UserProfileToUserPreferencesDtoMapper toPreferencesDtoMapper,
                                     UserProfileToUserStatsDtoMapper toStatsDtoMapper,
                                     UserProfileToUserPrivacyDtoMapper toPrivacyDtoMapper,
                                     UserProfileToUserNotificationPreferencesDtoMapper toNotifPrefDtoMapper,
                                     CreateUpdateUserPreferencesDtoToUserProfileMapper toUserProfilePreferencesMapper,
                                     CreateUpdateUserSettingsDtoToUserProfileMapper toUserProfileSettingsMapper,
                                     IdentityUserManager userManager,
                                     IUserFollowAppService userFollowAppService) : base(identityUserRepository)

        {
            _identityUserRepository = identityUserRepository;
            _toCommunityDtoMapper = toCommunityDtoMapper;
            _toPreferencesDtoMapper = toPreferencesDtoMapper;
            _toStatsDtoMapper = toStatsDtoMapper;
            _toPrivacyDtoMapper = toPrivacyDtoMapper;
            _toNotifPrefDtoMapper = toNotifPrefDtoMapper;
            _toUserProfilePreferencesMapper = toUserProfilePreferencesMapper;
            _toUserProfileSettingsMapper = toUserProfileSettingsMapper;
            _userManager = userManager;
            _userFollowAppService = userFollowAppService;
        }
        #endregion
        public async Task<ProfileDto> GetCurrentUserProfileAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await GetUserProfileAsync(CurrentUser.GetId(), cancellationToken);
                return BuildUserProfileDto(user);
            }
            catch (OperationCanceledException)
            {
                // Client navigated away — not a real error
                throw new UserFriendlyException("Request was cancelled.");
            }
        }
        [AllowAnonymous]
        public async Task<CommunityUserDto> GetCommunityProfileAsync(Guid userId)
        {
            var user = await GetUserProfileAsync(userId);
            var dto = _toCommunityDtoMapper.Map(user);
            await _userFollowAppService.FillFollowStateAsync(new[] { dto });
            return dto;
        }
        [AllowAnonymous]
        public async Task<PagedResultDto<CommunityUserDto>> GetCommunityListAsync(PagedAndSortedResultRequestDto input,
                                                                                  CancellationToken cancellationToken = default)
        {
            var totalCount = await _identityUserRepository.GetCountAsync();

            var users = await _identityUserRepository.GetListAsync(
                sorting: input.Sorting,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount);

            var profiles = users.OfType<UserProfile>().ToList();
            var dtos = _toCommunityDtoMapper.MapList(profiles);
            await _userFollowAppService.FillFollowStateAsync(dtos);
            return new PagedResultDto<CommunityUserDto>(totalCount, dtos);
        }
        // ── Write ─────────────────────────────────────────────────────────────────
        public async Task<ProfileDto> UpdatePreferencesAsync(CreateUpdateUserPreferencesDto input,
                                                             CancellationToken cancellationToken = default)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());

            _toUserProfilePreferencesMapper.Map(input, user);
            user.SetDietaryRestrictions(input.DietaryRestrictions);
            user.SetCuisinePreferences(input.CuisinePreferences);

            await _identityUserRepository.UpdateAsync(user, autoSave: true);
            return BuildUserProfileDto(user);
        }
        public async Task<ProfileDto> UpdateSettingsAsync(CreateUpdateUserSettingsDto input,
                                                          CancellationToken cancellationToken = default)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());
            _toUserProfileSettingsMapper.Map(input, user);

            await _identityUserRepository.UpdateAsync(user, autoSave: true);
            return BuildUserProfileDto(user);
        }
        public async Task<ProfileDto> UpdateAvatarAsync(string avatarUrl)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());
            user.UpdateAvatar(avatarUrl);

            await _identityUserRepository.UpdateAsync(user, autoSave: true);
            return BuildUserProfileDto(user);
        }
        public async Task<ProfileDto> UpdateProfileInfoAsync(UpdateProfileInfoDto input,
                                                             CancellationToken cancellationToken = default)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());

            user.Name = input.Name;
            user.Surname = input.Surname;

            // UserName and PhoneNumber go through the manager for normalisation + uniqueness
            (await _userManager.SetUserNameAsync(user, input.UserName)).CheckErrors();
            (await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();

            await _identityUserRepository.UpdateAsync(user, autoSave: true);
            return BuildUserProfileDto(user);
        }
        public async Task ChangePasswordAsync(Volo.Abp.Account.ChangePasswordInput input,
                                              CancellationToken cancellation = default)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());

            (await _userManager.ChangePasswordAsync(
                user,
                input.CurrentPassword,
                input.NewPassword)).CheckErrors();
        }
        public async Task ChangeEmailAsync(ChangeEmailDto input,
                                           CancellationToken cancellationToken = default)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());

            // GenerateChangeEmailTokenAsync + ChangeEmailAsync is the two-step ABP flow.
            // For simplicity we use SetEmailAsync which skips the confirmation token.
            // Swap to the two-step flow if email verification.
            (await _userManager.SetEmailAsync(user, input.NewEmail)).CheckErrors();

            await _identityUserRepository.UpdateAsync(user, autoSave: true);
        }
        public async Task<UserSettingsDto> GetProfileSettingsAsync()
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());
            return BuildSettingsDto(user);
        }
        // ── Helpers ───────────────────────────────────────────────────────────────
        private ProfileDto BuildUserProfileDto(UserProfile user)
        {
            var preferences = _toPreferencesDtoMapper.Map(user);
            preferences.DietaryRestrictions = user.GetDietaryRestrictions();
            preferences.CuisinePreferences = user.GetCuisinePreferences();

            return new ProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                AvatarUrl = user.AvatarUrl,
                CreationTime = user.CreationTime,
                CreatorId = user.CreatorId,
                LastModificationTime = user.LastModificationTime,
                LastModifierId = user.LastModifierId,
                IsDeleted = user.IsDeleted,
                DeletionTime = user.DeletionTime,
                DeleterId = user.DeleterId,
                Preferences = preferences,
                Stats = _toStatsDtoMapper.Map(user),
                Settings = new UserSettingsDto
                {
                    Privacy = _toPrivacyDtoMapper.Map(user),
                    Notifications = _toNotifPrefDtoMapper.Map(user)
                },
                FollowersCount = user.Followers,
                FollowingCount = user.Following,
            };
        }

        private UserSettingsDto BuildSettingsDto(UserProfile user) => new()
        {
            Privacy = _toPrivacyDtoMapper.Map(user),
            Notifications = _toNotifPrefDtoMapper.Map(user)
        };

    }
}
