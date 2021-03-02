using MediatR;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}