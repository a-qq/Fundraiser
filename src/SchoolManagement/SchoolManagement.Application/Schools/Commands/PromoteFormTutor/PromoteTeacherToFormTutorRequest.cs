using System;

namespace SchoolManagement.Application.Schools.Commands.PromoteFormTutor
{
    public sealed class PromoteTeacherToFormTutorRequest
    {
        public Guid TeacherId { get; set; }
    }
}