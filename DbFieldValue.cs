using System;
namespace QueryNet
{
    public class DbFieldValue<T> : DbFieldValue
    {
        public DbFieldValue(string fieldName) : base(fieldName)
        {
        }

        public T value
        {
            get => Get();
            set => Set(value);
        }

        public bool boolValue
        {
            get => Convert.ToInt32(Get()) > 0;
            set => Set(value ? 1 : 0);
        }

        public new T Get()
        {
            return (T)base.Get();
        }

        public void Set(T value)
        {
            base.Set(value);
        }
    }

    public class DbFieldValue
    {
        /// <summary>
        /// The name of the field within the database
        /// </summary>
        private readonly string fieldName;

        /// <summary>
        /// The value of the field within the database
        /// </summary>
        private object dbValue;

        /// <summary>
        /// The local value of the field
        /// </summary>
        private object localValue;

        public DbFieldValue(string fieldName)
        {
            this.fieldName = fieldName;
        }

        /// <summary>
        /// Gets the field name of this value within the database
        /// </summary>
        /// <returns></returns>
        public string GetFieldName()
        {
            return fieldName;
        }

        /// <summary>
        /// Gets the local value of this field
        /// </summary>
        /// <returns></returns>
        public object Get()
        {
            return localValue;
        }

        /// <summary>
        /// Sets the local value of this field
        /// </summary>
        public void Set(object value)
        {
            localValue = value;
        }

        /// <summary>
        /// Saves the local values into db values
        /// </summary>
        internal void Save()
        {
            dbValue = localValue;
        }

        /// <summary>
        /// Gets the value as a db value
        /// </summary>
        /// <returns></returns>
        internal object GetForDb()
        {
            if (localValue == null)
            {
                return DBNull.Value;
            }

            return Get();
        }

        /// <summary>
        /// Returns if the current value is updated from the db value
        /// </summary>
        /// <returns></returns>
        public bool IsUpdated()
        {
            if (localValue == null)
            {
                return dbValue != null;
            }

            return !localValue.Equals(dbValue);
        }
    }
}
