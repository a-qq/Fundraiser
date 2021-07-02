using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Options;
using System;
using System.Linq;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public sealed class AdministratorsProvider : IAdministratorsProvider
    {
        private readonly AdministratorsOptions _adminSettings;
        private readonly IMemoryCache _cache;

        public AdministratorsProvider(
            AdministratorsOptions adminSettings,
            IMemoryCache cache)
        {
            _adminSettings = adminSettings;
            _cache = cache;
        }

        public bool ExistById(Guid adminId)
        {
            return GetById(adminId).HasValue;
        }

        public Maybe<Administrator> GetById(Guid adminId)
        {
            if (!_cache.TryGetValue(adminId, out Administrator admin))
            {
                var adminData = _adminSettings.Admins.SingleOrDefault(x => Guid.Parse(x.Id) == adminId);
                if (adminData != null)
                {
                    var email = Email.Create(adminData.Email).Value;
                    admin = new Administrator(Guid.Parse(adminData.Id), email);
                    _cache.Set(admin.Id, admin);
                }
            }

            return admin ?? Maybe<Administrator>.None;
        }
    }
}