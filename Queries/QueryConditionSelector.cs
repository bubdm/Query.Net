using System;
using System.Threading.Tasks;
using Query.Net.Queries.Conditions;

namespace Query.Net.Queries
{
    public class QueryConditionSelector<T, TResult, TResultType>
        where T : DbModel, new()
        where TResult : IQueryResult<T, TResultType>, new()
    {
        /// <summary>
        /// The query being affected
        /// </summary>
        private readonly QueryBuilder<T> queryBuilder;

        public QueryConditionSelector(QueryBuilder<T> queryBuilder)
        {
            this.queryBuilder = queryBuilder;
        }

        public Task<TResultType> Where(Func<T, Condition[]> conditionGetter)
        {
            queryBuilder.SetCondition(new WhereCondition<T>(conditionGetter));
            var result = new TResult();
            result.SetQuery(queryBuilder);
            return result.GetResult();
        }
    }
}
