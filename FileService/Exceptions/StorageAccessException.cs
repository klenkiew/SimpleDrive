using System;
using System.Runtime.Serialization;

namespace FileService.Exceptions
{
    internal class StorageAccessException : Exception
    {
        public StorageAccessException()
        {
        }

        public StorageAccessException(string message) : base(message)
        {
        }

        public StorageAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StorageAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}