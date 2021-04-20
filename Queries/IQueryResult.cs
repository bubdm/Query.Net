using System.Threading.Tasks;

namespace Query.Net.Queries
{
    public interface IQueryResult<T, TResult> where T : DbModel, new()
    {
        void SetQuery(QueryBuilder<T> queryBuilder);

        Task<TResult> GetResult();
    }
}
