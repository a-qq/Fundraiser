using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EditSchool.Admin
{
    public class EditSchoolHandler : IRequestHandler<EditSchoolCommand, Result<bool, RequestError>>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public EditSchoolHandler(
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(EditSchoolCommand request, CancellationToken cancellationToken)
        {
            if(request.AuthId != User.Admin.Id)
                throw new UnauthorizedAccessException($"UserId: {request.AuthId}");

            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(nameof(School), request.SchoolId.ToString()));

            Name name = Name.Create(request.Name).Value;
            Description description = Description.Create(request.Description).Value;

            User.Admin.EditSchool(name, description, schoolOrNone.Value);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
