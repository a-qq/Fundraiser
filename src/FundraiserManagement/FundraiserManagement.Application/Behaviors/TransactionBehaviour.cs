using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;

namespace FundraiserManagement.Application.Behaviors
{
    internal sealed class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
        where TResponse : IResult

    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly IFundraiserContext _context;
        private readonly IIntegrationEventService _integrationEventService;

        public TransactionBehaviour(IFundraiserContext context,
            IIntegrationEventService integrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _context = Guard.Against.Null(context, nameof(context));
            _integrationEventService = Guard.Against.Null(integrationEventService, nameof(IIntegrationEventService)); 
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_context.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _context.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction = await _context.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})",
                            transaction.TransactionId, typeName, request);

                        response = await next();
                        if (response.IsFailure)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            return;
                        }

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}",
                            transaction.TransactionId, typeName);

                        await _context.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    await _integrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}