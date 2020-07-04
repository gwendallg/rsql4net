using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using Antlr4.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using RSql4Net.Configurations;
using RSql4Net.Models.Queries;

namespace RSql4Net.Tests
{
    public static class Helper
    {
        public static IOptions<JsonOptions> JsonOptions(Action<JsonOptions> configuration = null)
        {

            var options = new JsonOptions()
            {
                JsonSerializerOptions = {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
            };

            configuration?.Invoke(options);
            return Options.Create(options);
        }

        public static Settings Settings(Action<Settings> configuration = null)
        {
            var result = new Settings();
            configuration?.Invoke(result);
            return result;
        }

        public static QueryCollection QueryCollection(params string[] args)
        {
            var data = new Dictionary<string, StringValues>();
            if (args.Length <= 0)
            {
                return new QueryCollection(data);
            }

            for (var i = 0; i < args.Length / 2; i++)
            {
                data[ args[i*2]]= new StringValues(args[i*2+1]);
            }
            return new QueryCollection(data);    
        }

        public static RSqlQueryParser Parser(string query)
        {
            var antlrInputStream = new AntlrInputStream(query);
            var lexer = new RSqlQueryLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            return new RSqlQueryParser(commonTokenStream);
        }

        public static Expression<Func<T, bool>> Expression<T>(string query,
            Action<Settings> settingsConfiguration = null, Action<JsonOptions> jsonOptionsConfiguration = null)
            where T : class
        {
            var settings = Settings(settingsConfiguration);
            var jsonOptions = JsonOptions(jsonOptionsConfiguration);
            var queryCollection = QueryCollection(
                jsonOptions.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(settings.QueryField),
                query);
            var mockLogger = new Mock<ILogger<T>>();
            mockLogger.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            var rSqlQueryModelBinder = new RSqlQueryModelBinder<T>(settings, jsonOptions, mockLogger.Object);
            return rSqlQueryModelBinder
                .Build(queryCollection)
                .Value();
        }

        public static Func<T, bool> Function<T>(string query, Action<Settings> settingsConfiguration = null,
            Action<JsonOptions> jsonOptionsConfiguration = null) where T : class
        {
            var expression = Expression<T>(query, settingsConfiguration, jsonOptionsConfiguration);
            return expression.Compile();
        }

        public static string GetJsonPropertyName(object obj, IOptions<JsonOptions> option = null)
        {
            var o = option ?? JsonOptions();
            return o.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName(obj.GetType().Name);
        }

        public static string GetChildJsonPropertyName(object obj, IOptions<JsonOptions> option =null)
        {
            var o = option ?? JsonOptions();
            return o.Value.JsonSerializerOptions.PropertyNamingPolicy.ConvertName("ChildP") + "." +
                GetJsonPropertyName(obj, o);
        }
        

    }
}
