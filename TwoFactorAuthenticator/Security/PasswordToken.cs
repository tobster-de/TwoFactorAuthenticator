using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("TwoFactorAuthenticator.Tests")]

namespace TwoFactorAuthenticator.Security
{
    /// <summary>
    ///     Password token class for handling tokens in a protected fashion. 
    /// </summary>
    public class PasswordToken : IDisposable, IEquatable<PasswordToken>
    {
        private byte[] _storage;
        private int _hash;

        /// <summary>
        ///     Sets the digit at the provided index.
        /// </summary>
        /// <param name="index">Index of the digit</param>
        public byte this[int index]
        {
            internal get => this.GetDigit(index);
            set => this.SetDigit(index, value);
        }

        /// <summary>
        ///     The maximum length this token can be filled to.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        ///     Current length of this token.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        ///     Create a new token that can hold up to the provided count of digits.
        /// </summary>
        /// <param name="maxLength">The token will hold up to this count of digits.</param>
        public PasswordToken(int maxLength = 6)
        {
            this.MaxLength = maxLength;
            this.Init();
        }

        /// <summary>
        ///     Create a token from provided code. 
        /// </summary>
        /// <param name="passcode">The passcode as an array of bytes.</param>
        /// <returns>The generated passwort token.</returns>
        /// <remarks>
        /// <para>
        ///     Handle with care and demonstration / test purposes: using this method implies the passcode
        ///     is held somewhere in memory by your code. This is most likely to be completely unprotected.
        /// </para>
        /// <para>
        ///     Use <see cref="AppendDigit"/>, <see cref="InsertDigit"/>, <see cref="SetDigit"/>,
        ///     <see cref="RemoveDigit"/> to modify a token in release code.
        /// </para>
        /// </remarks>
        public static PasswordToken FromPassCode(byte[] passcode)
        {
            PasswordToken token = new PasswordToken(passcode.Length);
            foreach (byte value in passcode)
            {
                token.AppendDigit(value);
            }

            return token;
        }

        /// <summary>
        ///     Create a token from provided code.
        ///     Handle with care: you need to provide the correct digit count when there are leading zeros. 
        /// </summary>
        /// <param name="passcode">The passcode as integer.</param>
        /// <param name="digits">The digit count to apply if there are leading zeros.</param>
        /// <returns>The generated password token.</returns>
        /// <remarks>
        /// <para>
        ///     Handle with care and demonstration / test purposes: using this method implies the passcode
        ///     is held somewhere in memory by your code. This is most likely to be completely unprotected.
        /// </para>
        /// <para>
        ///     Use <see cref="AppendDigit"/>, <see cref="InsertDigit"/>, <see cref="SetDigit"/>,
        ///     <see cref="RemoveDigit"/> to modify a token in release code.
        /// </para>
        /// </remarks>
        public static PasswordToken FromPassCode(int passcode, int? digits = null)
        {
            int count = 0;
            int copy = passcode;
            while (copy > 0)
            {
                count++;
                copy /= 10;
            }

            if (digits.HasValue && digits.Value > count)
            {
                count = digits.Value;
            }

            PasswordToken token = new PasswordToken(count);
            for (int i = 0; i < count; i++)
            {
                byte value = (byte)(passcode % 10);
                token.InsertDigit(0, value);
                passcode /= 10;
            }

            return token;
        }

        private void Init()
        {
            _storage = Protector.Protect(new byte[this.MaxLength]);
            _hash = 0;
            this.Length = 0;
        }

