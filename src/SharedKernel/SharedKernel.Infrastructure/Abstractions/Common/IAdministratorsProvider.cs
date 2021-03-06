﻿using System;
using CSharpFunctionalExtensions;
using SharedKernel.Domain.EnumeratedEntities;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IAdministratorsProvider
    {
        public Maybe<Administrator> GetById(Guid adminId);
        public bool ExistById(Guid adminId);
    }
}