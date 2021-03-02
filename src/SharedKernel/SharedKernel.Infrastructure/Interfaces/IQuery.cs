using MediatR;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {

    }
}
