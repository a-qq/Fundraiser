using CSharpFunctionalExtensions;
using SharedKernel.Domain.EnumeratedEntities;
using System;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface IAdministratorsProvider
    {
        public Maybe<Administrator> GetById(Guid adminId);
        public bool ExistById(Guid adminId);
            
    }
}
