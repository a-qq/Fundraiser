using MediatR;

namespace FundraiserManagement.Application.Common.Interfaces.Mediator
{
    public interface IInternalRequest<out T> : IRequest<T>
    {
    }
}
