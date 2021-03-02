﻿using CSharpFunctionalExtensions;
using IDP.Domain.UserAggregate.Entities;
using SharedKernel.Domain.ValueObjects;
using System.Threading.Tasks;

namespace IDP.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<Maybe<User>> GetUserByEmailAsync(Email email);
        Task<Maybe<User>> GetUserBySubjectAsync(string subject);
        Task<Maybe<User>> GetUserBySecurityCodeAsync(string securityCode);
    }
}
