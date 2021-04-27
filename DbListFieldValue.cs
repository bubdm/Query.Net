using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace QueryNet
{
    public class DbListFieldValue<T> : DbFieldValue<string>
    {
        private readonly Func<string, T> deserializer;
        private readonly Func<T, string> serializer;

        public DbListFieldValue(string fieldName, Func<string, T> deserializer, Func<T, string> serializer) : base(fieldName)
        {
            this.deserializer = deserializer;
            this.serializer = serializer;
        }

        public new ReadOnlyCollection<T> value
        {
            get => Get();
        }

        private List<T> GetList()
        {
            var stringValue = base.Get();

            if (stringValue == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                new List<T>();
            }

            return stringValue.Split(',')
                .Select(x => deserializer(x))
                .ToList();
        }

        internal new ReadOnlyCollection<T> Get()
        {
            var list = GetList();

            if (list == null)
            {
                return null;
            }

            return new ReadOnlyCollection<T>(list);
        }

        public void Add(T value)
        {
            var list = GetList();
            if (list == null)
            {
                list = new List<T>();
            }

            list.Add(value);

            var newValue = list.Aggregate(new StringBuilder(), (builder, next) =>
            {
                if (builder.Length != 0)
                {
                    builder.Append(',');
                }
                builder.Append(serializer(next));
                return builder;
            }).ToString();

            Set(newValue);
        }
    }
}
