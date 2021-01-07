using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreExtensions.EncryptedMigration.ValueConverters;
using EfCoreExtensions.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EfCoreExtensions.Features.EncryptedMigration
{
    /// <summary>
    /// Migrator that provide logic for updating old values during migrations with encrypting values.
    /// </summary>
    internal static class EncryptingMigrator
    {
        private static IEnumerable<string> pendingMigrations;
        private static readonly List<EncryptedProperty> EncryptedProperties = new List<EncryptedProperty>();

        internal static void AddEncryptedProperty(EncryptedProperty encryptedProperty)
        {
            ArgumentUtils.ThrowIfNull(encryptedProperty, nameof(encryptedProperty));
            AddConverters(encryptedProperty);
            EncryptedProperties.Add(encryptedProperty);
        }

        private static void AddConverters(EncryptedProperty encryptedProperty)
        {
            // Apply encrypted conversion.
            var cryptoValueConverter = new CryptoValueConverter(encryptedProperty.CryptoConverter);
            encryptedProperty.PropertyBuilder.HasConversion(cryptoValueConverter);

            if (encryptedProperty.MaxLength.HasValue)
            {
                var maxEncryptedLength = encryptedProperty.CryptoConverter.GetMaximalOverheadedLength(encryptedProperty.MaxLength.Value);
                encryptedProperty.PropertyBuilder.HasMaxLength(maxEncryptedLength);
            }
        }

        internal static void MigrateWithEncriptingMigrator(DatabaseFacade databaseFacade)
        {
            var dbContext = (databaseFacade as IDatabaseFacadeDependenciesAccessor).Context;
            pendingMigrations = dbContext.Database.GetPendingMigrations();
            databaseFacade.Migrate();
            MigrateEncryptedPropertiesAsync(dbContext).GetAwaiter().GetResult();
        }

        internal static async Task MigrateWithEncriptingMigratorAsync(DatabaseFacade databaseFacade, CancellationToken cancellationToken = default)
        {
            var dbContext = (databaseFacade as IDatabaseFacadeDependenciesAccessor).Context;
            pendingMigrations = dbContext.Database.GetPendingMigrations();
            await databaseFacade.MigrateAsync(cancellationToken);
            await MigrateEncryptedPropertiesAsync(dbContext);
        }

        /// <summary>
        /// Migrate old data for encrypted properties if needed.
        /// </summary>
        private static async Task MigrateEncryptedPropertiesAsync(DbContext dbContext)
        {
            foreach (var encryptedProperty in EncryptedProperties)
            {
                // Convert old data.
                if (!(encryptedProperty.MigrationId is null))
                {
                    // Check that migration exists or not.
                    var isValuesNeedsToBeEncrypted = pendingMigrations.Contains(encryptedProperty.MigrationId);

                    if (isValuesNeedsToBeEncrypted)
                    {
                        var propertyMetadata = encryptedProperty.PropertyBuilder.Metadata;

                        var entityType = dbContext.Model.FindEntityType(propertyMetadata.DeclaringEntityType.ClrType.FullName);

                        var tableName = entityType.GetAnnotation("Relational:TableName").Value;
                        var pkProps = entityType.FindPrimaryKey().Properties.Select(_ => _.Name);
                        var targetColumnName = propertyMetadata.Name;

                        dbContext.Database.OpenConnection();

                        // Select rows that needs to be updated with encryption.
                        IEnumerable<IDataRecord> rowsToUpdate;
                        using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                        {
                            var columns = AtomicUtils.Join(",", pkProps.Append(targetColumnName).Select(_ => $"\"{_}\""));
                            command.CommandText = $"SELECT {columns} FROM \"{tableName}\"";

                            using var result = await command.ExecuteReaderAsync();
                            rowsToUpdate = result.Cast<IDataRecord>().ToList();
                        }

                        // Update rows with encryption.
                        await dbContext.Database.BeginTransactionAsync();
                        foreach (var row in rowsToUpdate)
                        {
                            var whereValues = pkProps
                                .Select(propName => $"\"{propName}\"='{row.GetValue(row.GetOrdinal(propName))}'");
                            var newEncryptedValue = encryptedProperty.CryptoConverter.Encrypt(row.GetString(row.GetOrdinal(targetColumnName)));
                            await dbContext.Database.ExecuteSqlRawAsync($"UPDATE \"{tableName}\" SET \"{targetColumnName}\"='{newEncryptedValue}' WHERE {AtomicUtils.Join(" and ", whereValues)}");
                        }
                        await dbContext.Database.CommitTransactionAsync();
                        dbContext.Database.CloseConnection();
                    }
                }
            }

            EncryptedProperties.Clear();
        }
    }
}
