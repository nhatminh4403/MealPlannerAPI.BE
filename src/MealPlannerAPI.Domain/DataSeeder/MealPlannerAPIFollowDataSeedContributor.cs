using MealPlannerAPI.Users;
using MealPlannerAPI.Users.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace MealPlannerAPI.DataSeeder
{
    public class MealPlannerAPIFollowDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IUserFollowRepository _userFollowRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IGuidGenerator _guidGenerator;

        public int Order = 5;
        public MealPlannerAPIFollowDataSeedContributor(IUserFollowRepository userFollowRepository,
                                                       IUserProfileRepository userProfileRepository,
                                                       IGuidGenerator guidGenerator)
        {
            _userFollowRepository = userFollowRepository;
            _userProfileRepository = userProfileRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _userFollowRepository.GetCountAsync() > 0) return;

            var userNames = FollowPairs
                .SelectMany(p => new[] { p.Follower, p.Followee })
                .Distinct()
                .ToList();

            var users = (await _userProfileRepository.GetListAsync(u => userNames.Contains(u.UserName)))
                .ToDictionary(u => u.UserName, StringComparer.OrdinalIgnoreCase);

            // Resolve usernames to ids, skipping pairs where a user is missing or it's a self-follow
            var resolved = FollowPairs
                .Where(p => users.ContainsKey(p.Follower)
                         && users.ContainsKey(p.Followee)
                         && p.Follower != p.Followee)
                .Select(p => (FollowerId: users[p.Follower].Id, FolloweeId: users[p.Followee].Id))
                .Distinct()
                .ToList();

            if (resolved.Count == 0) return;

            await _userFollowRepository.InsertManyAsync(
                resolved.Select(r => new UserFollow(_guidGenerator.Create(), r.FollowerId, r.FolloweeId)),
                autoSave: true);

            // Make the denormalized counters match the rows we just inserted
            foreach (var user in users.Values)
            {
                user.Followers = resolved.Count(r => r.FolloweeId == user.Id);
                user.Following = resolved.Count(r => r.FollowerId == user.Id);
            }

            await _userProfileRepository.UpdateManyAsync(users.Values, autoSave: true);
        }
        private static readonly (string Follower, string Followee)[] FollowPairs =
        [
            ("chef_maria",        "healthy_john"),
            ("chef_maria",        "baker_sarah"),
            ("chef_maria",        "vegan_emma"),
            ("healthy_john",      "chef_maria"),
            ("healthy_john",      "vegan_emma"),
            ("baker_sarah",       "chef_maria"),
            ("baker_sarah",       "asian_fusion_mike"),
            ("asian_fusion_mike", "chef_maria"),
            ("asian_fusion_mike", "baker_sarah"),
            ("vegan_emma",        "healthy_john"),
            ("vegan_emma",        "baker_sarah"),
            ("admin",             "chef_maria"),
            ("admin",             "vegan_emma"),
            ("chef_maria",        "admin"),
        ];
    }
}
