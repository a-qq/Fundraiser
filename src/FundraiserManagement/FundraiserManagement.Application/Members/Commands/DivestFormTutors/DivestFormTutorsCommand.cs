using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Members.Commands.DivestFormTutor;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.Commands.DivestFormTutors
{
    internal sealed class DivestFormTutorsCommand : IInternalCommand
    {
        public DivestFormTutorsCommand(IReadOnlyCollection<MemberId> formTutorIds)
        {
            FormTutorIds = formTutorIds;
        }

        public IReadOnlyCollection<MemberId> FormTutorIds { get; } 
    }

    internal sealed class DivestFormTutorsCommandHandler : IRequestHandler<DivestFormTutorsCommand, Result>
    {
        private readonly ISender _mediator;

        public DivestFormTutorsCommandHandler(ISender mediator)
        {
            _mediator = mediator;
        }


        public async Task<Result> Handle(DivestFormTutorsCommand request, CancellationToken token)
        {
            
            var results = await Task.WhenAll(request.FormTutorIds
                .Select(x =>  _mediator.Send(new DivestFormTutorCommand(x), token)));

            return Result.Combine(results);
        }
    }
}