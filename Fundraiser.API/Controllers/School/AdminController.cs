﻿using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Data.Schools.Commands.RegisterSchool;
using SchoolManagement.Data.Schools.RegisterSchool;
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
        public async Task<IActionResult> RegisterSchool([FromBody]RegisterSchoolRequest request)

        {
            var command = new RegisterSchoolCommand(request.Name, request.HeadmasterFirstName,
                request.HeadmasterLastName, request.HeadmasterEmail, request.HeadmasterGender, AuthId);

            var result = await Handle(command);
            IActionResult response = FromResultOk(result);

            //var response = errorResponseOrNull.HasNoValue
            //    ? Ok(result.Value)/* CreatedAtRoute("GetSchool", new { result.Value.Id }, Envelope.Ok(result.Value))*/
            //    : errorResponseOrNull.Value;

            return response;
        }
    }
}
