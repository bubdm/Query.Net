using System;
using System.Text;

namespace QueryNet
{
    internal interface IQueryMethod<T> : IQueryPart<T> where T : DbModel
    {
        bool CanExecute();
    }
}
