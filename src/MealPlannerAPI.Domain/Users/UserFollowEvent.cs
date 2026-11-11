using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.Users
{
    public class UserFollowEvent : CreationAuditedEntity<Guid>
    {
        public Guid FollowerId { get; private set; }   // the user who follows
        public Guid FolloweeId { get; private set; }   // the user being followed
        public FollowEventType Type { get; private set; }

        protected UserFollowEvent() { }

        public UserFollowEvent(Guid id, Guid followerId, Guid followeeId, FollowEventType type, DateTime? occurredAt = null) : base(id)
        {
            FollowerId = followerId;
            FolloweeId = followeeId;
            Type = type;
            if (occurredAt.HasValue) CreationTime = occurredAt.Value;
        }
    }
}
