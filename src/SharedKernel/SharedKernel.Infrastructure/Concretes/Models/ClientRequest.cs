using System;

namespace SharedKernel.Infrastructure.Concretes.Models
{
    public class ClientRequest
    {
        public Guid Id { get; }
        public string Name { get; }
        public DateTime Time { get; }

        public ClientRequest(Guid id, string name, DateTime time)
        {
            Id = id;
            Name = name;
            Time = time;
        }

        protected ClientRequest() { }
    }
}
