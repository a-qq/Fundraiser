using System;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using SK = SharedKernel.Infrastructure.Abstractions.Requests;

namespace IDP.Application.Common.Models
{
    public class IdentifiedCommand<T> : SK.IdentifiedCommand<T>, ICommand<Result>
        where T : IInternalCommand
    {
        public IdentifiedCommand(T command, Guid id) 
            : base(command, id) { }
    }
}