# Testinator

Lightning fast test database instances for EF Core databases (Sql Server only)  

> Its like the Terminator - but for slow running database tests

This is currently early release beta software

Running unit tests against SQL LocalDb instances can be very slow, particularly if you want to create a new Db Instance per Test or Per Collection. The EF Core Database.EnsureCreated() call can take 10-20 seconds. Repeat this by a few dozen tests and this can get very time consuming.

This library will give you a TestContextManager class, that will create a single instance of the database per migration, and store the created mdf and ldf files in a cache directory (%AppData%\Roaming\Testinator by default). Then, for this and each subsequent request for that database the cached database files are copied to the working directory and the database name is changed to a unique database name. The TestManager instance will then have it's ConnectionString (and also for convenenience an Options propery created using teh connection string) set to the Connection string for this database instance.

When the TestManager instance is Disposed, the database instance will be deleted.

## Installation

Install-Package Testinator.EntityFrameworkCore.SqlServer -Pre

## Usage 

From within your test, Just create an instance of TestContextManager<TContext> in a using block, where TContext is your DbContext Type. You can then create an instance of your context using the ConnectionString property (or the Options property, if you don't need to customize the options) on the testContextManager instance)

e.g.

```C#
  [Fact]
  public async Task WhenIReadAnObjectFromTheDBItExists()
  {
      using (var manager = new TestContextManager<MyDbContext>())
      {
          using (var ctx = new MyDbContext(manager.Options))
          {
              var widget = await ctx.FindAsync<Widget>(1);
              Assert.Equal(1, widget.Id);
          }
      }
  }
```

Alternatively, if you are using Xunit and want a Database instance per Class/Collection, you could use a TestFixture such as the following:

```C#
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
```



