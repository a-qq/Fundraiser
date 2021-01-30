using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Data.Schools.AddStudentsToGroup;
using SchoolManagement.Data.Schools.Commands.RegisterSchool;
using SchoolManagement.Data.Schools.CreateGroup;
using SchoolManagement.Data.Schools.DeleteGroup;
using SchoolManagement.Data.Schools.DeleteSchool;
using SchoolManagement.Data.Schools.DisenrollStudentFromGroup;
using SchoolManagement.Data.Schools.DivestFormTutor;
using SchoolManagement.Data.Schools.EditSchool.Admin;
using SchoolManagement.Data.Schools.EditSchoolLogo;
using SchoolManagement.Data.Schools.EnrollMember;
using SchoolManagement.Data.Schools.EnrollMembersFromCsv;
using SchoolManagement.Data.Schools.ExpellMember;
using SchoolManagement.Data.Schools.MakeTeacherFormTutor;
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
            var command = new RegisterSchoolCommand(request.Name, request.YearsOfEducation, request.HeadmasterFirstName,
                request.HeadmasterLastName, request.HeadmasterEmail, request.HeadmasterGender, AuthId);

            var result = await Handle(command);
            IActionResult response = FromResultOk(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/edit")]
        public async Task<IActionResult> EditSchool(Guid schoolId, EditSchoolRequest request)
        {
            var command = new EditSchoolCommand(
                request.Name, request.Description, request.MaxNumberOfMembersInGroup, AuthId, schoolId);

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

        [HttpDelete("schools/{schoolId}")]
        public async Task<IActionResult> DeleteSchool(Guid schoolId)
        {
            var command = new DeleteSchoolCommand(schoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

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

        [HttpPost("schools/{schoolId}/members/csv")]
        public async Task<IActionResult> EnrollMembersFromCsv(Guid schoolId, [FromForm] EnrollMembersFromCsvRequest request)
        {
            var command = new EnrollMembersFromCsvCommand(request.File, schoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/members/{memberId}")]
        public async Task<IActionResult> ExpellMember(Guid schoolId, Guid memberId)
        {
            var command = new ExpellMemberCommand(memberId, schoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

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

        [HttpPut("schools/{schoolId}/groups/{groupId}/students")]
        public async Task<IActionResult> AddStudentsToGroup(Guid schoolId, int groupId, AddStudentsToGroupRequest request)
        {
            var command = new AddStudentsToGroupCommand(request.StudentIds, AuthId, schoolId, groupId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpPut("schools/{schoolId}/groups/{groupId}/form-tutor")]
        public async Task<IActionResult> MakeTeacherFormTutor(Guid schoolId, long groupId, MakeTeacherFormTutorRequest request)
        {
            var command = new MakeTeacherFormTutorCommand(request.TeacherId, groupId, schoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/groups/{groupId}/form-tutor")]
        public async Task<IActionResult> DivestFormTutor(Guid schoolId, long groupId)
        {
            var command = new DivestFormTutorCommand(groupId, schoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/groups/{groupId}/students/{studentId}")]
        public async Task<IActionResult> DisenrollStudentFromGroup(Guid schoolId, long groupId, Guid studentId)
        {
            var command = new DisenrollStudentFromGroupCommand(groupId, studentId, schoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("schools/{schoolId}/groups/{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid schoolId, long groupId)
        {
            var command = new DeleteGroupCommand(groupId, schoolId, AuthId);

            var result = await Handle(command);

            var response = FromResultOk(result);

            return response;
        }
    }
}

