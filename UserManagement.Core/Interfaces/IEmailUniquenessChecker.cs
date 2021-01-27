using Fundraiser.SharedKernel.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagement.Core.Interfaces
{
    public interface IEmailUniquenessChecker
    {
        Task<bool> IsUnique(Email email);
        Task<Tuple<bool, IEnumerable<Email>>> AreUnique(IEnumerable<Email> emails);
    }
}
