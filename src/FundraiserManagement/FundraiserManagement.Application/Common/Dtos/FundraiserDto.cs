using System;

namespace FundraiserManagement.Application.Common.Dtos
{
    public sealed class FundraiserDto
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Guid? GroupId { get; private set; }
        public Guid SchoolId { get; private set; }
        public string Range { get; private set; }
        public string State { get; private set; }
        public string Type { get; private set; }
        public decimal Goal { get; private set; }
        public bool IsShared { get; private set; }
        public Guid ManagerId { get; private set; }

    }
}
