using CSharpFunctionalExtensions;
using System;

namespace DlxWorker
{
    public class DeadLetterEntry : Entity<Guid>
    {
        public DeadLetterEntry(string type, string content,
            string reason, DateTime creationTime, DateTime handledAt)
        {
            Type = type;
            Content = content;
            CreationTime = creationTime;
            HandledAt = handledAt;
            Reason = reason;
        }

        public string Type { get; }
        public string Content { get; }
        public string Reason { get; }
        public DateTime CreationTime { get; }
        public DateTime HandledAt { get; }

        protected DeadLetterEntry()
        {
        }

    }
}