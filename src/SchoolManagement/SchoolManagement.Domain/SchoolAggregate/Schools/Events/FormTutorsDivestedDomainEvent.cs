using Ardalis.GuardClauses;
using SchoolManagement.Domain.Common.Models;
using SharedKernel.Domain.Common;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class FormTutorsDivestedDomainEvent : DomainEvent
    {
        public IEnumerable<MemberIsActiveModel> FormTutorsData { get; }

        internal FormTutorsDivestedDomainEvent(IEnumerable<MemberIsActiveModel> formTutorsData)
        {
            FormTutorsData = Guard.Against.NullOrEmpty(formTutorsData, nameof(formTutorsData));
        }
    }
}