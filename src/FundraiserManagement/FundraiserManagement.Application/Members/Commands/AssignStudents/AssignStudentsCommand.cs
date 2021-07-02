using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Members.Commands.AssignStudent;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.AssignStudents
{
    internal sealed class AssignStudentsCommand : IInternalCommand
    {
        public IReadOnlyCollection<MemberId> StudentIds { get; }
        public GroupId GroupId { get; }

        public AssignStudentsCommand(IReadOnlyCollection<MemberId> studentIds, GroupId groupId)
        {
            StudentIds = studentIds;
            GroupId = groupId;
        }
    }
    internal sealed class AssignStudentsCommandHandler : IRequestHandler<AssignStudentsCommand, Result>
    {
        private readonly ISender _mediator;

        public AssignStudentsCommandHandler(ISender mediator)
        {
            _mediator = mediator;
        }
        public async Task<Result> Handle(AssignStudentsCommand request, CancellationToken token)
        {
            var results = await Task.WhenAll(request.StudentIds.Select(x => _mediator.Send(
                new AssignStudentCommand(x, request.GroupId), token)));

            return Result.Combine(results);
        }
    }
}