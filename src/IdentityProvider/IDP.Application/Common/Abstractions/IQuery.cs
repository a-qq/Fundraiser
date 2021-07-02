using MediatR;

namespace IDP.Application.Common.Abstractions
{
    public interface IQuery<out T> : IRequest<T>
    {
    }
}
