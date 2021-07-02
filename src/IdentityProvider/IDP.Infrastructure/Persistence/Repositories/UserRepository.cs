using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Utils;

namespace IDP.Infrastructure.Persistence.Repositories
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

        public async Task<Maybe<User>> GetUserByEmailAsync(Email email, CancellationToken token = default)
        {
            Guard.Against.Null(email, nameof(email));

            return Maybe<User>.From(await _dbSet
                .Include(u => u.Claims)
                .Include(u => u.SecurityCode)
                .FirstOrDefaultAsync(u => u.Email == email, token));
        }

        public async Task<Maybe<User>> GetUserBySubjectAsync(Subject subject, CancellationToken token = default)
        {
            Guard.Against.Null(subject, nameof(subject));

            var memberOrNone = Maybe<User>.From(_cache.Get<User>(SchemaNames.Authentication + subject));

            if (memberOrNone.HasNoValue)
                memberOrNone = Maybe<User>.From(await _dbSet
                    .Include(u => u.Claims)
                    .FirstOrDefaultAsync(u => u.Subject == subject, token));

            return memberOrNone;
        }

        public async Task<IReadOnlyCollection<User>> GetUsersBySubjectsAsync(IEnumerable<Subject> subjects, CancellationToken token = default)
        {
            return await _dbSet
                .Include(u => u.Claims)
                .Where(u => subjects.Contains(u.Subject))
                .ToListAsync(token);
        }

        public async Task<Maybe<User>> GetUserBySecurityCodeAsync(string securityCode, CancellationToken token = default)
        {
            Guard.Against.NullOrWhiteSpace(securityCode, nameof(securityCode));

            if (SecurityCode.ValidateCode(securityCode).IsFailure)
                return Maybe<User>.None;

            return await _dbSet
                .Include(u => u.Claims)
                .SingleOrDefaultAsync(u => u.SecurityCode.Value == securityCode, token);
        }

        public async Task<IReadOnlyCollection<User>> GetUsersByClaimValue(string claimType, string claimValue, CancellationToken token = default)
        {
            Guard.Against.NullOrWhiteSpace(claimType, nameof(claimType));
            Guard.Against.NullOrWhiteSpace(claimValue, nameof(claimValue));

            return await _dbSet
                .Include(u => u.Claims)
                .Where(c => c.Claims.Any(c => c.Type == claimType && c.Value == claimValue))
                .ToListAsync(token);
        }

        public void Add(User user)
        {
            Guard.Against.Null(user, nameof(user));

            _dbSet.Add(user);
        }

        public void Remove(User user)
        {
            Guard.Against.Null(user, nameof(user));

            _dbSet.Remove(user);
        }

        public void Remove(IReadOnlyCollection<User> users)
        {
            Guard.Against.NullOrEmpty(users, nameof(users));

            var i = 0;

            foreach (var user in users)
            {
                Guard.Against.Null(user, $"{nameof(users)}[{i}]");
                i += i;
            }

            _dbSet.RemoveRange(users);
        }
    }
}