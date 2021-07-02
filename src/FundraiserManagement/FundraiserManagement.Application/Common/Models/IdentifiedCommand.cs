using System;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using SK = SharedKernel.Infrastructure.Abstractions.Requests;

namespace FundraiserManagement.Application.Common.Models
{
    public class IdentifiedCommand<T> : SK.IdentifiedCommand<T>, ICommand<Result>
        where T : IInternalCommand
    {
        public IdentifiedCommand(T command, Guid id) 
            : base(command, id) { }
    }
}