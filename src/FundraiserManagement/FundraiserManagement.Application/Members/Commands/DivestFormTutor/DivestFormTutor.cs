using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;

namespace FundraiserManagement.Application.Members.Commands.DivestFormTutor
{
    internal sealed class DivestFormTutorCommand : IInternalCommand
    {
        public DivestFormTutorCommand(MemberId formTutorId)
        {
            FormTutorId = formTutorId;
        }

        public MemberId FormTutorId { get; }
    }

    internal sealed class DivestFormTutorCommandHandler : IRequestHandler<DivestFormTutorCommand, Result>
    {
        private readonly IMemberRepository _memberRepository;

        public DivestFormTutorCommandHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<Result> Handle(DivestFormTutorCommand request, CancellationToken token)
        {
            var memberOrNone = await _memberRepository.GetByIdAsync(request.FormTutorId, token);
            if (memberOrNone.HasNoValue)
                return Result.Failure($"Member (Id:{request.FormTutorId}) not found!");

            //check for any fundraiser with this treasurer, make it suspended

            var result = memberOrNone.Value.DivestFormTutor();

            return result;
        }
    }
}
