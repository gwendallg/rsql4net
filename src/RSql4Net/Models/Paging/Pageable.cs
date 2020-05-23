namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     Default implement of Pageable.
    /// </summary>
    public class Pageable<T> : IPageable<T> where T : class
    {
        private readonly int _pageNumber;
        private readonly int _pageSize;
        private readonly Sort<T> _sort;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:RSql4Net.Models.Paging.Pageable`1" /> class.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sort">Sort.</param>
        public Pageable(int pageNumber, int pageSize, Sort<T> sort = null)
        {
            _pageNumber = pageNumber;
            _pageSize = pageSize;
            _sort = sort;
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
        public Sort<T> Sort()
        {
            return _sort;
        }
    }
}
