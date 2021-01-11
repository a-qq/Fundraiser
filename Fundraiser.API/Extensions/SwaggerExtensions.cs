using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace Fundraiser.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fundraiser",
                    Version = "v1"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Environment.GetEnvironmentVariable("FrontendSettings__IDPUrl") + "connect/authorize"),
                            TokenUrl = new Uri(Environment.GetEnvironmentVariable("FrontendSettings__IDPUrl") + "connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"fundraiserapi.fullaccess", "Fundraiser API  - fullaccess"}
                            }
                        }
                    }
                }); 

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }
    }
}

