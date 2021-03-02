using System;
using System.Collections.Generic;

namespace SchoolManagement.Application.Schools.Commands.AddStudentsToGroup
{
    public sealed class AddStudentsToGroupRequest
    {
        public IEnumerable<Guid> StudentIds { get; set; }
    }
}