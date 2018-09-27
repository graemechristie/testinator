using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace Testinator.EntityFrameworkCore.SqlServer.Test
{
    public class TestContextManager<TContext> : IDisposable where TContext : DbContext
    {
        private static string ContextName => typeof(TContext).Name;

        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private static string _dbFilePath = null;
        private static string _logFilePath = null;

        private static string _cacheFolder;

        public string ConnectionString { get; private set; }

        public DbContextOptions<TContext> Options { get; private set; }
       
        public TestContextManager() : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Testinator"))
        {
        }

        public TestContextManager(string cacheFolder)
        {
            _cacheFolder = cacheFolder;

            EnsureCachedDbCreated();
            CreateClonedDatabaseInstance();
        }

        private void CreateClonedDatabaseInstance()
        {
            if (string.IsNullOrWhiteSpace(_dbFilePath))
                throw new InvalidOperationException("DBFilePath is not set. Call EnsureDbCreated before attempting to clone an instance");

            var testInstanceDbName = $"{ContextName}_{Guid.NewGuid()}";

            var clonedDbPath = Path.Combine(Directory.GetCurrentDirectory(), $"{testInstanceDbName}.mdf");
            var clonedLogPath = Path.Combine(Directory.GetCurrentDirectory(), $"{testInstanceDbName}_log.ldf");

            File.Copy(_dbFilePath, clonedDbPath, true);
            SetFileSecurity(clonedDbPath);
            File.Copy(_logFilePath, clonedLogPath, true);
            SetFileSecurity(clonedLogPath);

            // Rename the db in the mdf file (so it is unique) and attach it 
            var sql = $"CREATE DATABASE [{testInstanceDbName}] ON (FILENAME = N'{clonedDbPath}'),(FILENAME = N'{clonedLogPath}') FOR ATTACH;";
            ExecuteLocalDbNonQuerySql(sql);

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = testInstanceDbName,
                AttachDBFilename = clonedDbPath,
                MultipleActiveResultSets = true,
                IntegratedSecurity = true,
            };

            ConnectionString = connectionStringBuilder.ConnectionString;
            Options = new DbContextOptionsBuilder<TContext>()
                 .UseSqlServer(ConnectionString)
                 .Options;

        }

        private void EnsureCachedDbCreated()
        {
            _semaphore.Wait();

            try
            {
                if (_dbFilePath != null)
                    return;

                var dbName = $"{ContextName}_Cache_{Guid.NewGuid()}";
                var connectionStringBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = @"(LocalDB)\MSSQLLocalDB",
                    InitialCatalog = dbName,
                    MultipleActiveResultSets = true,
                    IntegratedSecurity = true,
                };

                var dummyOptions = new DbContextOptionsBuilder<TContext>()
                     .UseSqlServer(connectionStringBuilder.ToString())
                     .Options;

                string migrationVersion;

                using (var dummyContext = (TContext)(Activator.CreateInstance(typeof(TContext), dummyOptions)))
                {
                    migrationVersion = dummyContext.Database.GetMigrations().LastOrDefault();
                    if (string.IsNullOrWhiteSpace(migrationVersion))
                        throw new InvalidOperationException("Cannot get version of context");
                }

                var fileBaseName = $"{ContextName}_{migrationVersion}";
                var dbFilePath = Path.Combine(_cacheFolder, $"{fileBaseName}.mdf");
                var logFilePath = Path.Combine(_cacheFolder, $"{fileBaseName}_log.ldf");

                if (File.Exists(dbFilePath))
                {
                    _dbFilePath = dbFilePath;
                    _logFilePath = logFilePath;
                    return;
                }

                connectionStringBuilder.AttachDBFilename = dbFilePath;
                var connectionString = connectionStringBuilder.ToString();

                var options = new DbContextOptionsBuilder<TContext>()
                     .UseSqlServer(connectionString)
                     .Options;

                Directory.CreateDirectory(_cacheFolder);

                using (var context = (TContext)(Activator.CreateInstance(typeof(TContext), options)))
                {
                    context.Database.EnsureDeleted();
                    context.Database.Migrate();
                }

                // Detach the DB (so the file is not locked)
                var sql = $"ALTER DATABASE [{dbName}] SET OFFLINE WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{dbName}'";

                ExecuteLocalDbNonQuerySql(sql);

                _dbFilePath = dbFilePath;
                _logFilePath = logFilePath;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void ExecuteLocalDbNonQuerySql(string sql)
        {
            var masterConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true,
            };

            using (SqlConnection conn = new SqlConnection(masterConnectionStringBuilder.ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void SetFileSecurity(string filePath)
        {
            FileInfo fInfo = new FileInfo(filePath);
            FileSecurity fSecurity = fInfo.GetAccessControl();
            fSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            fInfo.SetAccessControl(fSecurity);
        }

        public void Dispose()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                return;

            using (var context = (TContext)(Activator.CreateInstance(typeof(TContext), Options)))
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}
