using Ardalis.GuardClauses;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.CreateGroup
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class CreateGroupCommand : IUserCommand<GroupDTO>, ISchoolAuthorizationRequest
    {
        public CreateGroupCommand(int number, string sign, Guid schoolId)
        {
            Number = number;
            Sign = sign;
            SchoolId = schoolId;
        }

        public int Number { get; }
        public string Sign { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Result<GroupDTO, RequestError>>
    {
        private readonly IMapper _mapper;
        private readonly ISchoolRepository _schoolRepository;

        public CreateGroupCommandHandler(
            ISchoolRepository schoolRepository,
            IMapper mapper)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
            _mapper = Guard.Against.Null(mapper, nameof(mapper));
        }

        public async Task<Result<GroupDTO, RequestError>> Handle(CreateGroupCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var number = Number.Create(request.Number).Value;
            var sign = Sign.Create(request.Sign).Value;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var groupOrError = schoolOrNone.Value.CreateGroup(number, sign);

            if (groupOrError.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(groupOrError.Error);

            var groupDto = _mapper.Map<GroupDTO>(groupOrError.Value);

            return groupDto;
        }
    }
}