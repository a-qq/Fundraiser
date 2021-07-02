using CSharpFunctionalExtensions;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<Maybe<User>> GetUserByEmailAsync(Email email, CancellationToken token = default);
        Task<Maybe<User>> GetUserBySubjectAsync(Subject subject, CancellationToken token = default);
        Task<IReadOnlyCollection<User>> GetUsersBySubjectsAsync(IEnumerable<Subject> subjects, CancellationToken token = default);
        Task<Maybe<User>> GetUserBySecurityCodeAsync(string securityCode, CancellationToken token = default);
        Task<IReadOnlyCollection<User>> GetUsersByClaimValue(string claimType, string claimValue, CancellationToken token = default);
        void Add(User user);
        void Remove(User user);
        void Remove(IReadOnlyCollection<User> users);
    }
}