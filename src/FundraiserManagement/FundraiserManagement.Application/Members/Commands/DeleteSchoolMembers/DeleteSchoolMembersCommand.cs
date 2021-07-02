using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.Common.Models;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;

namespace FundraiserManagement.Application.Members.Commands.DeleteSchoolMembers
{
    internal sealed class DeleteSchoolMembersCommand : IInternalCommand
    {
        public SchoolId SchoolId { get; }

        public DeleteSchoolMembersCommand(SchoolId schoolId)
        {
            SchoolId = schoolId;
        }
    }

    internal sealed class DeleteSchoolMembersCommandHandler : IRequestHandler<DeleteSchoolMembersCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IDateTime _dateTimeService;

        public DeleteSchoolMembersCommandHandler(
            IMemberRepository memberRepository,
            IDateTime dateTimeService)
        {
            _memberRepository = memberRepository;
            _dateTimeService = dateTimeService;
        }


        public async Task<Result> Handle(DeleteSchoolMembersCommand request, CancellationToken token)
        {
            var members = await _memberRepository.GetBySchoolIdAsync(request.SchoolId, token);
            var now = _dateTimeService.Now;

            var result = Result.Combine(members.Where(m => !m.DeletedAt.HasValue).Select(m => m.Delete(now)));

            return result;
        }
    }
}