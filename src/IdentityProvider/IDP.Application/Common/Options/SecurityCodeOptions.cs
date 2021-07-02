namespace IDP.Application.Common.Options
{
    public sealed class SecurityCodeOptions
    {
        public const string SecurityCode = "SecurityCode";
        public int AntiSpamInMinutes { get; set; }
        public int ExpirationTimeInHours { get; set; }
    }
}