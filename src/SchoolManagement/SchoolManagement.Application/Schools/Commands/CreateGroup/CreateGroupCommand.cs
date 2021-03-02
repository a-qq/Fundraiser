using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.CreateGroup
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class CreateGroupCommand : CommandRequest<GroupDTO>
    {
        public int Number { get; }
        public string Sign { get; }
        public Guid SchoolId { get; }

        public CreateGroupCommand(int number, string sign, Guid schoolId)
        {
            Number = number;
            Sign = sign;
            SchoolId = schoolId;
        }
    }

    internal sealed class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Result<GroupDTO, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;

        public CreateGroupHandler(
            ISchoolContext schoolContext,
            ISchoolRepository schoolRepository,
            IMapper mapper)
        {
            _context = schoolContext;
            _schoolRepository = schoolRepository;
            _mapper = mapper;
        }

        public async Task<Result<GroupDTO, RequestError>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            Number number = Number.Create(request.Number).Value;
            Sign sign = Sign.Create(request.Sign).Value;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);

            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var groupOrError = schoolOrNone.Value.CreateGroup(number, sign);

            if (groupOrError.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(groupOrError.Error);

            await _context.SaveChangesAsync(cancellationToken);

            var groupDto = _mapper.Map<GroupDTO>(groupOrError.Value);

            return groupDto;
        }
    }
}
