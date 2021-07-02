using CSharpFunctionalExtensions;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface IInternalQuery<T> : IInternalRequest<Maybe<T>>, IQuery<Maybe<T>>
    {
    }
}
