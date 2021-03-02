namespace SharedKernel.Infrastructure.Errors
{
    public sealed class BodyFieldErrorModel
    {
        public BodyFieldErrorModel(string fieldName, string[] message)
        {
            FieldName = fieldName;
            Message = message;
        }

        public string FieldName { get; }
        public string[] Message { get; }
    }
}