using System.Data;
using System.Text;

namespace Query.Net.Queries
{
    internal interface IQueryPart<T> where T : DbModel
    {
        void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command);
    }
}
