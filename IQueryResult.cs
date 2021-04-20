using System.Threading.Tasks;

namespace QueryNet
{
    public interface IQueryResult<T, TResult> where T : DbModel, new()
    {
        void SetQuery(QueryBuilder<T> queryBuilder);

        Task<TResult> GetResult();
    }
}
