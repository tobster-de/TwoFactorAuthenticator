using System;

namespace TwoFactorAuthenticator
{
    public class QRException : Exception
    {
        public QRException(string message) : base(message)
        { }

        public QRException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
