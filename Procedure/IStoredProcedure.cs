using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QueryNet.Procedure
{
    public interface IStoredProcedure<T>
    {
        DbType returnType { get; }

        string returnName { get; }

        string procedureName { get; }
    }
}
