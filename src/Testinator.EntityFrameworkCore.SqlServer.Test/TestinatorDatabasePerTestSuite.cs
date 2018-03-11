using MobileTransaction.Domain;
using System.Threading.Tasks;
using Testinator.EntityFrameworkCore.SqlServer.Test.Domain.Models;
using Xunit;

namespace Testinator.EntityFrameworkCore.SqlServer.Test
{
    public class TestinatorDatabasePerTestSuite
    {
        [Theory()]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task TestWidgetExists(int id)
        {
            using (var manager = new TestContextManager<TestContext>())
            {
                using (var ctx = new TestContext(manager.Options))
                {
                    var widget = await ctx.FindAsync<Widget>(id);
                    Assert.Equal(id, widget.Id);
                }
            }
        }

        [Fact]
        public async Task WhenIReadAnObjectFromTheDBItExists()
        {
            using (var manager = new TestContextManager<TestContext>())
            {
                using (var ctx = new TestContext(manager.Options))
                {
                    var widget = await ctx.FindAsync<Widget>(1);
                    Assert.Equal(1, widget.Id);
                }
            }
        }
    }
}

