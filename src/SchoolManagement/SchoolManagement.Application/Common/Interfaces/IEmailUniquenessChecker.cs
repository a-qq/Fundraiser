using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.Domain.ValueObjects;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IEmailUniquenessChecker
    {
        Task<bool> IsUnique(Email email);
        Task<Tuple<bool, IEnumerable<Email>>> AreUnique(IReadOnlyCollection<Email> emails);
    }
}