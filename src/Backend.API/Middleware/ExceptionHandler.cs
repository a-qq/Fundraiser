using System;
using System.Net;
using System.Threading.Tasks;
using Backend.API.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SharedKernel.Infrastructure.Errors;

namespace Backend.API.Middleware
{
    public sealed class ExceptionHandler
    {
        private readonly IWebHostEnvironment _env;
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //TODO: Log exception here
            var result = _env.IsProduction()
                ? JsonConvert.SerializeObject(Envelope.Error(SharedRequestError.General.InternalServerError()))
                : JsonConvert.SerializeObject(
                    Envelope.Error(SharedRequestError.General.InternalServerError(exception.Message)));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}