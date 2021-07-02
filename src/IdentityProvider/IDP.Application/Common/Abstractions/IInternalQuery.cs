using CSharpFunctionalExtensions;

namespace IDP.Application.Common.Abstractions
{
    public interface IInternalQuery<T> : IInternalRequest<Maybe<T>>, IQuery<Maybe<T>>
    {
    }
}
