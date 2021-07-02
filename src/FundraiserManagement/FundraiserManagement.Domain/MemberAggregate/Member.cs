using System;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate.Cards;
using FundraiserManagement.Domain.MemberAggregate.DomainEvents;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.ValueObjects;

namespace FundraiserManagement.Domain.MemberAggregate
{
    public class Member : AggregateRoot<MemberId>
    {
        public GroupId? GroupId { get; private set; }
        public SchoolId SchoolId { get; }
        public Gender Gender { get; }
        public Email Email { get; }
        public SchoolRole Role { get; private set; }
        public bool IsArchived { get; private set; }
        public bool IsTreasurer { get; private set; }
        public bool IsFormTutor { get; private set; }
        public virtual Card Card { get; private set; }
        public string AccountId { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public Member(MemberId memberId, SchoolId schoolId, Gender gender, SchoolRole role, Email email)
            : base(Guard.Against.Default(memberId, nameof(memberId)))
        {
            SchoolId = Guard.Against.Default(schoolId, nameof(schoolId));
            GroupId = null;
            Gender = gender;
            Role = role;
            Email = Guard.Against.Null(email, nameof(email));
            IsArchived = false;
            IsTreasurer = false;
            IsFormTutor = false;
            Card = null;
        }

        protected Member(Email email)
        {
            Email = email;
        }

        public Result SetCard(Card card)
        {
            if (IsArchived)
                return Result.Failure<Card>($"Member (Id: {Id}) cannot be archived in order to set card!");

            Card = Guard.Against.Null(card, nameof(card));

            return Result.Success();
        }

        public Result SetAccountId(string accountId)
        {
            var validation = CanSetAccountId();
            if (validation.IsFailure)
                return validation;

            AccountId = Guard.Against.NullOrWhiteSpace(accountId, nameof(accountId));

            return Result.Success();
        }

        public Result CanSetAccountId()
        {
            if (!string.IsNullOrWhiteSpace(AccountId))
                return Result.Failure($"Member (Id: {Id}) already has payment account set!");

            //if (Role == SchoolRole.Headmaster)
            //    return Result.Failure($"Headmaster (Id: {Id}) should use school account!");

            if (!IsFormTutor && !IsFormTutor && Role != SchoolRole.Teacher)
                return Result.Failure($"Member (Id: {Id}) not authorized to have payment account!");

            return Result.Success();
        }

        public Result PromoteToHeadmaster()
        {
            if (Role == SchoolRole.Headmaster)
                return Result.Failure($"Member (Id: {Id}) is already a {SchoolRole.Headmaster}!");

            if (IsArchived)
                return Result.Failure($"Member (Id: {Id}) cannot be archived!");

            if (Role != SchoolRole.Teacher)
            {
                return Result.Failure(
                    $"Member (Id: {Id}) must be a {SchoolRole.Teacher} to be promoted to the {SchoolRole.Headmaster}!");
            }

            Role = SchoolRole.Headmaster;

            AddDomainEvent(new HeadmasterPromotedDomainEvent(Id, SchoolId));

            return Result.Success();
        }

        public Result DivestFromHeadmaster()
        {
            if (Role != SchoolRole.Headmaster)
                return Result.Failure($"Member (Id: {Id}) is not a {SchoolRole.Headmaster}!");

            Role = SchoolRole.Teacher;
            AddDomainEvent(new MemberPermissionsDowngradedDomainEvent(Id, SchoolId, true));
            return Result.Success();
        }

        public Result PromoteToTreasurer()
        {
            if (Role != SchoolRole.Student)
                return Result.Failure($"Member (Id: {Id}) is not a {SchoolRole.Student}!");

            if (GroupId is null)
                return Result.Failure($"Member (Id: {Id}) does not belong to group!");

            if (IsTreasurer)
                return Result.Failure($"Member (Id: {Id}) is already a treasurer!");

            if (IsArchived)
                return Result.Failure($"Member (Id: {Id}) cannot be archived!");

            IsTreasurer = true;

            return Result.Success();
        }

        public Result DivestTreasurer()
        {
            if (!IsTreasurer)
                return Result.Failure($"Member (Id: {Id}) is not a treasurer!");

            IsTreasurer = false;

            AddDomainEvent(new MemberPermissionsDowngradedDomainEvent(Id, SchoolId));

            return Result.Success();
        }

        public Result PromoteToFormTutor(GroupId groupId)
        {
            if (Role != SchoolRole.Teacher)
                return Result.Failure($"Member (Id: {Id}) is not a {SchoolRole.Teacher}!");

            if (IsFormTutor)
                return Result.Failure($"Member (Id: {Id}) is already a form tutor!");

            if (IsArchived)
                return Result.Failure($"Member (Id: {Id}) cannot be archived!");

            GroupId = Guard.Against.Default(groupId, nameof(groupId));
            IsFormTutor = true;

            return Result.Success();
        }

        public Result DivestFormTutor()
        {
            if (!IsFormTutor)
                return Result.Failure($"Member (Id: {Id}) is not a form tutor!");

            IsFormTutor = false;
            GroupId = null;

            AddDomainEvent(new MemberPermissionsDowngradedDomainEvent(Id, SchoolId));

            return Result.Success();
        }

        public Result Archive()
        {
            if (IsArchived)
                return Result.Failure($"Member (Id: {Id}) is already archived!");

            if (Role == SchoolRole.Headmaster)
                return Result.Failure($"Member (Id: {Id}) is headmaster!");

            if(DeletedAt.HasValue)
                return Result.Failure($"Member (Id: {Id}) is marked as deleted!");

            if (IsTreasurer || Role == SchoolRole.Teacher)
                AddDomainEvent(new MemberPermissionsDowngradedDomainEvent(Id, SchoolId));

            Card = null;
            IsArchived = true;
            IsFormTutor = false;
            IsTreasurer = false;
            GroupId = null;

            return Result.Success();
        }

        public Result Restore()
        {
            if (!IsArchived)
                return Result.Failure($"Member (Id: {Id}) is not archived!");

            if (DeletedAt.HasValue)
                return Result.Failure($"Member (Id: {Id}) was deleted at {DeletedAt}!");

            IsArchived = false;

            return Result.Success();
        }

        public Result EnrollToGroup(GroupId groupId)
        {
            if (Role != SchoolRole.Student)
                return Result.Failure($"Member (Id: {Id}) is not a {SchoolRole.Student}!");

            if (!(GroupId is null))
                return Result.Failure($"Member (Id: {Id}) already belongs to group (Id: {GroupId})");

            GroupId = Guard.Against.Default(groupId, nameof(groupId));

            return Result.Success();
        }

        public Result DisenrollFromGroup()
        {
            if (GroupId is null)
                return Result.Failure($"Member (Id: {Id}) does not belong to any group!");

            if (Role != SchoolRole.Student)
                return Result.Failure($"Member (Id: {Id}) is not a {SchoolRole.Student}!");

            GroupId = null;
            IsTreasurer = false;

            return Result.Success();
        }

        public Result Delete(DateTime now)
        {
            if (DeletedAt.HasValue)
                return Result.Failure($"Member (Id: {Id}) already deleted!");

            if (IsTreasurer || Role == SchoolRole.Teacher)
                AddDomainEvent(new MemberPermissionsDowngradedDomainEvent(Id, SchoolId));

            Card = null;
            IsFormTutor = false;
            IsTreasurer = false;
            GroupId = null;
            DeletedAt = now;

            return Result.Success();
        }
    }
}