using Fundraiser.SharedKernel.Utils;

namespace SchoolManagement.Core.Interfaces
{
    public interface IEmailUniquenessChecker
    {
        bool IsUnique(Email email);
    }
}
