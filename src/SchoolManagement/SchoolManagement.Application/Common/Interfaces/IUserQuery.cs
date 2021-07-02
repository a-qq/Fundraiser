using CSharpFunctionalExtensions;
using SharedKernel.Infrastructure.Errors;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IUserQuery<T> : IUserRequest<T>, IQuery<IResult<T, RequestError>>
    {
    }
}
