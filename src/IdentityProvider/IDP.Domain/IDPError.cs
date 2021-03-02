using SharedKernel.Domain.Errors;

namespace IDP.Domain
{
    public static class IDPError 
    {
        public static class User
        {
            public static Error LoginFailed() => new Error("Invalid email or password!");
            public static Error InvalidSecurityCode() => new Error("Security code invalid or expired!");
            public static Error RegistrationNotCompelted() => new Error("Registration has not been completed!");

        }
    }
}
