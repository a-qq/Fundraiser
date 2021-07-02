using System;
using CSharpFunctionalExtensions;
using MediatR;

namespace SharedKernel.Infrastructure.Abstractions.Requests
{
    public abstract class IdentifiedCommand<T> : IRequest<Result>
        where T : IRequest<Result>
    {
        public T Command { get; }
        public Guid Id { get; }
        protected IdentifiedCommand(T command, Guid id)
        {
            Command = command;
            Id = id;
        }
    }
}