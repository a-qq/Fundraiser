using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Data.Schools.CreateGroup;
using SchoolManagement.Data.Schools.EditSchool.Headmaster;
using SchoolManagement.Data.Schools.EditSchoolLogo;
using SchoolManagement.Data.Schools.EnrollMember;
using System.Threading.Tasks;

namespace Fundraiser.API.Controllers.School
{
    [Route("management/")]
    [ApiController]
    public class SchoolsController : MediatrController
    {

        public SchoolsController(IMediator mediator)
            : base(mediator) {  }
        
        [Authorize("MustBeHeadmaster")]
        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll(EnrollMemberRequest request)
        {
            var command = new EnrollMemberCommand(request.FirstName, request.LastName,
                request.Email, request.Role, request.Gender, AuthId, SchoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);
            //var response = errorResponseOrNull.HasNoValue
            //    ? CreatedAtRoute("GetMember", new { result.Value.Id }, Envelope.Ok(result.Value))
            //    : errorResponseOrNull.Value;

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
        {
            var command = new CreateGroupCommand(request.Number, request.Sign, AuthId, SchoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);
            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpPut("edit-info")]
        public async Task<IActionResult> EditSchoolInfo(EditSchoolInfoRequest request)
        {
            var command = new EditSchoolInfoCommand(request.Description, AuthId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpPut("edit-logo")]
        public async Task<IActionResult> EditSchoolLogo([FromForm] EditSchoolLogoRequest request)
        {
            var command = new EditSchoolLogoCommand(request.Logo, AuthId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

    }
}
