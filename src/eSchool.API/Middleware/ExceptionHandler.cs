using System;
using System.Net;
using System.Threading.Tasks;
using eSchool.API.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedKernel.Infrastructure.Errors;

namespace eSchool.API.Middleware
{
    public sealed class ExceptionHandler
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next, IWebHostEnvironment env, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
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
            _logger.LogError(exception, "Error when processing request {RequestName}", exception.Source ?? string.Empty);
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