using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RSql4Net.Configurations.Exceptions;
using RSql4Net.Models.Paging.Exceptions;
using RSql4Net.Models.Queries;

namespace RSql4Net.Configurations
{
    /// <summary>
    ///     RSql4 net settings builder.
    /// </summary>
    public class SettingsBuilder
    {
        private readonly Dictionary<string, string> _fieldNames = new Dictionary<string, string>();
        private string _pageNumberFieldName = Settings.CDefaultPageNumberFieldName;
        private int _pageSize = Settings.CDefaultPageSize;
        private string _pageSizeFieldName = Settings.CDefaultPageSizeFieldName;
        private string _queryFieldName = Settings.CDefaultQueryFieldName;
        private string _sortFieldName = Settings.CDefaultSortFieldName;
        private IRSqlQueryCache _queryCache;
        
        /// <summary>
        ///     Ckecks the name of the and register field.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="fieldName">Field name.</param>
        private string CheckAndRegisterFieldName(string value, string fieldName)
        {
            if (Regex.Match(value, @"(_)?([A-Za-z0-9]((_)?[A-Za-z0-9])*(_)?)").Value != value)
            {
                throw new InvalidFormatFieldNameException(fieldName, value);
            }

            var check = value.Trim();
            foreach (var item in _fieldNames.Keys)
            {
                if (item != fieldName && _fieldNames[item] == check)
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
            var settings = new Settings();
            _fieldNames.Clear();
            settings.PageNumberField =
                CheckAndRegisterFieldName(_pageNumberFieldName, "PageNumberFieldName");
            settings.PageSizeField =
                CheckAndRegisterFieldName(_pageSizeFieldName, "PageSizeFieldName");
            settings.SortField = CheckAndRegisterFieldName(_sortFieldName, "SortFieldName");
            settings.QueryField = CheckAndRegisterFieldName(_queryFieldName, "QueryFieldName");
            if (_pageSize < 1)
            {
                throw new OutOfRangePageSizeException(_pageSize);
            }

            settings.PageSize = _pageSize;
            settings.QueryCache = _queryCache;
            return settings;
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

        public SettingsBuilder QueryCache(IRSqlQueryCache queryCache)
        {
            _queryCache = queryCache;
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
        ///     configuration of sort field
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
