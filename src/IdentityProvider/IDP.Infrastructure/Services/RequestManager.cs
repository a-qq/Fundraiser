using IDP.Infrastructure.Persistence;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace IDP.Infrastructure.Services
{
    public sealed class RequestManager : SK.RequestManager
    {
        public RequestManager(IdentityDbContext context) 
            : base(context)
        {
        }
    }
}