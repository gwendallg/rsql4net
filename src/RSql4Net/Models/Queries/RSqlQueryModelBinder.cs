using System;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RSql4Net.Configurations;

namespace RSql4Net.Models.Queries
{
    public class RSqlQueryModelBinder<T> : IModelBinder where T: class
    {
        private readonly Settings _settings;
        private readonly IOptions<JsonOptions> _options;
        private readonly ILogger<T> _logger;

        public RSqlQueryModelBinder(Settings settings,IOptions<JsonOptions> options, ILogger<T> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
                bindingContext.Result = ModelBindingResult.Success(Build(queryCollection));
            }
            catch (Exception e)
            {
                bindingContext.ModelState.AddModelError(GetType().FullName, e.Message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     build specification from RSql query
        /// </summary>
        /// <param name="queryCollection"></param>
        /// <returns></returns>
        public IRSqlQuery<T> Build(IQueryCollection queryCollection)
        {
            if (queryCollection == null)
            {
                throw new ArgumentNullException(nameof(queryCollection));
            }
            var queryField =
                _options.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(_settings.QueryField);
            if (
                !queryCollection.TryGetValue(queryField, out var query) ||
                string.IsNullOrWhiteSpace(query.FirstOrDefault()))
            {
                return new RSqlQuery<T>(RSqlQueryExpressionHelper.True<T>());
            }

            var result = CreateAndAddCacheQuery(query.FirstOrDefault());
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"RSqlQuery<{typeof(T).FullName}>  query : ?{queryField}={query.FirstOrDefault()} -> {result.Value()}");
            }
            return result;
        }

        private IRSqlQuery<T> CreateAndAddCacheQuery(string query)
        {
            if (_settings.QueryCache != null
                && _settings.QueryCache.TryGetValue(query, out var resultCache))
            {
                return resultCache as IRSqlQuery<T>;
            }

            var antlrInputStream = new AntlrInputStream(query);
            var lexer = new RSqlQueryLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new RSqlQueryParser(commonTokenStream);
            var eval = parser.eval();
            var visitor = new RSqlDefaultQueryVisitor<T>(_options.Value.JsonSerializerOptions.PropertyNamingPolicy);
            var value = visitor.Visit(eval);
            var result = new RSqlQuery<T>(value);
            if (_settings.QueryCache == null)
            {
                return result;
            }
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions() {Size = 1024};
            _settings.OnCreateCacheEntry?.Invoke(memoryCacheEntryOptions);
            _settings.QueryCache.Set(query, result, memoryCacheEntryOptions);
            return result;
        }
    }
}
