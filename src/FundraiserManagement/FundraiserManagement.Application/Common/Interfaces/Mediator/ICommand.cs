using CSharpFunctionalExtensions;
using MediatR;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface ICommand<out TResult> : IRequest<TResult>
        where TResult : IResult
    {
    }
}