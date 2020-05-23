using System;
using System.Linq.Expressions;

namespace RSql4Net.Models.Queries
{
    public interface IQuery<T>
    {
        Expression<Func<T, bool>> Value();
    }
}
