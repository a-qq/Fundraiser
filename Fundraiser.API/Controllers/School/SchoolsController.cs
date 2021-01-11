using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Data.Schools.EnrollMember;
using System.Threading.Tasks;

namespace Fundraiser.API.Controllers.School
{
    [Route("schools/")]
    [ApiController]
    public class SchoolsController : MediatrController
    {

        public SchoolsController(IMediator mediator)
            : base(mediator) {  }
        

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
    }
}
