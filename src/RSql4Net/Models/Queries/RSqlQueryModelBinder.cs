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
    public class RSqlQueryModelBinder<T> : IModelBinder where T: class
    {
        private readonly Settings _settings;

        public RSqlQueryModelBinder(Settings settings)
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
        public IRSqlQuery<T> Build(IQueryCollection queryCollection)
        {
            if (
                !queryCollection.TryGetValue(_settings.QueryField, out var query) ||
                string.IsNullOrWhiteSpace(query.FirstOrDefault()))
            {
                return  new RSqlQuery<T>(RSqlQueryExpressionHelper.True<T>());
            }
           
            return CreateAndAddCacheQuery(query.FirstOrDefault());
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
            var or = parser.or();
            var visitor = new RSqlDefaultQueryVisitor<T>(_settings.NamingStrategy);
            var value = visitor.Visit(or);
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
