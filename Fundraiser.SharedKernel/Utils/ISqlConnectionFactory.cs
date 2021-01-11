using System.Data;

namespace Fundraiser.SharedKernel.Utils
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
