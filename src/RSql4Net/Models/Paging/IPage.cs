using System.Collections.Generic;

namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     interface of page
    /// </summary>
    public interface IPage<T> where T : class
    {
        /// <summary>
        ///     Gets the content.
        /// </summary>
        /// <value>The content.</value>
        IList<T> Content { get; }

        /// <summary>
        ///     Gets the total elements.
        /// </summary>
        /// <value>The total elements.</value>
        long TotalElements { get; }

        /// <summary>
        ///     Gets the number of page.
        /// </summary>
        /// <value>The number.</value>
        int Number { get; }

        /// <summary>
        ///     Gets the number of elements of page.
        /// </summary>
        /// <value>The number of elements.</value>
        int NumberOfElements { get; }

        /// <summary>
        ///     Gets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        int TotalPages { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="T:Autumn.Mvc.Models.Paginations.IPage`1" /> has content.
        /// </summary>
        /// <value><c>true</c> if has content; otherwise, <c>false</c>.</value>
        bool HasContent { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="T:Autumn.Mvc.Models.Paginations.IPage`1" /> has next.
        /// </summary>
        /// <value><c>true</c> if has next; otherwise, <c>false</c>.</value>
        bool HasNext { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="T:Autumn.Mvc.Models.Paginations.IPage`1" /> has previous.
        /// </summary>
        /// <value><c>true</c> if has previous; otherwise, <c>false</c>.</value>
        bool HasPrevious { get; }
    }
}
