using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;

namespace FundraiserManagement.Application.Fundraisers.Commands.ProcessPayment
{
    internal sealed class ProcessPaymentCommand : IInternalCommand
    {
        public PaymentId PaymentId { get; }

        public ProcessPaymentCommand(PaymentId paymentId)
        {
            PaymentId = paymentId;
        }
    }

    //internal sealed class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result>
    //{
    //    private readonly IFundraiserRepository _fundraiserRepository;
    //    private readonly IDateTime _dateTime;

    //    public ProcessPaymentCommandHandler(IFundraiserRepository fundraiserRepository, IDateTime dateTime)
    //    {
    //        _fundraiserRepository = fundraiserRepository;
    //        _dateTime = dateTime;
    //    }
    //    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    //    {
    //        var fundraiserOrNone = await _fundraiserRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);
    //        if (fundraiserOrNone.HasNoValue)
    //            return Result.Failure($"{nameof(Fundraiser)} with {nameof(Payment)} (Id: {request.PaymentId}) not found!");

    //        var payment = fundraiserOrNone.Value.Participations
    //            .SelectMany(p => p.Payments)
    //            .Single(p => p.Id == request.PaymentId);

    //    }
    //}
}
