using System;
using System.Collections.Generic;
using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if DEBUG
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using RSql4Net.Samples.Models;

namespace RSql4Net.Samples
{
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
            var namingStrategy = new CamelCaseNamingStrategy();

            var memoryCache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = 1024
            });
            
            services
                .AddControllers()
                .AddRSql(options =>
                    options
                        .NamingStrategy(namingStrategy)
                        .QueryCache(memoryCache)
                        .OnCreateCacheEntry((o) =>
                        {
                            o.Size = 1024;
                            o.SlidingExpiration = TimeSpan.FromSeconds(25);
                            o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                        })
                );
            services.AddSingleton(GetSampleData());
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RSql4NetOperationFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        private IList<Customer> GetSampleData()
        {
            var customerId = 1;
            var addressFaker = new Faker<Address>()
                .RuleFor(o => o.City, f => f.Address.City())
                .RuleFor(o => o.Street, f => f.Address.StreetAddress())
                .RuleFor(o => o.Country, f => f.Address.Country())
                .RuleFor(o => o.Zipcode, f => f.Address.ZipCode());

            var customerFaker = new Faker<Customer>()
                .CustomInstantiator(f => new Customer {Id = customerId++})
                .RuleFor(o => o.Address, addressFaker.Generate())
                .RuleFor(o => o.BirthDate, f => f.Date.Past(20))
                .RuleFor(o => o.Company, f => f.Company.CompanyName())
                .RuleFor(o => o.Credit, f => f.Random.Double())
                .RuleFor(o => o.Debit, f => f.Random.Double())
                .RuleFor(o => o.Email, f => f.Internet.Email())
                .RuleFor(o => o.Name, f => f.Name.LastName())
                .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(o => o.Username, f => f.Internet.UserNameUnicode())
                .RuleFor(o => o.Website, f => f.Internet.Url());

            return customerFaker.Generate(1000);
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
