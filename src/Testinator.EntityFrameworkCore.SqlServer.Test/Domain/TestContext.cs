using Microsoft.EntityFrameworkCore;
using Testinator.EntityFrameworkCore.SqlServer.Test.Domain.Models;

namespace MobileTransaction.Domain
{
    public class TestContext : DbContext
    {
        public DbSet<Widget> Widgets { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<Category> Categories { get; set; }


        public TestContext(DbContextOptions<TestContext> options) : base(options)
        {
            Database.SetCommandTimeout(150000);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreWidget>()
                   .HasKey(e => new { e.StoreId, e.WidgetId });

            modelBuilder.Entity<StoreWidget>()
                .HasOne(e => e.Store)
                .WithMany(e => e.StoreWidgets)
                .HasForeignKey(e =>e.StoreId);

            modelBuilder.Entity<StoreWidget>()
                .HasOne(e => e.Widget)
                .WithMany(e => e.StoreWidgets)
                .HasForeignKey(e => e.WidgetId);


            modelBuilder.Entity<Widget>()
                .HasOne(e => e.Category)
                .WithMany(e => e.Widgets);

            modelBuilder.Entity<Widget>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Category>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Store>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Category>()
                .HasData(
                    new Category { Id = 1, Description = "Blue Widgets" },
                    new Category { Id = 2, Description = "Red Widgets" },
                    new Category { Id = 3, Description = "Other Widgets" });

            modelBuilder.Entity<Widget>()
                .HasData(
                    new Widget { Id = 1, Description = "Blue with Spots", CategoryId = 1, Price = 1.0m },
                    new Widget { Id = 2, Description = "Blue with Lines", CategoryId = 1, Price = 1.0m },
                    new Widget { Id = 3, Description = "Red all over", CategoryId = 2, Price = 1.0m },
                    new Widget { Id = 4, Description = "Green and Purple", CategoryId = 3, Price = 1.0m });

            modelBuilder.Entity<Store>()
                .HasData(
                    new Store {
                        Id = 1,
                        Name = "The Widget Haus",
                        Address = "1 Credibility Street, Upper Whingeing",
                    },
                    new Store {
                        Id = 2,
                        Name = "Widgets R Us",
                        Address = "On every street corner",
                    },
                    new Store {
                        Id = 3,
                        Name = "Red Widget Specialists",
                        Address = "23 Red Street"
                    });


            modelBuilder.Entity<StoreWidget>()
                .HasData(
                    new StoreWidget { StoreId = 1, WidgetId = 1 },
                    new StoreWidget { StoreId = 1, WidgetId = 2 },
                    new StoreWidget { StoreId = 2, WidgetId = 2 },
                    new StoreWidget { StoreId = 2, WidgetId = 3 },
                    new StoreWidget { StoreId = 2, WidgetId = 4 },
                    new StoreWidget { StoreId = 3, WidgetId = 2 });
        }


        public string EscapeRawSqlString(string rawsql)
        {
            return rawsql.Replace("{", "{{").Replace("}", "}}");
        }
    }
}

