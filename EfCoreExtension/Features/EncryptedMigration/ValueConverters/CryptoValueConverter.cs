﻿using System;
using System.Linq.Expressions;
using EfCoreExtension.Exceptions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EfCoreExtensions.EncryptedMigration.ValueConverters
{
    /// <summary>
    /// Value converter to store data in DB encrypted.
    /// </summary>
    internal class CryptoValueConverter : ValueConverter<string, string>
    {
        /// <summary>
        /// Wrapper to build <see cref="CryptoValueConverter"/> with dependencies.
        /// </summary>
        private class Wrapper
        {
            private readonly ICryptoConverter cryptoConverter;

            public Wrapper(ICryptoConverter cryptoConverter)
            {
                this.cryptoConverter = cryptoConverter;
            }

            private string Decrypt(string value)
            {
                try
                {
                    return cryptoConverter.Decrypt(value);
                }
                catch (DecryptionException)
                {
                    return "";
                }
            }

            public Expression<Func<string, string>> To => value => cryptoConverter.Encrypt(value);

            public Expression<Func<string, string>> From => value => Decrypt(value);
        }

        public CryptoValueConverter(ICryptoConverter cryptoConverter, ConverterMappingHints mappingHints = default)
            : this(new Wrapper(cryptoConverter), mappingHints)
        {
        }

        private CryptoValueConverter(Wrapper wrapper, ConverterMappingHints mappingHints)
            : base(wrapper.To, wrapper.From, mappingHints)
        {
        }
    }
}
