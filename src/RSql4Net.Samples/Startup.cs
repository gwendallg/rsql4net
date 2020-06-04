using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if DEBUG
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.OpenApi.Models;
using RSql4Net.SwaggerGen;

namespace RSql4Net.Samples
{
    public static class StringUtils
    {
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }

    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

        public override string ConvertName(string name)
        {
            // Conversion to other naming convention goes here. Like SnakeCase, KebabCase etc.
            return name.ToSnakeCase();
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // message rsql query cache sample
            var memoryCache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = 1024
            });

            services
                .AddControllers()
                .AddJsonOptions(options =>
                    {
                        options
                            .JsonSerializerOptions.IgnoreNullValues = true;
                        options
                            .JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
                    }
                )
                .AddRSql(options =>
                    options
                        // use the memory cache
                        .QueryCache(memoryCache)
                        // when add a new query in cache ...
                        .OnCreateCacheEntry((o) =>
                        {
                            o.Size = 1024;
                            o.SlidingExpiration = TimeSpan.FromSeconds(25);
                            o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                        })
                );
            services.AddSingleton(Helper.Fake());
            services.AddSwaggerGen(c =>
            {
                // add supported to Rsql SwaggerGen Documentation
                c.OperationFilter<RSqlOperationFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            #if DEBUG
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            #endif

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
