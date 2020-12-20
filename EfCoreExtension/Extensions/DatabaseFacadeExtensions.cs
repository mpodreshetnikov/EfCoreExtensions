using System.Threading;
using System.Threading.Tasks;
using EfCoreExtension.Features.EncryptedMigration;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EfCoreExtension.Extensions
{
    public static class DatabaseFacadeExtensions
    {
        public static void MigrateWithEncryptingMigrator(this DatabaseFacade databaseFacade)
        {
            EncryptingMigrator.MigrateWithEncriptingMigrator(databaseFacade);
        }

        public static Task MigrateWithEncryptingMigratorAsync(this DatabaseFacade databaseFacade, CancellationToken cancellationToken = default)
        {
            return EncryptingMigrator.MigrateWithEncriptingMigratorAsync(databaseFacade, cancellationToken);
        }
    }
}
