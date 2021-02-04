using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
#if DEBUG
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.OpenApi.Models;
using Prometheus;
using RSql4Net.SwaggerGen;

namespace RSql4Net.Samples
{

    public class Startup
    {
     
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
            services.AddHealthChecks();
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
                endpoints.MapMetrics();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = ((context, report) =>
                    {
                        context.Response.ContentType = "application/json; charset=utf-8";
                        var options = new JsonWriterOptions
                        {
                            Indented = true
                        };
                        using var stream = new MemoryStream();
                        using (var writer = new Utf8JsonWriter(stream, options))
                        {
                            writer.WriteStartObject();
                            writer.WriteString("status", report.Status.ToString());
                            writer.WriteEndObject();
                        }
                        var json = Encoding.UTF8.GetString(stream.ToArray());
                        return context.Response.WriteAsync(json);
                    })
                });
            });
        }
    }
}
