using CSharpFunctionalExtensions;
using MediatR;

namespace IDP.Application.Common.Abstractions
{
    public interface IUserRequest<T> : IRequest<Result<T>>
    {
    }
}