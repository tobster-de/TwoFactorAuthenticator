using System;

namespace TwoFactorAuthenticator.QrCoder
{
    public class QrException : Exception
    {
        public QrException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
