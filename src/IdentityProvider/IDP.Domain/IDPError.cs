using SharedKernel.Domain.Errors;

namespace IDP.Domain
{
    public static class IDPError
    {
        public static class User
        {
            public static Error LoginFailed()
            {
                return new Error("Invalid email or password!");
            }

            public static Error InvalidSecurityCode()
            {
                return new Error("Security code invalid or expired!");
            }

            public static Error RegistrationNotCompelted()
            {
                return new Error("Registration has not been completed!");
            }
        }
    }
}