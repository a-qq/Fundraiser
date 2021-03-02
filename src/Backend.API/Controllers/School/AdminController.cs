using System;
using System.Threading.Tasks;
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
using SchoolManagement.Application.Schools.Commands.DivestHeadmaster;
using SchoolManagement.Application.Schools.Commands.DivestTreasurer;
using SchoolManagement.Application.Schools.Commands.EditSchool.Admin;
using SchoolManagement.Application.Schools.Commands.EditSchoolLogo;
using SchoolManagement.Application.Schools.Commands.EnrollMember;
using SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv;
using SchoolManagement.Application.Schools.Commands.ExpellMember;
using SchoolManagement.Application.Schools.Commands.Graduate;
using SchoolManagement.Application.Schools.Commands.PromoteFormTutor;
using SchoolManagement.Application.Schools.Commands.PromoteHeadmaster;
using SchoolManagement.Application.Schools.Commands.PromoteTreasurer;
using SchoolManagement.Application.Schools.Commands.RegisterSchool;
using SchoolManagement.Application.Schools.Commands.RestoreMember;

namespace Backend.API.Controllers.School
{
    [Authorize("MustBeAdmin")]
    [Route("management/admin/")]
    [ApiController]
    public class AdminController : MediatrController
    {
        [HttpPost("schools")]
        public async Task<IActionResult> RegisterSchool(RegisterSchoolRequest request)

        {
            var command = new RegisterSchoolCommand(request.Name, request.YearsOfEducation, request.HeadmasterFirstName,
                request.HeadmasterLastName, request.HeadmasterEmail, request.HeadmasterGender);

            var result = await Handle(command);
            var response = FromResultOk(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/edit")]
        public async Task<IActionResult> EditSchool(Guid schoolId, EditSchoolRequest request)
        {
            var command = new EditSchoolCommand(
                request.Name, request.Description, request.MaxNumberOfMembersInGroup, schoolId);

            var result = await Handle(command);
            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/edit-logo")]
        public async Task<IActionResult> EditSchoolLogo(Guid schoolId, [FromForm] EditSchoolLogoRequest request)
        {
            var command = new EditSchoolLogoCommand(request.Logo, schoolId);

            var result = await Handle(command);
            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}")]
        public async Task<IActionResult> DeleteSchool(Guid schoolId)
        {
            var command = new DeleteSchoolCommand(schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPost("schools/{schoolId}/headmaster")]
        public async Task<IActionResult> PromoteHeadmaster(Guid schoolId, PromoteHeadmasterRequest request)
        {
            var command = new PromoteHeadmasterCommand(schoolId, request.TeacherId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/headmaster")]
        public async Task<IActionResult> DivestHeadmaster(Guid schoolId)
        {
            var command = new DivestHeadmasterCommand(schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPost("schools/{schoolId}/members")]
        public async Task<IActionResult> EnrollMember(Guid schoolId, EnrollMemberRequest request)

        {
            var command = new EnrollMemberCommand(request.FirstName, request.LastName,
                request.Email, request.Role, request.Gender, schoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        [HttpPost("schools/{schoolId}/members/csv")]
        public async Task<IActionResult> EnrollMembersFromCsv(Guid schoolId,
            [FromForm] EnrollMembersFromCsvRequest request)
        {
            var command = new EnrollMembersFromCsvCommand(request.File, request.Delimiter, schoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/members/{memberId}/archive")]
        public async Task<IActionResult> ArchiveMember(Guid schoolId, Guid memberId)
        {
            var command = new ArchiveMemberCommand(memberId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/members/{memberId}/restore")]
        public async Task<IActionResult> RestoreMember(Guid schoolId, Guid memberId)
        {
            var command = new RestoreMemberCommand(schoolId, memberId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/members/{memberId}")]
        public async Task<IActionResult> ExpellMember(Guid schoolId, Guid memberId)
        {
            var command = new ExpellMemberCommand(memberId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/members/{studentId}/group/{groupId}")]
        public async Task<IActionResult> TransferStudent(Guid schoolId, Guid studentId, Guid groupId)
        {
            var command = new ChangeGroupAssignmentCommand(studentId, groupId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPost("schools/{schoolId}/groups")]
        public async Task<IActionResult> CreateGroup(Guid schoolId, CreateGroupRequest request)
        {
            var command = new CreateGroupCommand(request.Number, request.Sign, schoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        //must be post as effect is not idempotent 
        [HttpPost("schools/{schoolId}/groups/graduate")]
        public async Task<IActionResult> Graduate(Guid schoolId)
        {
            var command = new GraduateCommand(schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);
            return response;
        }

        [HttpPut("schools/{schoolId}/groups/{groupId}/students")]
        public async Task<IActionResult> AddStudentsToGroup(Guid schoolId, Guid groupId,
            AddStudentsToGroupRequest request)
        {
            var command = new AddStudentsToGroupCommand(request.StudentIds, schoolId, groupId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/groups/{groupId}/form-tutor")]
        public async Task<IActionResult> MakeTeacherFormTutor(Guid schoolId, Guid groupId,
            PromoteTeacherToFormTutorRequest request)
        {
            var command = new PromoteFormTutorCommand(request.TeacherId, groupId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/groups/{groupId}/form-tutor")]
        public async Task<IActionResult> DivestFormTutor(Guid schoolId, Guid groupId)
        {
            var command = new DivestFormTutorCommand(groupId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/groups/{groupId}/treasurer")]
        public async Task<IActionResult> PromoteTreasurer(Guid schoolId, Guid groupId, PromoteTreasurerRequest request)
        {
            var command = new PromoteTreasurerCommand(groupId, request.StudentId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/groups/{groupId}/treasurer")]
        public async Task<IActionResult> DivestTreasurer(Guid schoolId, Guid groupId)
        {
            var command = new DivestTreasurerCommand(groupId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/groups/{groupId}/students/{studentId}")]
        public async Task<IActionResult> DisenrollStudentFromGroup(Guid schoolId, Guid groupId, Guid studentId)
        {
            var command = new DisenrollStudentFromGroupCommand(groupId, studentId, schoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/groups/{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid schoolId, Guid groupId)
        {
            var command = new DeleteGroupCommand(groupId, schoolId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }
    }
}