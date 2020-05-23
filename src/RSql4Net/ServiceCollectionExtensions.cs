using System;
using Microsoft.Extensions.DependencyInjection;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;

namespace RSql4Net
{
    /// <summary>
    ///     Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the RS ql.
        /// </summary>
        /// <returns>The RS ql.</returns>
        /// <param name="mvcBuilder">Services.</param>
        /// <param name="optionsAction">options action.</param>
        public static IMvcBuilder AddRSql(this IMvcBuilder mvcBuilder,
            Action<SettingsBuilder> optionsAction = null)
        {
            if (mvcBuilder == null)
            {
                throw new ArgumentNullException(nameof(mvcBuilder));
            }

            var builder = new SettingsBuilder();
            optionsAction?.Invoke(builder);
            var settings = builder.Build();
            mvcBuilder.Services.AddSingleton(settings);
            mvcBuilder.AddMvcOptions(c =>
            {
                c.ModelBinderProviders.Insert(0,
                    new PageableModelBinderProvider(settings));
                c.ModelBinderProviders.Insert(1,
                    new QueryModelBinderProvider(settings));
            });
            return mvcBuilder;
        }
    }
}
