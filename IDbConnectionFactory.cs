using System.Data.Common;
using System.Threading.Tasks;

namespace QueryNet
{
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateDbConnection();
    }
}
