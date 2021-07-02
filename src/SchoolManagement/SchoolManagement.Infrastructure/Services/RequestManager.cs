using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Infrastructure.Persistence;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace SchoolManagement.Infrastructure.Services
{
    internal sealed class RequestManager : SK.RequestManager, IRequestManager
    {
        public RequestManager(SchoolContext context)
            : base(context) { }
    }
}
