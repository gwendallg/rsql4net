using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     Sort expression.
    /// </summary>
    public class RSqlSort<T> where T : class
    {
        /// <summary>
        /// next sort
        /// </summary>
        public RSqlSort<T> Next { get; set; }
        
        /// <summary>
        /// previous sort
        /// </summary>
        public RSqlSort<T> Previous { get; set; }

        /// <summary>
        /// root sort
        /// </summary>
        public RSqlSort<T> Root
        {
            get
            {
                return Previous == null ? this : Previous.Root;
            }
        }


        /// <summary>
        /// value of sort
        /// </summary>
        public Expression<Func<T, object>> Value { get; set; }
        
        /// <summary>
        /// is descending sort
        /// </summary>
        public bool IsDescending { get; set; }
    }
}
