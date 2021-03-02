using SharedKernel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IEmailUniquenessChecker
    {
        Task<bool> IsUnique(Email email);
        Task<Tuple<bool, IEnumerable<Email>>> AreUnique(IEnumerable<Email> emails);
    }
}
