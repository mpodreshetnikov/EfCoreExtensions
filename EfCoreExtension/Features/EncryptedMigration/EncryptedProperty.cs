using EfCoreExtension.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreExtension.Features.EncryptedMigration
{
    /// <summary>
    /// Object for encrypted property.
    /// </summary>
    internal class EncryptedProperty
    {
        /// <summary>
        /// Property builder.
        /// </summary>
        public PropertyBuilder<string> PropertyBuilder { get; set; }

        public ICryptoConverter CryptoConverter { get; set; }

        /// <summary>
        /// Max property value length.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Id of migration that property started to be encrypted from.
        /// </summary>
        public string MigrationId { get; set; }
    }
}
