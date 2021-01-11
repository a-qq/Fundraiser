using Fundraiser.SharedKernel.Utils;

namespace IDP.Core
{
    public static class IDPErrors 
    {
        public static class User
        {
            public const string LoginFailed = "Invalid email or password!";
            public const string InvalidSecurityCode = "Security code invalid or expired!";
            public const string RegistrationNotCompelted = "Registration has not been completed!";
            /* other errors specific to students go here */
        }
    }
}
