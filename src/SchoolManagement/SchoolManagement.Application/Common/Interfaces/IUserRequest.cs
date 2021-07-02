using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Infrastructure.Errors;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IUserRequest<T> : IRequest<Result<T, RequestError>>
    {
    }
}
