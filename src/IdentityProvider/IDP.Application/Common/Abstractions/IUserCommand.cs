using CSharpFunctionalExtensions;
using MediatR;

namespace IDP.Application.Common.Abstractions
{
    public interface IUserCommand<T> : IUserRequest<T>, ICommand<Result<T>>
    {
    }

    public interface IUserCommand : IUserCommand<Unit>
    {
    }
}