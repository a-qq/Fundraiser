using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Data.Schools.Commands.RegisterSchool;
using SchoolManagement.Data.Schools.CreateGroup;
using SchoolManagement.Data.Schools.EditSchool.Admin;
using SchoolManagement.Data.Schools.EditSchoolLogo;
using SchoolManagement.Data.Schools.EnrollMember;
using SchoolManagement.Data.Schools.RegisterSchool;
using System;
using System.Threading.Tasks;

namespace Fundraiser.API.Controllers.School
{
    [Authorize("MustBeAdmin")]
    [Route("admin/")]
    [ApiController]
    public class AdminController : MediatrController
    {
        
        public AdminController(IMediator mediator)
            : base(mediator) { }

        
        [HttpPost("schools")]
        public async Task<IActionResult> RegisterSchool(RegisterSchoolRequest request)

        {
            var command = new RegisterSchoolCommand(request.Name, request.HeadmasterFirstName,
                request.HeadmasterLastName, request.HeadmasterEmail, request.HeadmasterGender, AuthId);

            var result = await Handle(command);
            IActionResult response = FromResultOk(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/edit")]
        public async Task<IActionResult> EditSchool(Guid schoolId, EditSchoolRequest request)
        {
            var command = new EditSchoolCommand(request.Description, AuthId, schoolId);

            var result = await Handle(command);
            IActionResult response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/edit-logo")]
        public async Task<IActionResult> EditSchoolLogo(Guid schoolId, [FromForm] EditSchoolLogoRequest request)
        {
            var command = new EditSchoolLogoCommand(request.Logo, AuthId, schoolId);

            var result = await Handle(command);
            IActionResult response = FromResultNoContent(result);

            return response;
        }

        [HttpPost("schools/{schoolId}/members")]
        public async Task<IActionResult> EnrollMember(Guid schoolId, EnrollMemberRequest request)

        {
            var command = new EnrollMemberCommand(request.FirstName, request.LastName,
                request.Email, request.Role, request.Gender, AuthId, schoolId);

            var result = await Handle(command);
            IActionResult response = FromResultOk(result);

            return response;
        }

        [HttpPost("schools/{schoolId}/groups")]
        public async Task<IActionResult> CreateGroup(Guid schoolId, CreateGroupRequest request)
        {
            var command = new CreateGroupCommand(request.Number, request.Sign, AuthId, schoolId);

            var result = await Handle(command);
            IActionResult response = FromResultOk(result);

            return response;
        }

    }
}
