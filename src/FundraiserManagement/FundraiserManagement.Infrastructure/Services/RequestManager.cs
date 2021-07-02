using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Infrastructure.Persistence;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace FundraiserManagement.Infrastructure.Services
{
    internal sealed class RequestManager : SK.RequestManager, IRequestManager
    {
        public RequestManager(FundraiserContext context)
            : base(context) { }
    }
}