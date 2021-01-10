using System.Linq;
using System.Threading.Tasks;
using EfCoreExtensions.EncryptedMigration;
using EfCoreExtensions.Pagination;
using TestApp.Entities;

namespace TestApp
{
    internal static class Program
    {
        private static async Task Main()
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

            await dbContext.SaveChangesAsync();
            var users = await dbContext.Users.PagedAsync(new PagedQuery
            {
                FromPage = 1,
                PageElementsCount = 10,
                PagesCount = 3,
            });
        }
    }
}
