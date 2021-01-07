using System.Threading;
using System.Threading.Tasks;
using EfCoreExtensions.Features.EncryptedMigration;
using EfCoreExtensions.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EfCoreExtensions.EncryptedMigration
{
    public static class DatabaseFacadeExtensions
    {
        public static void MigrateWithEncryptingMigrator(this DatabaseFacade databaseFacade)
        {
            ArgumentUtils.ThrowIfNull(databaseFacade, nameof(databaseFacade));
            EncryptingMigrator.MigrateWithEncriptingMigrator(databaseFacade);
        }

        public static Task MigrateWithEncryptingMigratorAsync(this DatabaseFacade databaseFacade, CancellationToken cancellationToken = default)
        {
            ArgumentUtils.ThrowIfNull(databaseFacade, nameof(databaseFacade));
            return EncryptingMigrator.MigrateWithEncriptingMigratorAsync(databaseFacade, cancellationToken);
        }
    }
}
