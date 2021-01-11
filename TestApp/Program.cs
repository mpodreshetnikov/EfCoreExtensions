using System.Linq;
using System.Threading.Tasks;
using EfCoreExtensions.EncryptedMigration;
using EfCoreExtensions.Searching;
using TestApp.Entities;

namespace TestApp
{
    internal static class Program
    {
        private static async Task Main()
        {
            var dbContext = await CreateContextAndSeedDb();

                

            var usersQuery = dbContext.Users.SearchInTextProps("ivan", u => u.Mother.FirstName, u => u.Mother.LastName);
        }

        private static async Task<AppDbContext> CreateContextAndSeedDb()
        {
            var dbContext = new AppDbContext();
            dbContext.Database.MigrateWithEncryptingMigrator();

            dbContext.Users.RemoveRange(dbContext.Users);
            var user1 = dbContext.Users.Add(new User
            {
                FirstName = "Maxim",
                LastName = "Podreshetnikov",
                SSN = "123456789012",
                Age = 19,
            }).Entity;
            var user2 = dbContext.Users.Add(new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                SSN = "123456789012",
                Age = 20,
            }).Entity;
            var user3 = dbContext.Users.Add(new User
            {
                FirstName = "Karoline",
                LastName = "Ivanova",
                SSN = "123456789012",
                Age = 12,
            }).Entity;
            await dbContext.SaveChangesAsync();

            user1.Mother = user3;
            user2.Mother = user3;
            await dbContext.SaveChangesAsync();

            return dbContext;
        }
    }
}
