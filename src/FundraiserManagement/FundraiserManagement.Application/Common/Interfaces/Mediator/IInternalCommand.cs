using CSharpFunctionalExtensions;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface IInternalCommand : IInternalRequest<Result>, ICommand<Result>
    {
    }
}