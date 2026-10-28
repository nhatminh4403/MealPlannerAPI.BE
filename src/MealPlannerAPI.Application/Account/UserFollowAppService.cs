using MealPlannerAPI.Dashboard;
using MealPlannerAPI.Hubs;
using MealPlannerAPI.Mappings.Users;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.Users;
using MealPlannerAPI.Users.Dtos;
using MealPlannerAPI.Users.Interfaces;
using MealPlannerAPI.Users.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace MealPlannerAPI.Account
{
    [Authorize]
    [RemoteService(false)]
    public class UserFollowAppService : UserAppServiceBase, IUserFollowAppService
    {
        private readonly UserProfileToFollowUserDtoMapper _toFollowDtoMapper;
        private readonly IUserFollowRepository _followRepository;
        private readonly IMealPlannerHubPublisher _hub;
        private readonly IUserNotificationAppService _notificationService;
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        public UserFollowAppService(IIdentityUserRepository identityUserRepository,
                                    UserProfileToFollowUserDtoMapper toFollowDtoMapper,
                                    IUserFollowRepository followRepository,
                                    IMealPlannerHubPublisher hub,
                                    IUserNotificationAppService notificationService,
                                    IMealPlanRepository mealPlanRepository,
                                    IShoppingListRepository shoppingListRepository) : base(identityUserRepository)
        {
            _toFollowDtoMapper = toFollowDtoMapper;
            _followRepository = followRepository;
            _hub = hub;
            _notificationService = notificationService;
            _mealPlanRepository = mealPlanRepository;
            _shoppingListRepository = shoppingListRepository;
        }
        [AllowAnonymous]
        public async Task FillFollowStateAsync(IReadOnlyCollection<CommunityUserDto> dtos)
        {
            if (CurrentUser.Id is not Guid me || dtos.Count == 0) return; // anonymous: both stay false

            var otherIds = dtos.Select(d => d.Id).Where(id => id != me).ToList();
            if (otherIds.Count == 0) return;

            var query = await _followRepository.GetQueryableAsync();
            var rows = await AsyncExecuter.ToListAsync(
                query.Where(f => (f.FollowerId == me && otherIds.Contains(f.FolloweeId))
                              || (f.FolloweeId == me && otherIds.Contains(f.FollowerId)))
                     .Select(f => new { f.FollowerId, f.FolloweeId }));

            var iFollow = rows.Where(r => r.FollowerId == me).Select(r => r.FolloweeId).ToHashSet();
            var followsMe = rows.Where(r => r.FolloweeId == me).Select(r => r.FollowerId).ToHashSet();

            foreach (var dto in dtos)
            {
                dto.IsFollowedByCurrentUser = iFollow.Contains(dto.Id);
                dto.FollowsCurrentUser = followsMe.Contains(dto.Id);
            }
        }

        public async Task FollowAsync(Guid targetUserId)
        {
            var currentUserId = CurrentUser.GetId();

            if (currentUserId == targetUserId)
            {
                throw new BusinessException(MealPlannerAPIDomainErrorCodes.CannotFollowSelf);
            }

            if (await _followRepository.AnyAsync(x => x.FollowerId == currentUserId && x.FolloweeId == targetUserId))
            {
                throw new BusinessException(MealPlannerAPIDomainErrorCodes.AlreadyFollowing);
            }

            if (await _followRepository.AdjustCountAsync(targetUserId, +1, 0) == 0)
            {
                throw new EntityNotFoundException(typeof(UserProfile), targetUserId);
            }

            var currentUser = await GetUserProfileAsync(currentUserId);
            var targetUser = await GetUserProfileAsync(targetUserId); // also throws if target doesn't exist

            await _followRepository.InsertAsync(new UserFollow(GuidGenerator.Create(), currentUserId, targetUserId));
            await _followRepository.AdjustCountAsync(currentUserId, 0, +1);
            
            var currentStats = await BuildStatsAsync(currentUser);
            var targetStats = await BuildStatsAsync(targetUser);
            await _hub.NotifyStatsUpdatedAsync(currentUser.Id, currentStats);
            await _hub.NotifyStatsUpdatedAsync(targetUser.Id, targetStats);

            await NotificationFactory.NewFollower(_notificationService, targetUserId, currentUser.UserName, currentUser.AvatarUrl);
        }

        public async Task<PagedResultDto<FollowUserDto>> GetFollowersAsync(GetFollowListInput input) 
            => await GetFollowListAsync(input, followers: true);

        [AllowAnonymous]
        public Task<PagedResultDto<FollowUserDto>> GetFollowingAsync(GetFollowListInput input)
            => GetFollowListAsync(input, followers: false);
        public Task<bool> IsFollowingAsync(Guid targetUserId)
        {
            var me = CurrentUser.GetId();
            return _followRepository.AnyAsync(x => x.FollowerId == me && x.FolloweeId == targetUserId);
        }
        public async Task UnfollowAsync(Guid targetUserId)
        {
            var currentUserId = CurrentUser.GetId();

            var follow = await _followRepository.FindAsync(
                x => x.FollowerId == currentUserId && x.FolloweeId == targetUserId);

            if (follow == null)
                throw new BusinessException(MealPlannerAPIDomainErrorCodes.AlreadyNotFollowing);

            var currentUser = await GetUserProfileAsync(currentUserId);
            var targetUser = await GetUserProfileAsync(targetUserId);

            await _followRepository.DeleteAsync(follow);

            currentUser.Unfollow();
            targetUser.LoseFollower();

            await IdentityUserRepository.UpdateAsync(currentUser);
            await IdentityUserRepository.UpdateAsync(targetUser, autoSave: true);

            var currentStats = await BuildStatsAsync(currentUser);
            var targetStats = await BuildStatsAsync(targetUser);
            await _hub.NotifyStatsUpdatedAsync(currentUser.Id, currentStats);
            await _hub.NotifyStatsUpdatedAsync(targetUser.Id, targetStats);
        }
        private async Task<PagedResultDto<FollowUserDto>> GetFollowListAsync(GetFollowListInput input, bool followers)
        {
            var query = await _followRepository.GetQueryableAsync();
            query = followers
                ? query.Where(f => f.FolloweeId == input.UserId)   // people who follow UserId
                : query.Where(f => f.FollowerId == input.UserId);  // people UserId follows

            var totalCount = await AsyncExecuter.CountAsync(query);

            var rows = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(f => f.CreationTime)
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount));

            var otherIds = rows.Select(r => followers ? r.FollowerId : r.FolloweeId).ToList();

            var users = (await IdentityUserRepository.GetListByIdsAsync(otherIds))
                .OfType<UserProfile>()
                .ToDictionary(u => u.Id);

            // Which of these users does the viewer follow? (for the Follow/Following button)
            var iFollow = new HashSet<Guid>();
            if (CurrentUser.Id is Guid me && otherIds.Count > 0)
            {
                var myQuery = await _followRepository.GetQueryableAsync();
                iFollow = (await AsyncExecuter.ToListAsync(
                        myQuery.Where(f => f.FollowerId == me && otherIds.Contains(f.FolloweeId))
                               .Select(f => f.FolloweeId)))
                    .ToHashSet();
            }

            var items = new List<FollowUserDto>();
            foreach (var row in rows)
            {
                var otherId = followers ? row.FollowerId : row.FolloweeId;
                if (!users.TryGetValue(otherId, out var user)) continue;

                var dto = _toFollowDtoMapper.Map(user);
                dto.FollowedAt = row.CreationTime;
                items.Add(dto);
            }

            await FillFollowStateAsync(items);
            return new PagedResultDto<FollowUserDto>(totalCount, items);
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
