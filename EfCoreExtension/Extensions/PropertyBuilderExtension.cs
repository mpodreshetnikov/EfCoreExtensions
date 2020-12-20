using System;
using System.Linq;
using EfCoreExtension.Abstractions;
using EfCoreExtension.Features.EncryptedMigration;
using EfCoreExtension.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCoreExtensions.Extensions
{
    /// <summary>
    /// Extensions for <see cref="PropertyBuilder"/>.
    /// </summary>
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// Apply encryption for the property. If migration is provided then apply old value encryption during the migration.
        /// </summary>
        /// <param name="propertyBuilder">Property.</param>
        /// <param name="migrationType">Type of Migration that property started to be encrypted from.</param>
        public static PropertyBuilder<string> EncryptedWith(this PropertyBuilder<string> propertyBuilder, ICryptoConverter cryptoConverter, int? maxLength = default, Type migrationType = default)
        {
            ArgumentUtils.ThrowIfNull(propertyBuilder, nameof(propertyBuilder));
            ArgumentUtils.ThrowIfNull(cryptoConverter, nameof(cryptoConverter));

            MigrationAttribute migrationAttribute = default;
            if (!(migrationType is null))
            {
                if (!migrationType.IsSubclassOf(typeof(Migration)))
                {
                    throw new ArgumentException("Migration type must be inherited from Migration.", nameof(migrationType));
                }

                migrationAttribute = (MigrationAttribute)migrationType
                    .GetCustomAttributes(typeof(MigrationAttribute), true).FirstOrDefault();
                if (migrationAttribute is null)
                {
                    throw new ArgumentException("Provided Migration type has no MigrationAttribute.", nameof(migrationType));
                }
            }

            // Add this value converter into migration query.
            EncryptingMigrator.AddEncryptedProperty(new EncryptedProperty
            {
                PropertyBuilder = propertyBuilder,
                CryptoConverter = cryptoConverter,
                MaxLength = maxLength,
                MigrationId = migrationAttribute?.Id,
            });

            return propertyBuilder;
        }

        public static PropertyBuilder<string> EncryptedWith<T>(this PropertyBuilder<string> propertyBuilder, int? maxLength = default, Type migrationType = default)
            where T : ICryptoConverter, new()
        {
            return EncryptedWith(propertyBuilder, new T(), maxLength, migrationType);
        }
    }
}
