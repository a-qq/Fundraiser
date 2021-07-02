using MediatR;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IInternalRequest<out T> : IRequest<T>
    {
    }
}
