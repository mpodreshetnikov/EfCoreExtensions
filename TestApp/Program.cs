using System.Linq;
using EfCoreExtensions.EncryptedMigration;
using EfCoreExtensions.Pagination;
using TestApp.Entities;

namespace TestApp
{
    internal static class Program
    {
        private static void Main()
        {
            var dbContext = new AppDbContext();
            dbContext.Database.MigrateWithEncryptingMigrator();

            if (dbContext.Users.Count() <= 2)
            {
                dbContext.Users.Add(new User
                {
                    Name = "Maxim",
                    Age = 19,
                });
                dbContext.Users.Add(new User
                {
                    Name = "Ivan",
                    Age = 12,
                });
            }

            dbContext.SaveChanges();
            var users = dbContext.Users.TakePage(1, 10).ToList();
        }
    }
}
