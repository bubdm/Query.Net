using System;
namespace Query.Net.Queries
{
    internal interface IQueryCondition<T> : IQueryPart<T> where T : DbModel
    {
    }
}
