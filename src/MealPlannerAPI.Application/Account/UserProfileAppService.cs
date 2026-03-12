using MealPlannerAPI.Dashboard;
using MealPlannerAPI.Hubs;
using MealPlannerAPI.Mappings.Users;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.Users.Dtos;
using MealPlannerAPI.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
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
    public class UserProfileAppService : MealPlannerAPIAppService, IUserProfileAppService
    {
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<MealPlan, Guid> _mealPlanRepository;
        private readonly IRepository<ShoppingList, Guid> _shoppingListRepository;
        private readonly UserProfileToCommunityUserDtoMapper _toCommunityDtoMapper;
        private readonly UserProfileToUserPreferencesDtoMapper _toPreferencesDtoMapper;
        private readonly UserProfileToUserStatsDtoMapper _toStatsDtoMapper;
        private readonly UserProfileToUserPrivacyDtoMapper _toPrivacyDtoMapper;
        private readonly UserProfileToUserNotificationPreferencesDtoMapper _toNotifPrefDtoMapper;
        private readonly CreateUpdateUserPreferencesDtoToUserProfileMapper _toUserProfilePreferencesMapper;
        private readonly CreateUpdateUserSettingsDtoToUserProfileMapper _toUserProfileSettingsMapper;
        private readonly IMealPlannerHubPublisher _hub;

        public UserProfileAppService(IIdentityUserRepository identityUserRepository,
                                     IMealPlanRepository mealPlanRepository,
                                     IShoppingListRepository shoppingListRepository,
                                     UserProfileToCommunityUserDtoMapper toCommunityDtoMapper,
                                     UserProfileToUserPreferencesDtoMapper toPreferencesDtoMapper,
                                     UserProfileToUserStatsDtoMapper toStatsDtoMapper,
                                     UserProfileToUserPrivacyDtoMapper toPrivacyDtoMapper,
                                     UserProfileToUserNotificationPreferencesDtoMapper toNotifPrefDtoMapper,
                                     CreateUpdateUserPreferencesDtoToUserProfileMapper toUserProfilePreferencesMapper,
                                     CreateUpdateUserSettingsDtoToUserProfileMapper toUserProfileSettingsMapper,
                                     IMealPlannerHubPublisher hub,
                                     IdentityUserManager userManager)
        {
            _identityUserRepository = identityUserRepository;
            _mealPlanRepository = mealPlanRepository;
            _shoppingListRepository = shoppingListRepository;
            _toCommunityDtoMapper = toCommunityDtoMapper;
            _toPreferencesDtoMapper = toPreferencesDtoMapper;
            _toStatsDtoMapper = toStatsDtoMapper;
            _toPrivacyDtoMapper = toPrivacyDtoMapper;
            _toNotifPrefDtoMapper = toNotifPrefDtoMapper;
            _toUserProfilePreferencesMapper = toUserProfilePreferencesMapper;
            _toUserProfileSettingsMapper = toUserProfileSettingsMapper;
            _hub = hub;
            _userManager = userManager;
        }

        public async Task<ProfileDto> GetCurrentUserProfileAsync()
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());
            return BuildUserProfileDto(user);
        }

        [AllowAnonymous]
        public async Task<CommunityUserDto> GetCommunityProfileAsync(Guid userId)
        {
            var user = await GetUserProfileAsync(userId);
            return _toCommunityDtoMapper.Map(user);
        }

        [AllowAnonymous]
        public async Task<PagedResultDto<CommunityUserDto>> GetCommunityListAsync(
            PagedAndSortedResultRequestDto input)
        {
            var totalCount = await _identityUserRepository.GetCountAsync();

            var users = await _identityUserRepository.GetListAsync(
                sorting: input.Sorting,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount);

            var profiles = users.OfType<UserProfile>().ToList();

            return new PagedResultDto<CommunityUserDto>(
                totalCount,
                _toCommunityDtoMapper.MapList(profiles));
        }

        // ── Write ─────────────────────────────────────────────────────────────────

        public async Task<ProfileDto> UpdatePreferencesAsync(CreateUpdateUserPreferencesDto input)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());

            _toUserProfilePreferencesMapper.Map(input, user);
            user.SetDietaryRestrictions(input.DietaryRestrictions);
            user.SetCuisinePreferences(input.CuisinePreferences);

            await _identityUserRepository.UpdateAsync(user, autoSave: true);
            return BuildUserProfileDto(user);
        }

        public async Task<ProfileDto> UpdateSettingsAsync(CreateUpdateUserSettingsDto input)
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

        public async Task FollowAsync(Guid targetUserId)
        {
            var currentUser = await GetUserProfileAsync(CurrentUser.GetId());
            var targetUser = await GetUserProfileAsync(targetUserId);

            currentUser.Follow();
            targetUser.GainFollower();

            await _identityUserRepository.UpdateAsync(currentUser);
            await _identityUserRepository.UpdateAsync(targetUser, autoSave: true);

            // Push updated stats to both users
            var currentStats = await BuildStatsAsync(currentUser);
            var targetStats = await BuildStatsAsync(targetUser);

            await _hub.NotifyStatsUpdatedAsync(currentUser.Id, currentStats);
            await _hub.NotifyStatsUpdatedAsync(targetUser.Id, targetStats);
        }

        public async Task UnfollowAsync(Guid targetUserId)
        {
            var currentUser = await GetUserProfileAsync(CurrentUser.GetId());
            var targetUser = await GetUserProfileAsync(targetUserId);

            currentUser.Unfollow();
            targetUser.LoseFollower();

            await _identityUserRepository.UpdateAsync(currentUser);
            await _identityUserRepository.UpdateAsync(targetUser, autoSave: true);

            var currentStats = await BuildStatsAsync(currentUser);
            var targetStats = await BuildStatsAsync(targetUser);

            await _hub.NotifyStatsUpdatedAsync(currentUser.Id, currentStats);
            await _hub.NotifyStatsUpdatedAsync(targetUser.Id, targetStats);
        }


        public async Task<ProfileDto> UpdateProfileInfoAsync(UpdateProfileInfoDto input)
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

        public async Task ChangePasswordAsync(Volo.Abp.Account.ChangePasswordInput input)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());

            (await _userManager.ChangePasswordAsync(
                user,
                input.CurrentPassword,
                input.NewPassword)).CheckErrors();
        }

        public async Task ChangeEmailAsync(ChangeEmailDto input)
        {
            var user = await GetUserProfileAsync(CurrentUser.GetId());

            // GenerateChangeEmailTokenAsync + ChangeEmailAsync is the two-step ABP flow.
            // For simplicity we use SetEmailAsync which skips the confirmation token.
            // Swap to the two-step flow if email verification.
            (await _userManager.SetEmailAsync(user, input.NewEmail)).CheckErrors();

            await _identityUserRepository.UpdateAsync(user, autoSave: true);
        }


        // ── Helpers ───────────────────────────────────────────────────────────────

        private async Task<UserProfile> GetUserProfileAsync(Guid userId)
            => await _identityUserRepository.GetAsync(userId) as UserProfile
                ?? throw new EntityNotFoundException(typeof(UserProfile), userId);

        private ProfileDto BuildUserProfileDto(UserProfile user)
        {
            var preferences = _toPreferencesDtoMapper.Map(user);
            preferences.DietaryRestrictions = user.GetDietaryRestrictions();
            preferences.CuisinePreferences = user.GetCuisinePreferences();

            return new ProfileDto
            {
                Id = user.Id,
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
                }
            };
        }
        private async Task<DashboardStatsDto> BuildStatsAsync(UserProfile user)
        {
            var weekStart = MealPlan.GetWeekStart(DateTime.UtcNow);

            var mealPlanQuery = await _mealPlanRepository.GetQueryableAsync();
            var shoppingQuery = await _shoppingListRepository.GetQueryableAsync();

            var thisWeekMeals = await AsyncExecuter.CountAsync(
                mealPlanQuery
                    .Where(mp => mp.UserId == user.Id && mp.WeekStartDate == weekStart)
                    .SelectMany(mp => mp.Entries));

            var mealPlansTotal = await AsyncExecuter.CountAsync(
                mealPlanQuery.Where(mp => mp.UserId == user.Id));

            var shoppingListsCount = await AsyncExecuter.CountAsync(
                shoppingQuery.Where(sl => sl.UserId == user.Id));

            return new DashboardStatsDto
            {
                ThisWeekMeals = thisWeekMeals,
                RecipesSaved = user.RecipesCreated,
                TotalCookingMinutes = 0, // not relevant for a follow/unfollow push
                TotalRecipes = user.RecipesCreated,
                MealPlans = mealPlansTotal,
                ShoppingLists = shoppingListsCount
            };
        }


    }
}
