using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Infrastructure.Errors;

namespace SharedKernel.Infrastructure.Interfaces
{
    public abstract class CommandRequest<T> : ICommand<Result<T, RequestError>>
    {
    }

    public abstract class CommandRequest : CommandRequest<Unit>
    {
    }
}
