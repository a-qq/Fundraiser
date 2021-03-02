using System.Data;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}