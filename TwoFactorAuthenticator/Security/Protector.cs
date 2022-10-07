using System;
using System.Security.Cryptography;

namespace TwoFactorAuthenticator.Security
{
    internal sealed class Protector
    {
        private static readonly byte[] Entropy = new byte[16];

        static Protector()
        {
            Random rnd = new Random();
            rnd.NextBytes(Entropy);
        }

        public static byte[] Protect(byte[] data)
        {
            // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
            // only by the same current user.
            return ProtectedData.Protect(data, Entropy, DataProtectionScope.CurrentUser);
        }

        public static byte[] Unprotect(byte[] data)
        {
            //Decrypt the data using DataProtectionScope.CurrentUser.
            return ProtectedData.Unprotect(data, Entropy, DataProtectionScope.CurrentUser);
        }
    }
}