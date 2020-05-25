namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     Default implement of Pageable.
    /// </summary>
    public class RSqlPageable<T> : IRSqlPageable<T> where T : class
    {
        private readonly int _pageNumber;
        private readonly int _pageSize;
        private readonly RSqlSort<T> _rSqlSort;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:RSql4Net.Models.Paging.Pageable`1" /> class.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="rSqlSort">Sort.</param>
        public RSqlPageable(int pageNumber, int pageSize, RSqlSort<T> rSqlSort = null)
        {
            _pageNumber = pageNumber;
            _pageSize = pageSize;
            _rSqlSort = rSqlSort;
        }

        /// <summary>
        ///     Gets the page number.
        /// </summary>
        /// <value>The page number.</value>
        public int PageNumber()
        {
            return _pageNumber;
        }

        /// <summary>
        ///     Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize()
        {
            return _pageSize;
        }

        /// <summary>
        ///     Gets the sort.
        /// </summary>
        /// <value>The sort.</value>
        public RSqlSort<T> Sort()
        {
            return _rSqlSort;
        }
    }
}
