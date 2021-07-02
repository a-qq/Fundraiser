using CSharpFunctionalExtensions;
using MediatR;

namespace IDP.Application.Common.Abstractions
{
    public interface ICommand<out TResult> : IRequest<TResult>
        where TResult : IResult
    {
    }
}
