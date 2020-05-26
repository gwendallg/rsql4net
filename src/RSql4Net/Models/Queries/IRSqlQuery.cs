using System;
using System.Linq.Expressions;

namespace RSql4Net.Models.Queries
{
    /// <summary>
    /// interface describe a RSql Qiery
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRSqlQuery<T> where T: class
    {
        /// <summary>
        /// Value of RSql Query 
        /// </summary>
        /// <returns></returns>
        Expression<Func<T, bool>> Value();
    }
}
