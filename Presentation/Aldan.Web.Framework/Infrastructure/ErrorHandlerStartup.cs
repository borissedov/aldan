using Aldan.Core.Infrastructure;
using Aldan.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aldan.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring exceptions and errors handling on application startup
    /// </summary>
    public class ErrorHandlerStartup : IAldanStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //exception handling
            application.UseAldanExceptionHandler();

            //handle 400 errors (bad request)
            application.UseBadRequestResult();

            //handle 404 errors (not found)
            application.UsePageNotFound();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 0; //error handlers should be loaded first
    }
}