using AutoMapper;
using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SchoolManagement.Core.SchoolAggregate.Users.User;

namespace SchoolManagement.Data.Schools.CreateGroup
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Result<GroupDTO, RequestError>>
    {
        private readonly SchoolContext _schoolContext;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;
        public CreateGroupHandler(
            SchoolContext schoolContext,
            IAuthorizationService authorizationService,
            IMapper mapper)
        {
            _schoolContext = schoolContext;
            _authService = authorizationService;
            _mapper = mapper;
        }
        public async Task<Result<GroupDTO, RequestError>> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
        {
            Result<Tuple<School, User>, RequestError> context = await _authService.GetAuthorizationContextAsync(command.SchoolId, command.AuthId);
            if (context.IsFailure)
                return context.ConvertFailure<GroupDTO>();

            School school = context.Value.Item1;
            User currentUser = context.Value.Item2;
            Number number = Number.Create(command.Number).Value;
            Sign sign = Sign.Create(command.Sign).Value;

            Result<Group> groupOrError = currentUser.CreateGroup(number, sign, school);
            if(groupOrError.IsFailure)
                return Result.Failure<GroupDTO, RequestError>(SharedErrors.General.BusinessRuleViolation(groupOrError.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            GroupDTO groupDto = _mapper.Map<GroupDTO>(groupOrError.Value);
            return Result.Success<GroupDTO, RequestError>(groupDto);
        }
    }
}
