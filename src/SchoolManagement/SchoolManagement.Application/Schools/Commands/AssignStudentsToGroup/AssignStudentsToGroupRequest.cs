using System;
using System.Collections.Generic;

namespace SchoolManagement.Application.Schools.Commands.AssignStudentsToGroup
{
    public sealed class AssignStudentsToGroupRequest
    {
        public IEnumerable<Guid> StudentIds { get; set; }
    }
}