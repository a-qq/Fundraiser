﻿using Backend.API.Authorization.Validators.Absrtact;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Members;

namespace Backend.API.Authorization.Validators.Concrete
{
    internal sealed class StudentClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
        IClaimsPrincipalValidator
    {
        public StudentClaimPrincipalValidator(ISender mediator)
            : base(mediator)
        {
        }

        public override string RoleRequirement { get; } = RoleEnum.Student.ToString();
    }
}