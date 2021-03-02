using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Utils;

namespace IDP.Infrastructure.Persistance.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly IMemoryCache _cache;
        private readonly DbSet<User> _dbSet;

        public UserRepository(
            IdentityDbContext dbContext,
            IMemoryCache memoryCache)
        {
            _dbSet = dbContext.Users;
            _cache = memoryCache;
        }

        public async Task<Maybe<User>> GetUserByEmailAsync(Email email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            return Maybe<User>.From(await _dbSet
                .Include(u => u.Claims)
                .Include(u => u.SecurityCode)
                .FirstOrDefaultAsync(u => u.Email == email));
        }

        public async Task<Maybe<User>> GetUserBySubjectAsync(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));

            var memberOrNone = Maybe<User>.From(_cache.Get<User>(SchemaNames.Authentiaction + subject));

            if (memberOrNone.HasNoValue)
                memberOrNone = Maybe<User>.From(await _dbSet
                    .Include(u => u.Claims)
                    .FirstOrDefaultAsync(u => u.Subject == subject));

            return memberOrNone;
        }

        public async Task<Maybe<User>> GetUserBySecurityCodeAsync(string securityCode)
        {
            if (string.IsNullOrWhiteSpace(securityCode))
                throw new ArgumentNullException(nameof(securityCode));

            if (!SecurityCode.IsCodeValid(securityCode))
                return Maybe<User>.None;

            return Maybe<User>.From(await _dbSet
                .Include(u => u.Claims)
                .SingleOrDefaultAsync(u => u.SecurityCode.Value == securityCode));
        }

        public async Task<bool> UserExistBySecurityCodeAsync(string securityCode)
        {
            if (!SecurityCode.IsCodeValid(securityCode))
                return false;

            return await _dbSet
                .AnyAsync(u => u.SecurityCode.Value == securityCode);
        }
    }
}