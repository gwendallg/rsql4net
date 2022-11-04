namespace RSql4Net.Models.Queries
{
    /// <summary>
    /// interface describe a RSql Query Cache
    /// </summary>
    public interface IRSqlQueryCache
    {
        /// <summary>
        /// Try get Query from cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryGetValue<T>(string key, out IRSqlQuery<T> result) where T : class;
        
        /// <summary>
        /// Set Query to cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void Set<T>(string key, IRSqlQuery<T> value) where T : class;
    }
}
