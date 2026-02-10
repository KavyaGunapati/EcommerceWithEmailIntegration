using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using DataAccess.Context;

namespace ECommerceWithEmail.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection? _connection;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Ensure Program.cs registers Sqlite when running tests
            // create a unique in-memory sqlite database per factory instance
            var name = Guid.NewGuid().ToString("N");
            var connStr = $"DataSource=file:memdb_{name}?mode=memory&cache=shared";
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    ["UseInMemorySqlite"] = "true",
                    ["ConnectionStrings:SqliteConnection"] = connStr
                };
                config.AddInMemoryCollection(dict);
            });

            // keep the connection open for lifetime of factory to preserve in-memory DB
            _connection = new SqliteConnection(connStr);
            _connection.Open();

            builder.ConfigureServices(services =>
            {
                // Override authentication to use testing scheme
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestingAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestingAuthHandler.SchemeName;
                }).AddScheme<AuthenticationSchemeOptions, TestingAuthHandler>
                (TestingAuthHandler.SchemeName, options => { });

                // Build the service provider and create the database schema (Program will have registered DbContext using Sqlite)
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
        }

    }
}