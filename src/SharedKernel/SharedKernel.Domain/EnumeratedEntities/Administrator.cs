using CSharpFunctionalExtensions;
using SharedKernel.Domain.ValueObjects;
using System;

namespace SharedKernel.Domain.EnumeratedEntities
{
    public sealed class Administrator : Entity<Guid>
    {
        public static string RoleName = "Administrator";
        public Email Email { get; }
        public string Password { get; }

        public Administrator(Guid id, Email email)
            : base(id)
        {
            Email = email;
        }
    }
}
