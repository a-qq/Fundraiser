using System;
using CSharpFunctionalExtensions;
using SharedKernel.Domain.ValueObjects;

namespace SharedKernel.Domain.EnumeratedEntities
{
    public sealed class Administrator : Entity<Guid>
    {
        public static string RoleName = "Administrator";

        public Administrator(Guid id, Email email)
            : base(id)
        {
            Email = email;
        }

        public Email Email { get; }
        public string Password { get; }
    }
}