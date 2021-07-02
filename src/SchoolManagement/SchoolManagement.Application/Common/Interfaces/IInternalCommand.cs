using CSharpFunctionalExtensions;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IInternalCommand : IInternalRequest<Result>, ICommand<Result>
    {
    }
}