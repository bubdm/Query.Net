using System;
using System.Data;
using System.Text;

namespace QueryNet.Methods
{
    public class DeleteMethod<T> : IQueryMethod<T> where T : DbModel, new()
    {
        private T model;

        public DeleteMethod(T model)
        {
            this.model = model;
        }

        public void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command)
        {
            model = this.model ?? new T();
            AddMethod(model, builder);
        }

        public bool CanExecute()
        {
            return true;
        }

        private void AddMethod(T model, StringBuilder builder)
        {
            builder.Append("DELETE FROM ");
            builder.Append(model.tableName);
        }
    }
}
