using System.Collections.Generic;

namespace SharedKernel.Infrastructure.Errors
{
    public sealed class BodyFieldErrorModel
    {
        public BodyFieldErrorModel(string fieldName, IReadOnlyCollection<string> message)
        {
            FieldName = fieldName;
            Message = message;
        }

        public string FieldName { get; }
        public IReadOnlyCollection<string> Message { get; }
    }
}