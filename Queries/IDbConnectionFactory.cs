using System.Data.Common;
using System.Threading.Tasks;

namespace Query.Net.Queries
{
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateDbConnection();
    }
}
