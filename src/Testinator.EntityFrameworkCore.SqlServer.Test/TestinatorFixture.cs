using Microsoft.EntityFrameworkCore;
using System;

namespace Testinator.EntityFrameworkCore.SqlServer.Test
{
    public class TestinatorFixture<TContext> : IDisposable where TContext : DbContext
    {
        public DbContextOptions<TContext> Options => _testContextManager.Options;

        private TestContextManager<TContext> _testContextManager;

        public TestinatorFixture()
        {
            _testContextManager = new TestContextManager<TContext>();
        }

        public void Dispose()
        {
            if (_testContextManager == null)
                return;

            _testContextManager.Dispose();
        }
    }
}
