﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging.Exceptions;

namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     Pageable model binder.
    /// </summary>
    public class RSqlPageableModelBinder<T> : IModelBinder where T : class
    {
        private readonly Settings _settings;
        private readonly IOptions<JsonOptions> _options;
        private readonly ILogger<T> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:RSql4Net.Models.Paging.PageableModelBinder`1" /> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="options">Options.</param>
        /// <param name="logger">Logger.</param>
        public RSqlPageableModelBinder(Settings settings, IOptions<JsonOptions> options, ILogger<T> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Binds the model async.
        /// </summary>
        /// <returns>The model async.</returns>
        /// <param name="bindingContext">Binding context.</param>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                bindingContext.Result =
                    ModelBindingResult.Success(Build(bindingContext.ActionContext.HttpContext.Request.Query));
            }
            catch (Exception e)
            {
                bindingContext.ModelState.AddModelError(GetType().FullName ?? string.Empty, e.Message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Build the specified queryCollection.
        /// </summary>
        /// <returns>The build.</returns>
        /// <param name="queryCollection">Query collection.</param>
        public IRSqlPageable<T> Build(IQueryCollection queryCollection)
        {
            if (queryCollection == null)
            {
                throw new ArgumentNullException(nameof(queryCollection));
            }

            var pageSize = GetPageSize(queryCollection);
            var pageNumber = GetPageNumber(queryCollection);
            var sort = GetSort(queryCollection);
            return new RSqlPageable<T>(pageNumber, pageSize, sort);
        }

        /// <summary>
        ///     Gets the size of the page.
        /// </summary>
        /// <returns>The page size.</returns>
        /// <param name="queryCollection">Query collection.</param>
        public int GetPageSize(IQueryCollection queryCollection)
        {
            var pageSizeField =
                _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.PageSizeField);
            if (queryCollection.TryGetValue(pageSizeField, out var pageSizeString))
            {
                if (int.TryParse(pageSizeString[0], out var pageSize))
                {
                    if (pageSize <= 0)
                    {
                        throw new OutOfRangePageSizeException(pageSize);
                    }
                }
                else
                {
                    throw new InvalidPageSizeValueException(pageSizeString[0]);
                }
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"RSqlPageable<{typeof(T).FullName}> pageSize: ?{pageSizeField}={pageSizeString} -> PageSize => {pageSize}");
                }
                return pageSize;
            }
            return _settings.PageSize;
        }

        /// <summary>
        ///     Gets the page number.
        /// </summary>
        /// <returns>The page number.</returns>
        /// <param name="queryCollection">Query collection.</param>
        public int GetPageNumber(IQueryCollection queryCollection)
        {
            var pageNumberField =
                _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.PageNumberField);
            if (queryCollection.TryGetValue(pageNumberField, out var pageNumberString))
            {
                if (int.TryParse(pageNumberString[0], out var pageNumber))
                {
                    if (pageNumber < 0)
                    {
                        throw new OutOfRangePageNumberException(pageNumber);
                    }
                }
                else
                {
                    throw new InvalidPageNumberValueException(pageNumberString[0]);
                }
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"RSqlPageable<{typeof(T).FullName}> pageNumber: ?{pageNumberField}=s{pageNumberString} -> PageNumber => {pageNumber}");
                }
                return pageNumber;
            }

            return 0;
        }

        private static IDictionary<string, bool> ExtractSortFromQueryCollection(StringValues stringValues)
        {
            var result = new Dictionary<string, bool>();
            foreach (var stringValue in stringValues)
            {
                foreach (var item in stringValue.Split(','))
                {
                    var data = item.Split(';');
                    var field = data[0];
                    bool isDescending = false;

                    if (data.Length == 2)
                    {
                        if (data[1].ToLowerInvariant() == "desc")
                        {
                            isDescending = true;
                        }
                        else if (data[1].ToLowerInvariant() != "asc")
                        {
                            throw new UnknownSortException(item);
                        }
                    }
                    if (data.Length > 2)
                    {
                        throw new InvalidSortDirectionException(data);
                    }

                    result[field] = isDescending;
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets the sort.
        /// </summary>
        /// <returns>The sort.</returns>
        /// <param name="queryCollection">Query collection.</param>
        public RSqlSort<T> GetSort(IQueryCollection queryCollection)
        {
            var sortField = _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.SortField);
            if (queryCollection.TryGetValue(sortField, out var sortStringValues))
            {
                StringBuilder prefixLogBuilder = null;
                StringBuilder suffixLogBuilder = null;
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    prefixLogBuilder = new StringBuilder();
                    suffixLogBuilder = new StringBuilder();
                }

                var parameter = Expression.Parameter(typeof(T));
                RSqlSort<T> currentSort = null;
                foreach (var (key, isDescending) in ExtractSortFromQueryCollection(sortStringValues))
                {
                    if (!ExpressionValue.TryParse<T>(parameter, key,
                        _options.Value.JsonSerializerOptions.PropertyNamingPolicy,
                        out var exp))
                    {
                        throw new UnknownSortException(key);
                    }

                    var expression = Expression.Convert(exp.Expression, typeof(object));
                    var orderExpression = Expression.Lambda<Func<T, object>>(expression, parameter);
                    var newSort = new RSqlSort<T>()
                    {
                        Value = orderExpression, IsDescending = isDescending, Previous = currentSort
                    };

                    if (currentSort != null)
                    {
                        currentSort.Next = newSort;
                    }

                    currentSort = newSort;

                    LogSortField(sortField, key, prefixLogBuilder, suffixLogBuilder, currentSort, exp);

                }

                LogSort(prefixLogBuilder, suffixLogBuilder);
                return currentSort?.Root;
            }

            return null;
        }

        /// <summary>
        /// log sort field
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="key"></param>
        /// <param name="prefixLogBuilder"></param>
        /// <param name="suffixLogBuilder"></param>
        /// <param name="currentSort"></param>
        /// <param name="exp"></param>
        private void LogSortField(string sortField, string key, StringBuilder prefixLogBuilder, StringBuilder suffixLogBuilder,
            RSqlSort<T> currentSort, ExpressionValue exp)
        {
            if (!_logger.IsEnabled(LogLevel.Debug))
            {
                return;
            }

            var sort = currentSort.IsDescending ? "desc" : "asc";
            prefixLogBuilder?.Append($"{sortField}={key};{sort},");
            var sort2 = currentSort.IsDescending ? "OrderByDescending" : "OrderBy";
            suffixLogBuilder?.Append($"{sort2}({exp.Expression}) && ");
        }

        /// <summary>
        /// log sort
        /// </summary>
        /// <param name="prefixLogBuilder"></param>
        /// <param name="suffixLogBuilder"></param>
        private void LogSort(StringBuilder prefixLogBuilder, StringBuilder suffixLogBuilder)
        {
            if (!_logger.IsEnabled(LogLevel.Debug))
            {
                return;
            }

            var prefixLog = prefixLogBuilder?.ToString().TrimEnd(',');
            var suffixLog = suffixLogBuilder?.ToString();
            if (suffixLog != null && suffixLog.Length > 4)
            {
                suffixLog = suffixLog?.Substring(0, suffixLog.Length - 4);
            }
            _logger.LogDebug(
                $"RSqlPageable<{typeof(T).FullName}> sort: ?{prefixLog} -> {suffixLog}");
        }
    }
}
