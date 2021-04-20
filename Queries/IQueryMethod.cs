using System;
using System.Text;

namespace Query.Net.Queries
{
    internal interface IQueryMethod<T> : IQueryPart<T> where T : DbModel
    {

    }
}
