using MediatR;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface IQuery<out T> : IRequest<T>
    {
    }
}
