using MediatR;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IQuery<out T> : IRequest<T>
    {
    }
}
