using System.Data;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}