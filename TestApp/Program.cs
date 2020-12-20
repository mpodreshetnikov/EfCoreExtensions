using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestApp.Entities;

namespace TestApp
{
    internal static class Program
    {
        private static void Main()
        {
            var dbContext = new AppDbContext();
            dbContext.Database.Migrate();

            dbContext.Users.Add(new User
            {
                Name = "Maxim",
                Age = 19,
            });
            dbContext.SaveChanges();
            var users = dbContext.Users.ToList();
        }
    }
}
