using System;
namespace QueryNet
{
    internal interface IQueryCondition<T> : IQueryPart<T> where T : DbModel
    {
    }
}
