using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.Users
{
    public class UserFollow : CreationAuditedEntity<Guid>
    {
        public Guid FollowerId { get; private set; }   // the user who follows
        public Guid FolloweeId { get; private set; }   // the user being followed

        protected UserFollow() { }

        public UserFollow(Guid id, Guid followerId, Guid followeeId) : base(id)
        {
            if (followerId == followeeId)
                throw new BusinessException(MealPlannerAPIDomainErrorCodes.CannotFollowSelf);

            FollowerId = followerId;
            FolloweeId = followeeId;
        }
    }
}
    