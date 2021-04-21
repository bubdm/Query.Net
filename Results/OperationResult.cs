using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace QueryNet.Results
{
    public class OperationResult<T> : IQueryResult<T, int> where T : DbModel, new()
    {
        private QueryBuilder<T> queryBuilder;

        public void SetQuery(QueryBuilder<T> queryBuilder)
        {
            this.queryBuilder = queryBuilder;
        }

        public async Task<int> GetResult()
        {
            T model = null;
            var connection = await Query.GetConnection();

            try
            {
                var command = queryBuilder.Build(connection, ref model);
                var result = await command.ExecuteNonQueryAsync();
                if (result > 0 && model != null)
                {
                    foreach (var field in model.GetAllFields())
                    {
                        field.Save();
                    }
                }
                return result;
            }
            finally
            {
                Query.ReturnConnection(connection);
            }
        }
    }
}
