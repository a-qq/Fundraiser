using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Infrastructure.Errors;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface IUserRequest<T> : IRequest<Result<T, RequestError>>
    {
    }
}
