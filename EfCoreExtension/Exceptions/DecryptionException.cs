using System;

namespace EfCoreExtension.Exceptions
{
    /// <summary>
    /// Exception for decryption failures.
    /// </summary>
    [Serializable]
    public class DecryptionException : Exception
    {
        public DecryptionException()
        {
        }

        public DecryptionException(string message) : base(message)
        {
        }

        public DecryptionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
