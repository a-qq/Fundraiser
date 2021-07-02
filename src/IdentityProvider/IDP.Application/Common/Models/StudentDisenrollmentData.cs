namespace IDP.Application.Common.Models
{
    public sealed class StudentDisenrollmentData
    {
        public string StudentId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }

        public StudentDisenrollmentData(string studentId, string removedRole, bool isActive)
        {
            StudentId = studentId;
            RemovedRole = removedRole;
            IsActive = isActive;
        }
    }
}