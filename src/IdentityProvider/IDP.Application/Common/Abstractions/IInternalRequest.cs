using MediatR;

namespace IDP.Application.Common.Abstractions
{
    public interface IInternalRequest<out T> : IRequest<T>
    {
    }
}
