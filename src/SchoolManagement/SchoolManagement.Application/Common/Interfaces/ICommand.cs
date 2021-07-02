using CSharpFunctionalExtensions;
using MediatR;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface ICommand<out TResult> : IRequest<TResult>
        where TResult : IResult
    {
    }
}