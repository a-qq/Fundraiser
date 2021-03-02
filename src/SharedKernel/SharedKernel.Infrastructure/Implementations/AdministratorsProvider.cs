using System;
using System.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Options;

namespace SharedKernel.Infrastructure.Implementations
{
    public sealed class AdministratorsProvider : IAdministratorsProvider
    {
        private readonly AdministratorsOptions _adminSettings;
        private readonly IMemoryCache _cache;

        public AdministratorsProvider(
            IOptions<AdministratorsOptions> adminSettings,
            IMemoryCache cache)
        {
            _adminSettings = adminSettings.Value;
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