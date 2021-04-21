using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QueryNet.Procedure
{
    public abstract class PrModel<T> : DbModel
    {
        public abstract DbType returnType { get; }

        public abstract string returnName { get; }

        public abstract string procedureName { get; }
    }
}
