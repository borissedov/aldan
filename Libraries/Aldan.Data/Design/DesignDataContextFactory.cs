using System.IO;
using Aldan.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Aldan.Data.Design
{
    public class DesignDataContextFactory: IDesignTimeDbContextFactory<AldanObjectContext>
    {
        public AldanObjectContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "/../../Presentation/Aldan.Web/")
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            var config = new AldanConfig();
            configuration.GetSection("Aldan").Bind(config);

            string connectionString = config.Data.ConnectionString;
            
            var builder = new DbContextOptionsBuilder<AldanObjectContext>()
                .UseSqlServer(connectionString);
            return new AldanObjectContext(builder.Options);
        }
    }
}