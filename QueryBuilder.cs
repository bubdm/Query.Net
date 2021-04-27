using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace QueryNet
{
    public class QueryBuilder<T> where T : DbModel, new()
    {
        /// <summary>
        /// The method of the query
        /// </summary>
        private IQueryMethod<T> method;

        /// <summary>
        /// The condition of the query
        /// </summary>
        private IQueryCondition<T> condition;

        public QueryBuilder()
        {

        }

        internal void SetMethod(IQueryMethod<T> method)
        {
            this.method = method;
        }

        internal void SetCondition(IQueryCondition<T> condition)
        {
            this.condition = condition;
        }

        internal DbCommand Build(DbConnection connection, ref T model, out bool canExecute)
        {
            var textBuilder = new StringBuilder();
            var command = connection.CreateCommand();

            method.AppendToQuery(ref model, textBuilder, command);
            canExecute = method.CanExecute();
            condition?.AppendToQuery(ref model, textBuilder, command);

            command.CommandText = textBuilder.ToString();
            return command;
        }
    }
}
