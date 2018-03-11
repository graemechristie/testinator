using Microsoft.EntityFrameworkCore;
using MobileTransaction.Domain;
using System;
using System.Data.SqlClient;

namespace Testinator.EntityFrameworkCore.SqlServer.Test
{
    public class LocalDbFixture : IDisposable
    {
        public LocalDbFixture()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = $"LocalDbFixtureTest_{Guid.NewGuid()}",
                MultipleActiveResultSets = true,
                IntegratedSecurity = true,
            };

            ConnectionString =  connectionStringBuilder.ToString();

            Options = new DbContextOptionsBuilder<TestContext>()
                 .UseSqlServer(ConnectionString)
                 .Options;

            using (var context = new TestContext(Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        public void Dispose()
        {
            using (var context = new TestContext(Options))
            {
                context.Database.EnsureDeleted();
            }
        }

        public string ConnectionString { get; private set; }

        public DbContextOptions<TestContext> Options { get; private set; }
    }
}
