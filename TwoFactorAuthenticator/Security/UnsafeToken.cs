using System;

namespace TwoFactorAuthenticator.Security
{
    /// <summary>
    /// Unsafe token class for visualization.
    /// </summary>
    public class UnsafeToken : IDisposable
    {
        private int _value;
        private readonly int _length;
        
        /// <summary>
        /// Create an unsafe token for display and other UI purposes.
        /// </summary>
        public UnsafeToken(int value, int length)
        {
            _value = value;
            _length = length;
        }

        /// <summary>
        /// Create a displayable token using a provided password token.
        /// </summary>
        public static UnsafeToken FromPasswordToken(PasswordToken token)
        {
            int value = 0;
            int factor = 1;
            for (int i = token.Length - 1; i >= 0; i--)
            {
                value += token[i] * factor;
                factor *= 10;
            }

            return new UnsafeToken(value, token.Length);
        }

        /// <inheritdoc />
        public override string ToString() => _value.ToString($"D{_length}");

        /// <inheritdoc />
        public void Dispose()
        {
            _value = 0;
        }
    }
}