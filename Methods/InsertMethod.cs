using System;
using System.Data;
using System.Linq;
using System.Text;

namespace QueryNet.Methods
{
    public class InsertMethod<T> : IQueryMethod<T> where T : DbModel, new()
    {
        private readonly T[] models;

        public InsertMethod(T[] models)
        {
            if (models == null)
            {
                throw new ArgumentNullException("models");
            }

            if (models.Length == 0)
            {
                throw new ArgumentException("models array cannot be empty");
            }

            int fieldCount = models[0].GetAllFields().Count();
            foreach (var model in models)
            {
                if (model.GetAllFields().Count() != fieldCount)
                {
                    throw new ArgumentException("models received have differing fields");
                }
            }

            this.models = models;
        }

        public void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command)
        {
            model = models[0];
            AddMethod(ref model, builder);
            AddFields(ref model, builder);
            AddValues(builder, command);
        }

        public bool CanExecute()
        {
            return true;
        }

        private void AddMethod(ref T model, StringBuilder builder)
        {
            builder.Append("INSERT INTO ");
            builder.Append(model.tableName);
        }

        private void AddFields(ref T model, StringBuilder builder)
        {
            builder.Append(" (");
            bool first = true;
            foreach (var field in model.GetAllFields())
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;
                builder.Append(field.GetFieldName());
            }
            builder.Append(")");
        }

        private void AddValues(StringBuilder builder, IDbCommand command)
        {
            builder.Append(" VALUES");

            int modelIndex = 0;
            bool firstModel = true;
            foreach (var model in models)
            {
                if (!firstModel)
                {
                    builder.Append(',');
                }
                firstModel = false;

                builder.Append('(');
                bool first = true;
                foreach (var field in model.GetAllFields())
                {
                    if (!first)
                    {
                        builder.Append(", ");
                    }
                    first = false;
                    builder.Append('?');
                    builder.Append(field.GetFieldName());

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"?{field.GetFieldName()}{modelIndex}";
                    parameter.Value = field.GetForDb();
                    command.Parameters.Add(parameter);
                }
                builder.Append(")");
                modelIndex++;
            }
        }
    }
}
