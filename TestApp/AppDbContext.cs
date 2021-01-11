using System;
using EfCoreExtensions.EncryptedMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestApp.Entities;

namespace TestApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly ICryptoConverter cryptoConverter;

        public AppDbContext()
        {
            cryptoConverter = new DefaultCryptoConverter("1234567890123456", "1234");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.SSN)
                .EncryptedWith(cryptoConverter, maxLength: 12, migrationType: typeof(Migrations.ImproveTestUserModel));

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=TestEfCoreExtensionApplication;Username=eztaxsolver;Password=eztaxsolver");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
