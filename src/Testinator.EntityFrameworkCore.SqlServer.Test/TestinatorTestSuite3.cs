using MobileTransaction.Domain;
using System.Threading.Tasks;
using Testinator.EntityFrameworkCore.SqlServer.Test.Domain.Models;
using Xunit;

namespace Testinator.EntityFrameworkCore.SqlServer.Test
{
    public class TestinatorTestSuite3 : IClassFixture<TestinatorFixture<TestContext>>
    {
        protected TestinatorFixture<TestContext> Fixture;

        public TestinatorTestSuite3(TestinatorFixture<TestContext> fixture)
        {
            Fixture = fixture;

        }
        [Theory()]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task TestWidgetExists(int id)
        {
            using (var ctx = new TestContext(Fixture.Options))
            {
                var widget = await ctx.FindAsync<Widget>(id);
                Assert.Equal(id, widget.Id);
            }
        }
    }
}