        private void Zero(byte[] array)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++) array[i] = 0;
            }
        }

        private void Hash(byte[] array)
        {
            // hashing might create a vulnerability when someone brute forces
            // hashes for all valid combinations
            using (var sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(array, 0, this.Length);
                int offset = hash[hash.Length - 1] & 0xf;

                // Convert the 4 bytes into an integer, ignoring the sign.
                _hash = ((hash[offset] & 0x7f) << 24)
                        | (hash[offset + 1] << 16)
                        | (hash[offset + 2] << 8)
                        | hash[offset + 3];
            }
        }

        /// <summary>
        ///     Clears this token.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The token was already disposed.</exception>
        public void Clear()
        {
            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            this.Zero(_storage);
            this.Init();
        }

        /// <summary>
        ///     Append a single digit to the token.
        /// </summary>
        /// <param name="digit"></param>
        /// <exception cref="ObjectDisposedException">The token was already disposed.</exception>
        public void AppendDigit(byte digit)
        {
            if (this.Length >= this.MaxLength)
            {
                return;
            }

            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            byte[] temp = null;
            try
            {
                temp = Protector.Unprotect(_storage);
                temp[this.Length++] = (byte)(digit % 10);
                this.Hash(temp);
                _storage = Protector.Protect(temp);
            }
            finally
            {
                this.Zero(temp);
            }
        }

        public void InsertDigit(int index, byte value)
        {
            if (index < 0 || index >= this.MaxLength)
            {
                throw new IndexOutOfRangeException();
            }

            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (this.Length >= this.MaxLength)
            {
                return;
            }

            byte[] temp = null;
            try
            {
                temp = Protector.Unprotect(_storage);
                for (int i = this.MaxLength - 1; i > index; i--) temp[i] = temp[i - 1];
                temp[index] = value;

                if (this.Length < index + 1)
                {
                    this.Length = index + 1;
                }
                else
                {
                    this.Length++;
                }

                this.Hash(temp);
                _storage = Protector.Protect(temp);
            }
            finally
            {
                this.Zero(temp);
            }
        }

        /// <summary>
        ///     Remove the digit at the provided index. 
        /// </summary>
        /// <param name="index">Index of the digit to remove.</param>
        /// <exception cref="ObjectDisposedException">The token was already disposed.</exception>
        public void RemoveDigit(int index)
        {
            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (index < 0
                || index >= this.MaxLength
                || this.Length == 0)
            {
                return;
            }

            byte[] temp = null;
            try
            {
                temp = Protector.Unprotect(_storage);
                for (int i = index; i < this.MaxLength - 1; i++) temp[i] = temp[i + 1];
                temp[--this.Length] = 0;
                this.Hash(temp);
                _storage = Protector.Protect(temp);
            }
            finally
            {
                this.Zero(temp);
            }
        }

        internal byte GetDigit(int index)
        {
            if (index < 0 || index >= this.MaxLength)
            {
                throw new IndexOutOfRangeException();
            }

            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (index >= this.Length)
            {
                return 0;
            }

            byte[] temp = null;
            try
            {
                temp = Protector.Unprotect(_storage);
                return temp[index];
            }
            finally
            {
                this.Zero(temp);
            }
        }

        /// <summary>
        ///     Sets the digit at the provided index.
        /// </summary>
        /// <param name="index">Index of the digit.</param>
        /// <param name="value">Value to set the digit to.</param>
        /// <exception cref="IndexOutOfRangeException">Index is not valid.</exception>
        /// <exception cref="ObjectDisposedException">The token was already disposed.</exception>
        public void SetDigit(int index, byte value)
        {
            if (index < 0 || index >= this.MaxLength)
            {
                throw new IndexOutOfRangeException();
            }

            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            byte[] temp = null;
            try
            {
                temp = Protector.Unprotect(_storage);
                temp[index] = value;
                if (this.Length < index + 1) this.Length = index + 1;
                this.Hash(temp);
                _storage = Protector.Protect(temp);
            }
            finally
            {
                this.Zero(temp);
            }
        }

        /// <summary>
        ///     Validate this token against another one.
        /// </summary>
        /// <param name="other">The other token to validate with.</param>
        /// <returns>True, if both tokens are the same.</returns>
        /// <exception cref="ObjectDisposedException">One of the tokens was already disposed.</exception>
        public bool Validate(PasswordToken other)
        {
            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (other._storage == null)
            {
                throw new ObjectDisposedException(nameof(other));
            }

            return this.Length == other.Length
                   && _hash == other._hash;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Length = 0;
            this.Zero(_storage);
            _hash = 0;
            _storage = null;
        }

        /// <inheritdoc />
        public bool Equals(PasswordToken other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Validate(other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((PasswordToken)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => _hash;
    }
}