namespace IDP.Application.Common.Models
{
    internal sealed class ClaimInsertModel
    {
        public string Type { get; }
        public string Value { get; }

        public ClaimInsertModel(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
