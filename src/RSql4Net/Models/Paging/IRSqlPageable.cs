namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     Pageable interface
    /// </summary>
    public interface IRSqlPageable<T> where T : class
    {
        /// <summary>
        ///     Gets the page number.
        /// </summary>
        /// <value>The page number.</value>
        int PageNumber();

        /// <summary>
        ///     Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        int PageSize();

        /// <summary>
        ///     Gets the sort.
        /// </summary>
        /// <value>The sort.</value>
        RSqlSort<T> Sort();
    }
}
