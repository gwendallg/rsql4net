using System;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using RSql4Net.Configurations;

namespace RSql4Net.Models.Queries
{
    public class QueryModelBinder<T> : IModelBinder
    {
        private readonly Settings _settings;

        public QueryModelBinder(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
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
        public IQuery<T> Build(IQueryCollection queryCollection)
        {
            if (
                !queryCollection.TryGetValue(_settings.QueryField, out var query) ||
                string.IsNullOrWhiteSpace(query.FirstOrDefault()))
            {
                return  new Query<T>(QueryExpressionHelper.True<T>());
            }
           
            return CreateAndAddCacheQuery(query.FirstOrDefault());
        }

        private IQuery<T> CreateAndAddCacheQuery(string query)
        {
            if (_settings.QueryCache != null
                && _settings.QueryCache.TryGetValue(query, out var resultCache))
            {
                return resultCache as IQuery<T>;
            }

            var antlrInputStream = new AntlrInputStream(query);
            var lexer = new QueryLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new QueryParser(commonTokenStream);
            var or = parser.or();
            var visitor = new DefaultQueryVisitor<T>(_settings.NamingStrategy);
            var value = visitor.Visit(or);
            var result = new Query<T>(value);
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
