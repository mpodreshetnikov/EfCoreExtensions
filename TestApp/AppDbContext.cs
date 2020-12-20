using EfCoreExtension.Abstractions;
using EfCoreExtension.DefaultImplementations;
using EfCoreExtensions.Extensions;
using Microsoft.EntityFrameworkCore;
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
                .Property(u => u.Name)
                .EncryptedWith(cryptoConverter, maxLength: 20, migrationType: typeof(Migrations.EncryptName));

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=TestEfCoreExtensionApplication;Username=eztaxsolver;Password=eztaxsolver");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
