using System.Linq;
using EfCoreExtensions.EncryptedMigration;
using TestApp.Entities;

namespace TestApp
{
    internal static class Program
    {
        private static void Main()
        {
            var dbContext = new AppDbContext();
            dbContext.Database.MigrateWithEncryptingMigrator();

            if (dbContext.Users.Count() <= 1)
            {
                dbContext.Users.Add(new User
                {
                    Name = "Maxim",
                    Age = 19,
                });
            }

            dbContext.SaveChanges();
            var users = dbContext.Users.ToList();
        }
    }
}
