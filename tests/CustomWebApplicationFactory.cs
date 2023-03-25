using CySim.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace CySim.Tests;

/*
 * Taken from https://github.com/dotnet/AspNetCore.Docs.Samples
 * Added null checking to it
 */
public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDesc = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

//            _ = dbContextDesc ?? throw new ArgumentNullException(nameof(dbContextDesc));

            services.Remove(dbContextDesc);

            var dbConnectionDesc = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));
            
//            _ = dbConnectionDesc ?? throw new ArgumentNullException(nameof(dbConnectionDesc));

            services.Remove(dbConnectionDesc);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<ApplicationDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        builder.UseEnvironment("Development");
    }
}
