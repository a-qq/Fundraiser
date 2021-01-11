using MediatR;
using System;

namespace Fundraiser.SharedKernel.Utils
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }

}
