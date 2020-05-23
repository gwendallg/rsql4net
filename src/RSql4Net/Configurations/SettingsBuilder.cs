using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Serialization;
using RSql4Net.Configurations.Exceptions;
using RSql4Net.Models.Paging.Exceptions;

namespace RSql4Net.Configurations
{
    /// <summary>
    ///     RSql4 net settings builder.
    /// </summary>
    public class SettingsBuilder
    {
        private readonly Dictionary<string, string> _fieldNames = new Dictionary<string, string>();
        private NamingStrategy _namingStrategy = new DefaultNamingStrategy();
        private string _pageNumberFieldName = Settings.CDefaultPageNumberFieldName;
        private int _pageSize = Settings.CDefaultPageSize;
        private string _pageSizeFieldName = Settings.CDefaultPageSizeFieldName;
        private string _queryFieldName = Settings.CDefaultQueryFieldName;
        private string _sortFieldName = Settings.CDefaultSortFieldName;
        private IMemoryCache _queryCache;
        private Action<MemoryCacheEntryOptions> _onCreateCache;
        
        /// <summary>
        ///     Ckecks the name of the and register field.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="fieldName">Field name.</param>
        /// <param name="namingStrategy">Naming Strategy</param>
        private string CheckAndRegisterFieldName(string value, string fieldName, NamingStrategy namingStrategy)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (Regex.Match(value, @"(_)?([A-Za-z0-9]((_)?[A-Za-z0-9])*(_)?)").Value != value)
            {
                throw new InvalidFormatFieldNameException(fieldName, value);
            }

            var check = namingStrategy.GetPropertyName(value.Trim(), false);
            foreach (var item in _fieldNames.Keys)
            {
                if (item == fieldName)
                {
                    continue;
                }

                if (_fieldNames[item] == check)
                {
                    throw new AlreadyFieldNameUsedException(item, value);
                }
            }

            _fieldNames[fieldName] = value.Trim();
            return _fieldNames[fieldName];
        }

        /// <summary>
        ///     Build this instance.
        /// </summary>
        /// <returns>The build.</returns>
        public Settings Build()
        {
            var _settings = new Settings(_namingStrategy);
            _fieldNames.Clear();
            _settings.PageNumberField =
                CheckAndRegisterFieldName(_pageNumberFieldName, "PageNumberFieldName", _namingStrategy);
            _settings.PageSizeField =
                CheckAndRegisterFieldName(_pageSizeFieldName, "PageSizeFieldName", _namingStrategy);
            _settings.SortField = CheckAndRegisterFieldName(_sortFieldName, "SortFieldName", _namingStrategy);
            _settings.QueryField = CheckAndRegisterFieldName(_queryFieldName, "QueryFieldName", _namingStrategy);
            if (_pageSize < 1)
            {
                throw new OutOfRangePageSizeException(_pageSize);
            }

            _settings.PageSize = _pageSize;
            _settings.QueryCache = _queryCache;
            _settings.OnCreateCacheEntry = _onCreateCache;
            return _settings;
        }

        /// <summary>
        ///     configuration of namingstrategy
        /// </summary>
        /// <param name="namingStrategy">strategy name</param>
        /// <returns></returns>
        public SettingsBuilder NamingStrategy(NamingStrategy namingStrategy)
        {
            _namingStrategy = namingStrategy ?? throw new ArgumentNullException(nameof(namingStrategy));
            return this;
        }

        /// <summary>
        ///     configuration of page size
        /// </summary>
        /// <param name="pageSizeFieldName">query parameter name for page size</param>
        /// <returns></returns>
        public SettingsBuilder PageSizeFieldName(string pageSizeFieldName)
        {
            _pageSizeFieldName = pageSizeFieldName ?? throw new ArgumentNullException(nameof(pageSizeFieldName));
            return this;
        }

        /// <summary>
        ///     configuration of page size
        /// </summary>
        /// <param name="pageSize">default page size</param>
        /// <returns></returns>
        public SettingsBuilder PageSize(int pageSize = 100)
        {
            _pageSize = pageSize;
            return this;
        }
        /// <summary>
        ///     configuration of query cache
        /// </summary>
        /// <param name="queryCache">default query memory cache</param>
        /// <returns></returns>

        public SettingsBuilder QueryCache(IMemoryCache queryCache)
        {
            _queryCache = queryCache;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onCreateCache"></param>
        /// <returns></returns>
        public SettingsBuilder OnCreateCacheEntry(Action<MemoryCacheEntryOptions> onCreateCache)
        {
            _onCreateCache = onCreateCache;
            return this;
        }

        /// <summary>
        ///     configuration of page number
        /// </summary>
        /// <param name="pageNumberFieldName">query parameter name for page number</param>
        /// <returns></returns>
        public SettingsBuilder PageNumberFieldName(string pageNumberFieldName)
        {
            _pageNumberFieldName = pageNumberFieldName ?? throw new ArgumentNullException(nameof(pageNumberFieldName));
            return this;
        }

        /// <summary>
        ///     configuraion of sort field
        /// </summary>
        /// <param name="sortFieldName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SettingsBuilder SortFieldName(string sortFieldName)
        {
            _sortFieldName = sortFieldName ?? throw new ArgumentNullException(nameof(sortFieldName));
            return this;
        }

        /// <summary>
        ///     configuration of query field name
        /// </summary>
        /// <param name="queryFieldName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SettingsBuilder QueryFieldName(string queryFieldName)
        {
            _queryFieldName = queryFieldName ?? throw new ArgumentNullException(nameof(queryFieldName));
            return this;
        }
    }
}
