using SchoolManagement.Application.Common.Interfaces;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace SchoolManagement.Infrastructure.Services
{
    internal sealed class SqlConnectionFactory : SK.SqlConnectionFactory, ISqlConnectionFactory
    {
        public SqlConnectionFactory(string connectionString) 
            : base(connectionString)
        {
        }
    }
}