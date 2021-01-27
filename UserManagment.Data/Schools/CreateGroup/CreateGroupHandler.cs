using AutoMapper;
using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.CreateGroup
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Result<GroupDTO, RequestError>>
    {
        private readonly SchoolContext _schoolContext;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;

        public CreateGroupHandler(
            SchoolContext schoolContext,
            ISchoolRepository schoolRepository,
            IAuthorizationService authorizationService,
            IMapper mapper)
        {
            _schoolContext = schoolContext;
            _schoolRepository = schoolRepository;
            _authService = authorizationService;
            _mapper = mapper;
        }

        public async Task<Result<GroupDTO, RequestError>> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(command.SchoolId, command.AuthId, Role.Headmaster);

            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(command.SchoolId);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<GroupDTO, RequestError>(SharedRequestError.General.NotFound(command.SchoolId, nameof(School)));

            Number number = Number.Create(command.Number).Value;
            Sign sign = Sign.Create(command.Sign).Value;

            Result<Group> groupOrError = schoolOrNone.Value.CreateGroup(number, sign);

            if (groupOrError.IsFailure)
                return Result.Failure<GroupDTO, RequestError>(SharedRequestError.General.BusinessRuleViolation(groupOrError.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            GroupDTO groupDto = _mapper.Map<GroupDTO>(groupOrError.Value);
            return Result.Success<GroupDTO, RequestError>(groupDto);
        }
    }
}
