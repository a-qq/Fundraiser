using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Participations;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using FundraiserManagement.Domain.MemberAggregate;
using SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.DomainEvents;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.Errors;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers
{
    public class Fundraiser : AggregateRoot<FundraiserId>
    {
        private readonly List<Participation> _participations = new List<Participation>();

        public Goal Goal { get; private set; }
        public Name Name { get; private set; }
        public GroupId? GroupId { get; private set; }
        public SchoolId SchoolId { get; }
        public Member Manager { get; private set; } //formtutor can set himself or treasurer from his group
        //headmaster can set as treasurer any teacher
        //treasurer with active status fundraisings and >1 payment cannnot be archived/expelled/divested !!!!!!
        public Description Description { get; private set; }
        public Range Range { get; private set; }
        public State State { get; private set; } //fundraising cannot begin if treasurer does not have card set
        public Type Type { get; private set; } //only girls / boys can be created only for groupwide range:
        // only by opposite gender if group formtutor gender != treasurer gender
        // by treasurer&&formtutor for not matching gender if sex of formtutor&&treasurer are the same
        // teachers day created only by treasurer

        public IReadOnlyList<Participation> Participations => _participations.AsReadOnly();
        //treasurer can only add people from group his group
        //formtutor can add anyone to participations

        private Fundraiser(Name name, Description description, Goal goal, SchoolId schoolId,
            GroupId? groupId, Member manager, Range range, Type type)
            : base(FundraiserId.New())
        {
            Goal = Guard.Against.Null(goal, nameof(goal));
            Name = Guard.Against.Null(name, nameof(name));
            GroupId = groupId;
            SchoolId = schoolId;
            Manager = Guard.Against.Null(manager, nameof(manager));
            Description = Guard.Against.Null(description, nameof(description));
            Range = range;
            State = State.Preparation;
            Type = type;
        }

        protected Fundraiser() { }

        public decimal? GetStake()
        {
            if (Goal.IsShared || _participations.Count == 0)
                return null;

            return decimal.Round(Goal.Value / _participations.Count, 2, MidpointRounding.AwayFromZero);
        }

        public static Result<Fundraiser, Error> Create(Name name, Description description, Goal goal, SchoolId schoolId,
            GroupId? groupId, Member manager, Range range, Type type)
        {
            if (Validate(groupId, range, type).IsFailure)
                throw new InvalidOperationException(nameof(Create));

            var validation = CanBeManager(groupId, schoolId, manager, range, type);

            if (validation.IsFailure)
                return validation.ConvertFailure<Fundraiser>();

            return new Fundraiser(name, description, goal, schoolId, groupId, manager, range, type);
        }

        public static Result<bool, Error> Validate(Guid? groupId, Range range, Type type,
            string groupIdPN = nameof(GroupId), string rangePN = nameof(Range), string typePN = nameof(Type))
        {
            var result = Result.Success<bool, Error>(true);
            if (groupId.HasValue)
            {
                if (range == Range.Intraschool)
                    result = new Error($"Fundraising with {range} {rangePN.ToLower()} cannot have {groupIdPN} set!");
            }
            else
            {
                if (range != Range.Intraschool)
                    result = new Error($"Fundraising with {range} {rangePN.ToLower()} must have {groupIdPN} set!");
            }

            if (type != Type.Normal && range != Range.Intragroup)
            {
                result = Result.Combine(result,
                    new Error($"Fundraising with {type} {typePN.ToLower()} must be in {Range.Intergroup} {rangePN.ToLower()}!"));
            }

            return result;
        }

        private static Result<bool, Error> CanBeManager(GroupId? groupId, SchoolId schoolId, Member manager, Range range, Type type)
        {
            Guard.Against.Null(manager, nameof(manager));

            if (manager.SchoolId != schoolId)
                throw new InvalidOperationException(nameof(CanBeManager));

            var result = Result.Success<bool, Error>(true);

            if (manager.IsArchived)
                result = new Error("Manager can't be an archived member!");

            if (groupId.HasValue)
            {
                if (manager.GroupId != groupId || !(manager.IsTreasurer || manager.IsFormTutor))
                    result = Result.Combine(result, new Error($"Manager must belong to the set group (Id: '{groupId}')!"));

                if (!manager.IsTreasurer && !manager.IsFormTutor)
                    result = Result.Combine(result, new Error("Manager must be in form tutor or treasurer role!"));

                if (type == Type.TeacherDay && manager.IsFormTutor)
                    result = Result.Combine(result, new Error("Teacher's day fundraiser can only be managed by treasurer!"));

                if (type == Type.MenDay && manager.Gender != Gender.Female)
                    result = Result.Combine(result, new Error("Man day fundraiser can only be managed by a female"!));

                if (type == Type.WomanDay && manager.Gender != Gender.Male)
                    result = Result.Combine(result, new Error("Woman day fundraiser can only be managed by a male!"));


            }
            else
            {
                if (manager.Role == SchoolRole.Student)
                    result = Result.Combine(result, new Error($"Manager must be at least teacher for {range} range fundraiser!"));
            }

            return result;
        }

        public Result Open()
        {
            var validation = CanBeOpened();

            if (validation.IsFailure)
                return validation;

            State = State.Open;

            return Result.Success();
        }

        public Result<bool, Error> RequestOpening()
        {
            var validation = CanBeOpened(checkManagerAccount: false);

            if (validation.IsFailure)
                return new Error(validation.Error);

            if (Manager.CanSetAccountId().IsSuccess)
                AddDomainEvent(new FundraiserOpeningRequestedDomainEvent(Id, SchoolId));

            else State = State.Open;

            return Result.Success<bool, Error>(true);
        }

        internal Result CanBeOpened(bool checkManagerAccount = true)
        {
            if (!(State == State.Preparation || State == State.Stopped))
                return Result.Failure($"Fundraising can't be opened from {State} state!");

            if (Manager is null)
                return Result.Failure("Manager must be set!");

            if (checkManagerAccount && Manager.CanSetAccountId().IsSuccess)
                return Result.Failure("Manager must have payment account set!");

            return Result.Success();
        }

        public Result<bool, Error> Edit(Name name, Description description, Goal goal, GroupId? groupId, Range range, Type type)
        {
            var validation = CanBeEdited(name, description, goal, groupId, range, type);
            if (validation.IsFailure)
                return validation.Error;

            validation = CanBeManager(groupId, SchoolId, Manager, range, type);
            if (validation.IsFailure)
                return validation.Error;

            Name = name;
            Description = description;
            Goal = goal;
            GroupId = groupId;
            Range = range;
            Type = type;

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> Edit(Name name, Description description, Goal goal, GroupId? groupId, Range range, Type type, Member manager)
        {
            var validation = CanBeEdited(name, description, goal, groupId, range, type);
            if (validation.IsFailure)
                return validation.Error;

            validation = CanChangeManagerTo(manager, groupId, SchoolId, range, type);
            if (validation.IsFailure)
                return validation.Error;

            Name = name;
            Description = description;
            Goal = goal;
            GroupId = groupId;
            Range = range;
            Type = type;
            ChangeManager(manager);

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> CanBeEdited(Name name, Description description, Goal goal, GroupId? groupId, Range range, Type type)
        {
            Guard.Against.Null(name, nameof(name));
            Guard.Against.Null(description, nameof(description));
            Guard.Against.Null(goal, nameof(goal));

            if (Validate(groupId, range, type).IsFailure)
                throw new InvalidOperationException(nameof(CanBeEdited));

            if (State != State.Preparation && (Name != name || Description != description || Goal != goal ||
                                               GroupId != groupId || Range != range || Type != type))
            {
                return new Error($"Fundraiser can only be edited when it's in {State.Preparation} state!");
            }

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> RequestManagerChange(Member manager)
        {
            var validation = CanChangeManagerTo(manager);
            if (validation.IsFailure)
                return validation.Error;

            if (State.HasFlag(State.Suspended) && !(Manager is null) || State == State.Stopped)
            {
                AddDomainEvent(new ManagerChangeRequestedDomainEvent(SchoolId, Id, manager.Id));
                return Result.Success<bool, Error>(true);
            }

            Manager = manager;
            if (State.HasFlag(State.Suspended))
                State &= ~State.Suspended;

            return Result.Success<bool, Error>(true);
        }

        public Result ChangeManager(Member manager)
        {
            var validation = CanChangeManagerTo(manager);
            if (validation.IsFailure)
                return Result.Failure(string.Join("\n", validation.Error));

            Manager = manager;
            if (State.HasFlag(State.Suspended))
                State &= ~State.Suspended;

            return Result.Success();
        }

        internal Result<bool, Error> CanChangeManagerTo(Member manager, GroupId? groupId, SchoolId schoolId,
            Range range, Type type)
        {
            Guard.Against.Null(manager, nameof(manager));

            if (Manager == manager && !State.HasFlag(State.Suspended))
                throw new InvalidOperationException(nameof(ChangeManager));

            if (!(State == State.Preparation || State == State.Stopped || State.HasFlag(State.Suspended)))
                return new Error($"Manager cannot be changed in {State} state!");


            var validation = CanBeManager(groupId, schoolId, manager, range, type);
            if (validation.IsFailure)
                return validation.Error;

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> CanChangeManagerTo(Member manager)
            => CanChangeManagerTo(manager, GroupId, SchoolId, Range, Type);

        public void Suspend(bool wasManagerHeadmaster)
        {
            if (wasManagerHeadmaster)
                Manager = null;

            if (State == State.Preparation)
            {
                Manager = null;
                return;
            }

            if (State == State.ResourcesPayedOut)
                throw new InvalidOperationException(nameof(Suspend));

            if (!State.HasFlag(State.Suspended))
                State |= State.Suspended;
        }

        public Result Stop()
        {
            if (State == State.Stopped)
                return Result.Failure("Fundraising is already stopped!");

            if (State != State.Open)
                return Result.Failure("Fundraising must be in opened state to be stopped!");

            State = State.Stopped;

            return Result.Success();
        }

        public void CancelAllProcessingPayments(DateTime now)
        {
            if (!State.HasFlag(State.Suspended))
                throw new InvalidOperationException(nameof(CancelAllProcessingPayments));

            foreach (var participation in Participations)
                participation.CancelAllProcessingPayments(now);
        }

        public Result Resume()
        {
            if (State != State.Stopped)
                return Result.Failure("Fundraising is not stopped!");

            State = State.Open;

            return Result.Success();
        }

        public Result Delete()
        {
            if (State != State.Preparation)
                return Result.Failure("Fundraising can be deleted in preparation phase!");

            _participations.Clear();

            return Result.Success();
        }

        public Result<bool, Error> AddParticipant(Member participant)
        {
            Guard.Against.Null(participant, nameof(participant));

            if (participant.SchoolId != SchoolId)
                throw new InvalidOperationException(nameof(AddParticipant));

            if (State != State.Preparation)
                return new Error("Participants can only be added during preparation sate!");

            if (_participations.Any(p => p.Participant == participant))
                return new Error($"Member (Id: '{participant.Id}') already participates is this fundraising!");

            if (Range == Range.Intergroup && participant.GroupId != GroupId)
                return new Error($"Member (Id: '{participant.Id}') does not belong to group (Id: '{GroupId}')!");

            if (Type == Type.MenDay && participant.Gender != Gender.Female)
                return new Error($"Member (Id: '{participant.Id}' is {participant.Gender} and cannot participate in {Type.MenDay} fundraiser!");

            if (Type == Type.WomanDay && participant.Gender != Gender.Male)
                return new Error($"Member (Id: '{participant.Id}' is {participant.Gender} and cannot participate in {Type.WomanDay} fundraiser!"); ;

            _participations.Add(new Participation(this, participant));

            return Result.Success<bool, Error>(true);
        }

        public Result RemoveParticipant(Member participant)
        {
            if (State != State.Preparation)
                return Result.Failure("Participants can be removed only during preparation phase!");

            var participation = _participations.SingleOrDefault(p => p.Participant == participant);
            if (participation is null)
                return Result.Failure($"Member {participant.Id} is not participating in this fundraising!");

            _participations.Remove(participation);

            return Result.Success();
        }

        public Result<PaymentId, Error> SavePayment(Participation participation, Amount amount, bool inCash, DateTime now)
        {
            Guard.Against.Null(participation, nameof(participation));

            if (participation.Fundraising != this || !_participations.Contains(participation))
                throw new InvalidOperationException(nameof(SavePayment));

            var result = participation.SavePayment(amount, inCash, now);

            if (result.IsSuccess && !inCash)
                AddDomainEvent(new OnlinePaymentSavedDomainEvent(result.Value));

            return result;
        }

        public void ChangePaymentStatus(ParticipationId participationId, PaymentId paymentId, bool isSuccessful, DateTime now)
        {
            var payment = _participations.Single(p => p.Id == participationId)
                    .Payments.Single(p => p.Id == paymentId);

            if (isSuccessful)
            {
                payment.Accept(now);

                if (Balance > Goal && AllPaymentsProcessed)
                    State = State.GoalReached;
            }

            else payment.Decline(now);
        }

        internal decimal GetEstimatedBalance()
            => _participations.SelectMany(p => p.Payments)
             .Where(p => p.Status != Status.Failed)
             .Sum(p => p.Amount);

        internal decimal Balance
            => _participations.SelectMany(p => p.Payments)
                .Where(p => p.Status == Status.Succeeded)
                .Sum(p => p.Amount);

        internal bool AllPaymentsProcessed
            => _participations.SelectMany(p => p.Payments)
                .All(p => p.Status != Status.Processing);
    }
}
