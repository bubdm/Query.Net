using System;
using System.Data;
using System.Text;

namespace QueryNet.Methods
{
    public class DeleteMethod<T> : IQueryMethod<T> where T : DbModel, new()
    {
        public void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command)
        {
            model = new T();
            AddMethod(model, builder);
        }

        private void AddMethod(T model, StringBuilder builder)
        {
            builder.Append("DELETE FROM ");
            builder.Append(model.tableName);
        }
    }
}
