using CSharpFunctionalExtensions;
using System;
using System.Linq;

namespace Fundraiser.SharedKernel.Utils
{
    public class Administrator : Entity<Guid>
    {
        public static readonly Administrator Admin_01 = new Administrator(Guid.Parse("3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407"), Email.Create(Environment.GetEnvironmentVariable("AdminEmail")).Value);
        public static readonly Administrator[] AllAdmins = { Admin_01 };
        public Email Email { get; }

        private Administrator(Guid id, Email email)
            : base(id)
        {
            Email = email;
        }

        public static Administrator FromId(Guid id)
        {
            return AllAdmins.SingleOrDefault(x => x.Id == id);
        }

    }


}
