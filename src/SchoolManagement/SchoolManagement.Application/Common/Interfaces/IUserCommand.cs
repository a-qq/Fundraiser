using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Infrastructure.Errors;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IUserCommand<T> : IUserRequest<T>, ICommand<Result<T, RequestError>>
    {
    }

    public interface IUserCommand : IUserRequest<Unit>, ICommand<Result<Unit, RequestError>>
    {
    }
}
