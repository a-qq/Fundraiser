﻿namespace SharedKernel.Infrastructure.Options
{
    public sealed class MailOptions
    {
        public const string MailSettings = "MailSettings";
        public string Email { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
    }
}
