using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);

        Task CreateRequestForCommandAsync<T>(Guid id)
            where T : IRequest<Result>;
    }
}
