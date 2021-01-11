using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using IDP.Core.Interfaces;
using IDP.Core.UserAggregate.Entities;
using IDP.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly DbSet<User> _dbSet;

        public UserRepository(IdentityDbContext dbContext)
        {
            _dbSet = dbContext.Users;
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

            return Maybe<User>.From(await _dbSet
                .Include(u => u.Claims)
                .FirstOrDefaultAsync(u => u.Subject == subject));
        }

        public async Task<Maybe<User>> GetUserBySecurityCodeAsync(string securityCode)
        {
            if (securityCode == null)
                throw new ArgumentNullException(nameof(securityCode));

            return Maybe<User>.From(await _dbSet
                .Include(u => u.Claims)
                .FirstOrDefaultAsync(u => u.SecurityCode.Value == securityCode));
        }
    }
}
