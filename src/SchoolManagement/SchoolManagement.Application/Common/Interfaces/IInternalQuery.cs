using CSharpFunctionalExtensions;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IInternalQuery<T> : IInternalRequest<Maybe<T>>, IQuery<Maybe<T>>
    {
    }
}
