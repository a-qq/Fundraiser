using CSharpFunctionalExtensions;

namespace IDP.Application.Common.Abstractions
{
    public interface IUserQuery<T> : IUserRequest<T>, IQuery<Result<T>>
    {
    }
}
