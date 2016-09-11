using System;

namespace DuwMonitorWebJob
{
    public class DuwStorageException: Exception
    {
        public DuwStorageException(string message) : base(message)
        {
        }

        public DuwStorageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}