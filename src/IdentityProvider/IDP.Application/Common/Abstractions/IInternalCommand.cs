using CSharpFunctionalExtensions;

namespace IDP.Application.Common.Abstractions
{
    public interface IInternalCommand : IInternalRequest<Result>, ICommand<Result>
    {
    }
}