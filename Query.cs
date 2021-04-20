using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using QueryNet.Methods;
using QueryNet.Procedure;
using QueryNet.Results;

namespace QueryNet
{
    public static class Query
    {
        private static readonly ConcurrentQueue<DbConnection> connections = new ConcurrentQueue<DbConnection>();

        private static IDbConnectionFactory connectionFactory;

        /// <summary>
        /// Sets the database connection factory
        /// </summary>
        public static void SetConnectionFactory(IDbConnectionFactory factory)
        {
            connectionFactory = factory;
        }

        /// <summary>
        /// Trys to get a database from the queue, creates a new database if none are available
        /// </summary>
        internal async static Task<DbConnection> GetConnection()
        {
            if (connections.TryDequeue(out var connection) && connection.State == ConnectionState.Open)
                return connection;
            return await Create();
        }

        /// <summary>
        /// Returns a connection to the queue
        /// </summary>
        internal static void ReturnConnection(DbConnection connection)
        {
            if (connection == null) return;
            connections.Enqueue(connection);
        }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns></returns>
        private async static Task<DbConnection> Create()
        {
            return await connectionFactory?.CreateDbConnection();
        }

        public static QueryConditionSelector<T, ModelResult<T>, List<T>> Select<T>(Func<T, DbFieldValue[]> fieldGetter = null) where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new SelectMethod<T>(fieldGetter));
            return new QueryConditionSelector<T, ModelResult<T>, List<T>>(builder);
        }

        /// <summary>
        /// Updates changed model values within the database
        /// </summary>
        /// <returns>The number of rows affected</returns>
        public static QueryConditionSelector<T, OperationResult<T>, int> Update<T>(T model) where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new UpdateMethod<T>(model));
            return new QueryConditionSelector<T, OperationResult<T>, int>(builder);
        }

        public static async Task<bool> Insert<T>(T model) where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new InsertMethod<T>(model));

            var result = new OperationResult<T>();
            result.SetQuery(builder);
            return await result.GetResult() > 0;
        }

        public static QueryConditionSelector<T, OperationResult<T>, int> Delete<T>(T model = null) where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new DeleteMethod<T>(model));
            return new QueryConditionSelector<T, OperationResult<T>, int>(builder);
        }

        public static async Task<TResult> Procedure<TResult, TModel>(TModel model) where TModel : DbModel, IStoredProcedure<TResult>
        {
            var connection = await GetConnection();

            try
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = model.procedureName;

                foreach (var field in model.GetAllFields())
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"?{field.GetFieldName()}";
                    parameter.Value = field.Get();
                    command.Parameters.Add(parameter);
                }

                var returnParamter = command.CreateParameter();
                returnParamter.ParameterName = $"?{model.returnName}";
                returnParamter.DbType = model.returnType;
                returnParamter.Direction = ParameterDirection.Output;
                command.Parameters.Add(returnParamter);

                await command.ExecuteNonQueryAsync();

                return (TResult)command.Parameters[$"?{model.returnName}"].Value;
            }
            finally
            {
                ReturnConnection(connection);
            }
        }
    }
}
