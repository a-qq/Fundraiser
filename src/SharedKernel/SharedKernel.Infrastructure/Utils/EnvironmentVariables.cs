using System;

namespace SharedKernel.Infrastructure.Utils
{
    public static class EnvironmentVariables
    {
        public static string IdpUrl = Environment.GetEnvironmentVariable("Urls__Idp");
        public static string ClientUrl = Environment.GetEnvironmentVariable("Urls__Client");
        public static string ApiUrl = Environment.GetEnvironmentVariable("Urls__Api");
        public static string JwtSecret = Environment.GetEnvironmentVariable("JWTSecret");
    }
}