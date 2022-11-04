using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RSql4Net.Models.Queries
{

    /// <summary>
    /// 
    /// </summary>
    public class MemoryRSqlQueryCache : IRSqlQueryCache
    {
        private readonly MemoryCache _memoryCache;
        private readonly Action<MemoryCacheEntryOptions> _onSetValue;

        public MemoryRSqlQueryCache(
            IOptions<MemoryCacheOptions> optionsAccessor = null,
            ILoggerFactory loggerFactory  = null,
            Action<MemoryCacheEntryOptions> onSet=null)
        {

            if(loggerFactory != null) {
                _memoryCache = new MemoryCache(optionsAccessor ?? new MemoryCacheOptions(), loggerFactory);
            }
            else{
                _memoryCache = new MemoryCache(optionsAccessor ?? new MemoryCacheOptions());
            }
            _onSetValue = onSet;
        }
        public bool TryGetValue<T>(string key, out IRSqlQuery<T> result) where T : class
        {
            result = null;
            if (_memoryCache.TryGetValue<IRSqlQuery<T>>(key, out var data))
            {
                result = data;
                return true;
            }
            return false;
        }

        public void Set<T>(string key, IRSqlQuery<T> value) where T : class
        {
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions();
            _onSetValue?.Invoke(memoryCacheEntryOptions);
            _memoryCache.Set(key, value, memoryCacheEntryOptions);
        }
    }
}
