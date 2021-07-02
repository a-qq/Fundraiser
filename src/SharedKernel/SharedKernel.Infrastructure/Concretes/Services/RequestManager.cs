using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Concretes.Models;
using System;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public class RequestManager : IRequestManager
    {
        private readonly DbContext _context;
        private volatile bool disposedValue;

        public RequestManager(DbContext context)
        {
            _context = Guard.Against.Null(context, nameof(context));
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.
                FindAsync<ClientRequest>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
            where T : IRequest<Result>
        {
            var exists = await ExistAsync(id);

            var request = exists
                ? throw new ApplicationException($"Request with {id} already exists")
                : new ClientRequest(id, typeof(T).Name, DateTime.UtcNow);
                
            _context.Add(request);

            await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }


                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}