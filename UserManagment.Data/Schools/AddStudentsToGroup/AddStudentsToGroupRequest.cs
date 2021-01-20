using System;
using System.Collections.Generic;

namespace SchoolManagement.Data.Schools.AddStudentsToGroup
{
    public class AddStudentsToGroupRequest
    {
        public IEnumerable<Guid> StudentIds { get; set; }
    }
}
