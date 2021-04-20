using System;
using System.Threading.Tasks;
using QueryNet.Conditions;

namespace QueryNet
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
            if (conditionGetter == null)
            {
                throw new ArgumentNullException("conditionGetter", "conditionGetter cannot be null");
            }

            queryBuilder.SetCondition(new WhereCondition<T>(conditionGetter));
            var result = new TResult();
            result.SetQuery(queryBuilder);
            return result.GetResult();
        }

        public Task<TResultType> Where(Func<T, DbFieldValue> singleKeyGetter)
        {
            if (singleKeyGetter == null)
            {
                throw new ArgumentNullException("singleKeyGetter", "singleKeyGetter cannot be null");
            }

            return Where(x =>
            {
                var key = singleKeyGetter(x);
                return new[] { Condition.Equals(key, key.Get()) };
            });
        }
    }
}
