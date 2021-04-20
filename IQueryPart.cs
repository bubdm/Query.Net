﻿using System.Data;
using System.Text;

namespace QueryNet
{
    internal interface IQueryPart<T> where T : DbModel
    {
        void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command);
    }
}
