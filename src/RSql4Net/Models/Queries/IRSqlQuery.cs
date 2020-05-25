using System;
using System.Linq.Expressions;

namespace RSql4Net.Models.Queries
{
    public interface IRSqlQuery<T>
    {
        Expression<Func<T, bool>> Value();
    }
}
