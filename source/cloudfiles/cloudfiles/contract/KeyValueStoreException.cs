using System;

namespace cloudfiles.contract
{
    [Serializable]
    public class KeyValueStoreException : Exception
    {
        public KeyValueStoreException(string message) : base(message) { }
        public KeyValueStoreException(string message, Exception innerException) : base(message, innerException){}
    }
}