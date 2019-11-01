using Aldan.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aldan.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of DbContextOptionsBuilder
    /// </summary>
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// SQL Server specific extension method for Microsoft.EntityFrameworkCore.DbContextOptionsBuilder
        /// </summary>
        /// <param name="optionsBuilder">Database context options builder</param>
        /// <param name="services">Collection of service descriptors</param>
        public static void UseSqlServerWithLazyLoading(this DbContextOptionsBuilder optionsBuilder, IServiceCollection services)
        {
            var aldanConfig = services.BuildServiceProvider().GetRequiredService<AldanConfig>();

            var dbContextOptionsBuilder = optionsBuilder.UseLazyLoadingProxies();
            
            dbContextOptionsBuilder.UseSqlServer(aldanConfig.Data.ConnectionString);
        }
    }
}
