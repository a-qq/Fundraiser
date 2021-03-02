using System;
using System.Collections.Generic;

namespace SharedKernel.Infrastructure.Options
{
    public sealed class AdministratorsOptions
    {
        public const string Administrators = "Administrators";
        public List<AdministratorDto> Admins { get; set; }
    }

    public sealed class AdministratorDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
