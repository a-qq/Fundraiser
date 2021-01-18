using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using System;

namespace SchoolManagement.Data.Schools.EditSchoolLogo
{
    public class EditSchoolLogoCommand : ICommand<Result<bool, RequestError>>
    {
        public IFormFile Logo { get; }
        public Guid AuthId { get; }
        public Guid SchoolId { get; }

        public EditSchoolLogoCommand(IFormFile logo, Guid authId, Guid schoolId)
        {
            Logo = logo;
            AuthId = authId;
            SchoolId = schoolId;
        }
    }
}
