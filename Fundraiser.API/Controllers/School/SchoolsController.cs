using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Data.Schools.AddStudentsToGroup;
using SchoolManagement.Data.Schools.ChangeGroupAssignment;
using SchoolManagement.Data.Schools.CreateGroup;
using SchoolManagement.Data.Schools.DeleteGroup;
using SchoolManagement.Data.Schools.DeleteSchool;
using SchoolManagement.Data.Schools.DisenrollStudentFromGroup;
using SchoolManagement.Data.Schools.DivestFormTutor;
using SchoolManagement.Data.Schools.EditSchool.Headmaster;
using SchoolManagement.Data.Schools.EditSchoolLogo;
using SchoolManagement.Data.Schools.EnrollMember;
using SchoolManagement.Data.Schools.EnrollMembersFromCsv;
using SchoolManagement.Data.Schools.ExpellMember;
using SchoolManagement.Data.Schools.MakeTeacherFormTutor;
using System;
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
        [HttpPut("edit-info")]
        public async Task<IActionResult> EditSchoolInfo(EditSchoolInfoRequest request)
        {
            var command = new EditSchoolInfoCommand(
                request.Description, request.MaxNumberOfMembersInGroup, AuthId, SchoolId);

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

        [Authorize("MustBeHeadmaster")]
        [HttpDelete("school")]
        public async Task<IActionResult> DeleteSchool()
        {
            var command = new DeleteSchoolCommand(SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpPost("members")]
        public async Task<IActionResult> EnrollMember(EnrollMemberRequest request)
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
        [HttpPost("members/csv")]
        public async Task<IActionResult> EnrollMembersFromCsv([FromForm] EnrollMembersFromCsvRequest request)
        {
            var command = new EnrollMembersFromCsvCommand(request.File, SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpDelete("members/{memberId}")]
        public async Task<IActionResult> ExpellMember(Guid memberId)
        {
            var command = new ExpellMemberCommand(memberId, SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpPut("members/{studentId}/group/{groupId}")]
        public async Task<IActionResult> TransferStudent(Guid studentId, long groupId)
        {
            var command = new ChangeGroupAssignmentCommand(studentId, groupId, SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

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
        [HttpPut("groups/{groupId}/students")]
        public async Task<IActionResult> AddStudentsToGroup(long groupId, AddStudentsToGroupRequest request)
        {
            var command = new AddStudentsToGroupCommand(request.StudentIds, AuthId, SchoolId, groupId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpPut("groups/{groupId}/form-tutor")]
        public async Task<IActionResult> MakeTeacherFormTutor(long groupId, MakeTeacherFormTutorRequest request)
        {
            var command = new MakeTeacherFormTutorCommand(request.TeacherId, groupId, SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }


        [Authorize("MustBeHeadmaster")]
        [HttpDelete("groups/{groupId}/form-tutor")]
        public async Task<IActionResult> DivestFormTutor(long groupId)
        {
            var command = new DivestFormTutorCommand(groupId, SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpDelete("groups/{groupId}/students/{studentId}")]
        public async Task<IActionResult> DisenrollStudentFromGroup(long groupId, Guid studentId)
        {
            var command = new DisenrollStudentFromGroupCommand(groupId, studentId, SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [Authorize("MustBeHeadmaster")]
        [HttpDelete("groups/{groupId}")]
        public async Task<IActionResult> DeleteGroup(long groupId)
        {
            var command = new DeleteGroupCommand(groupId, SchoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }
    }
}
