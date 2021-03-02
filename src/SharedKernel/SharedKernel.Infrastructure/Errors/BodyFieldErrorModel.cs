namespace SharedKernel.Infrastructure.Errors
{
    public sealed class BodyFieldErrorModel
    {
        public string FieldName { get; }
        public string[] Message { get; }

        public BodyFieldErrorModel(string fieldName, string[] message)
        {
            FieldName = fieldName;
            Message = message;
        }
    }
}
