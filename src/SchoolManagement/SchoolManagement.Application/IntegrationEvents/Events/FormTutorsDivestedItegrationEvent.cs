using Ardalis.GuardClauses;
using SchoolManagement.Application.Common.Models;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;
using SchoolManagement.Domain.Common.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class FormTutorsDivestedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<MemberIsActiveModel> FormTutorsData { get; }
        public string RemovedRole { get; }

        public FormTutorsDivestedIntegrationEvent(IEnumerable<MemberIsActiveModel> formTutorsData)
        {
            FormTutorsData = Guard.Against.NullOrEmpty(formTutorsData, nameof(formTutorsData));
            RemovedRole = GroupRoles.FormTutor;
        }
    }
}