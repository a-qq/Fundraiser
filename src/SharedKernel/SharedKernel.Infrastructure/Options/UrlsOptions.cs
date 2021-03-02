namespace SharedKernel.Infrastructure.Options
{
    public sealed class UrlsOptions
    {
        public const string Urls = "Urls";
        public string Idp { get; set; }
        public string Client { get; set; }
    }
}