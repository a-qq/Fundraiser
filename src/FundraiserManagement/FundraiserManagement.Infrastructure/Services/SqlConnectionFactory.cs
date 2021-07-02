using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Interfaces.Services;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace FundraiserManagement.Infrastructure.Services
{
    internal sealed class SqlConnectionFactory : SK.SqlConnectionFactory, ISqlConnectionFactory
    {
        public SqlConnectionFactory(string connectionString) 
            : base(connectionString)
        {
        }
    }
}