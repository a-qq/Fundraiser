using CSharpFunctionalExtensions;
using SharedKernel.Infrastructure.Errors;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface IUserQuery<T> : IUserRequest<T>, IQuery<IResult<T, RequestError>>
    {
    }
}
