using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Schools.Commands.DivestTreasurer;
using SchoolManagement.Application.Schools.Commands.PromoteTreasurer;

namespace Backend.API.Controllers.School
{
    [Route("management/formtutor/")]
    [Authorize("MustBeFormTutor")]
    [ApiController]
    public class FormTutorController : MediatrController
    {
        [HttpPut("treasurer")]
        public async Task<IActionResult> PromoteTreasurer(Guid groupId, PromoteTreasurerRequest request)
        {
            var command = new PromoteTreasurerCommand(groupId, request.StudentId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }

        [HttpDelete("treasurer")]
        public async Task<IActionResult> DivestTreasurer(Guid groupId)
        {
            var command = new DivestTreasurerCommand(groupId, SchoolId);

            var result = await Handle(command);

            var response = FromResultNoContent(result);

            return response;
        }
    }
}