using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Schools.Commands.AddStudentsToGroup;
using SchoolManagement.Application.Schools.Commands.ArchiveMember;
using SchoolManagement.Application.Schools.Commands.ChangeGroupAssignment;
using SchoolManagement.Application.Schools.Commands.CreateGroup;
using SchoolManagement.Application.Schools.Commands.DeleteGroup;
using SchoolManagement.Application.Schools.Commands.DeleteSchool;
using SchoolManagement.Application.Schools.Commands.DisenrollStudentFromGroup;
using SchoolManagement.Application.Schools.Commands.DivestFormTutor;
using SchoolManagement.Application.Schools.Commands.DivestTreasurer;
using SchoolManagement.Application.Schools.Commands.EditSchool.Headmaster;
using SchoolManagement.Application.Schools.Commands.EditSchoolLogo;
using SchoolManagement.Application.Schools.Commands.EnrollMember;
using SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv;
using SchoolManagement.Application.Schools.Commands.ExpellMember;
using SchoolManagement.Application.Schools.Commands.Graduate;
using SchoolManagement.Application.Schools.Commands.PassOnHeadmaster;
using SchoolManagement.Application.Schools.Commands.PromoteFormTutor;
using SchoolManagement.Application.Schools.Commands.PromoteTreasurer;
using SchoolManagement.Application.Schools.Commands.RestoreMember;
using SchoolManagement.Infrastructure.Schools.EnrollMember;
using System;
using System.Threading.Tasks;

namespace Backend.API.Controllers.School
{
    [Route("management/headmaster/")]
    [Authorize("MustBeHeadmaster")]
    [ApiController]
    public class HeadmasterController : MediatrController
    {
        public HeadmasterController() : base() { }

        [HttpPut("edit-info")]
        public async Task<IActionResult> EditSchoolInfo(EditSchoolInfoRequest request)
        {
            var command = new EditSchoolInfoCommand(
                request.Description, request.MaxNumberOfMembersInGroup, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("edit-logo")]
        public async Task<IActionResult> EditSchoolLogo([FromForm] EditSchoolLogoRequest request)
        {
            var command = new EditSchoolLogoCommand(request.Logo, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("headmaster")]
        public async Task<IActionResult> PassOnHeadmasterRole(PassOnHeadmasterRequest request)
        {
            var command = new PassOnHeadmasterCommand(SchoolId, request.TeacherId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("school")]
        public async Task<IActionResult> DeleteSchool()
        {
            var command = new DeleteSchoolCommand(SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPost("members")]
        public async Task<IActionResult> EnrollMember(EnrollMemberRequest request)
        {
            var command = new EnrollMemberCommand(request.FirstName, request.LastName,
                request.Email, request.Role, request.Gender, SchoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);
            //var response = errorResponseOrNull.HasNoValue
            //    ? CreatedAtRoute("GetMember", new { result.Value.Id }, Envelope.Ok(result.Value))
            //    : errorResponseOrNull.Value;

            return response;
        }

        [HttpPost("members/csv")]
        public async Task<IActionResult> EnrollMembersFromCsv([FromForm] EnrollMembersFromCsvRequest request)
        {
            var command = new EnrollMembersFromCsvCommand(request.File, request.Delimiter, SchoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        [HttpPut("members/{memberId}/archive")]
        public async Task<IActionResult> ArchiveMember(Guid memberId)
        {
            var command = new ArchiveMemberCommand(memberId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("members/{memberId}/restore")]
        public async Task<IActionResult> RestoreMember(Guid memberId)
        {
            var command = new RestoreMemberCommand(SchoolId, memberId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("members/{memberId}")]
        public async Task<IActionResult> ExpellMember(Guid memberId)
        {
            var command = new ExpellMemberCommand(memberId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("members/{studentId}/group/{groupId}")]
        public async Task<IActionResult> TransferStudent(Guid studentId, Guid groupId)
        {
            var command = new ChangeGroupAssignmentCommand(studentId, groupId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
        {
            var command = new CreateGroupCommand(request.Number, request.Sign, SchoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        [HttpPost("groups/graduate")]
        public async Task<IActionResult> Graduate()
        {
            var command = new GraduateCommand(SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("groups/{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            var command = new DeleteGroupCommand(groupId, SchoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        [HttpPut("groups/{groupId}/students")]
        public async Task<IActionResult> AddStudentsToGroup(Guid groupId, AddStudentsToGroupRequest request)
        {
            var command = new AddStudentsToGroupCommand(request.StudentIds, SchoolId, groupId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("groups/{groupId}/form-tutor")]
        public async Task<IActionResult> MakeTeacherFormTutor(Guid groupId, PromoteTeacherToFormTutorRequest request)
        {
            var command = new PromoteFormTutorCommand(request.TeacherId, groupId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("groups/{groupId}/form-tutor")]
        public async Task<IActionResult> DivestFormTutor(Guid groupId)
        {
            var command = new DivestFormTutorCommand(groupId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("groups/{groupId}/treasurer")]
        public async Task<IActionResult> PromoteTreasurer(Guid groupId, PromoteTreasurerRequest request)
        {
            var command = new PromoteTreasurerCommand(groupId, request.StudentId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("groups/{groupId}/treasurer")]
        public async Task<IActionResult> DivestTreasurer(Guid groupId)
        {
            var command = new DivestTreasurerCommand(groupId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("groups/{groupId}/students/{studentId}")]
        public async Task<IActionResult> DisenrollStudentFromGroup(Guid groupId, Guid studentId)
        {
            var command = new DisenrollStudentFromGroupCommand(groupId, studentId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }
    }
}
