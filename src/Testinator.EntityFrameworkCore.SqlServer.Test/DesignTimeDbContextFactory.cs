using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MobileTransaction.Domain;

namespace MobileTransactionService
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestContext>
    {
        public TestContext CreateDbContext(string[] args)
        {

            var builder = new DbContextOptionsBuilder<TestContext>();

            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=testinatortest;Trusted_Connection=True;");

            return new TestContext(builder.Options);
        }
    }
}
