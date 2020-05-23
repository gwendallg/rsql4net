using Microsoft.AspNetCore.Builder;
using RSql4Net.Configurations;

namespace RSql4Net
{
    /// <summary>
    ///     Application builder extensions.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     Gets the Rsql settings.
        /// </summary>
        /// <returns>The Rsql settings.</returns>
        /// <param name="app">App.</param>
        public static Settings GetRSqlSettings(this IApplicationBuilder app)
        {
            return (Settings)app.ApplicationServices.GetService(typeof(Settings));
        }
    }
}
