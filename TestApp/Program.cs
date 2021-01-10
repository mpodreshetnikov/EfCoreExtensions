using System.Linq;
using System.Threading.Tasks;
using EfCoreExtensions.EncryptedMigration;
﻿using System;
using System.Linq;
using EfCoreExtensions.EncryptedMigration;
using EfCoreExtensions.Searching;
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
                    Surname = "Podreshetnikov",
                    Nickname = "Max",
                    Age = 19,
                });
                dbContext.Users.Add(new User
                {
                    Name = "Maxim",
                    Surname = "Peretz",
                    Nickname = "Max",
                    Age = 20,
                });
                dbContext.Users.Add(new User
                {
                    Name = "Ivan",
                    Surname = "Podresh",
                    Age = 12,
                });
            }

            await dbContext.SaveChangesAsync();
            var usersQuery = dbContext.Users.SearchInTextProps("Po", u => u.Nickname, u => u.Surname);
        }
    }
}
