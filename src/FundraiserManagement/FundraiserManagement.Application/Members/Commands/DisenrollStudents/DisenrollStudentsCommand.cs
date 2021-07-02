using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Members.Commands.DisenrollStudent;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.DisenrollStudents
{
    internal sealed class DisenrollStudentsCommand : IInternalCommand
    {
        public IReadOnlyCollection<MemberId> StudentIds { get; }

        public DisenrollStudentsCommand(IEnumerable<MemberId> memberIds)
        {
            StudentIds = Guard.Against.NullOrEmpty(memberIds, nameof(memberIds)).ToList();
        }
    }


    internal sealed class DisenrollStudentsCommandHandler : IRequestHandler<DisenrollStudentsCommand, Result>
    {
        private readonly ISender _mediator;

        public DisenrollStudentsCommandHandler(ISender mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Handle(DisenrollStudentsCommand request, CancellationToken token)
        {
            var results = await Task.WhenAll(request.StudentIds.Select(
                x => _mediator.Send(new DisenrollStudentCommand(x), token)));

            return Result.Combine(results);
        }
    }
}