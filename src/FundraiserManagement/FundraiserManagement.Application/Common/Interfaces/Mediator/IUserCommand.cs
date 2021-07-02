using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Infrastructure.Errors;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface IUserCommand<T> : IUserRequest<T>, ICommand<Result<T, RequestError>>
    {
    }

    public interface IUserCommand : IUserCommand<Unit>
    { 
    }
}
