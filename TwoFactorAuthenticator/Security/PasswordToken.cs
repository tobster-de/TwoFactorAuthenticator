using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("TwoFactorAuthenticator.Tests")]

namespace TwoFactorAuthenticator.Security
{
    public class PasswordToken : IDisposable, IEquatable<PasswordToken>
    {
        private byte[] _storage;
        private int _hash;

        public byte this[int index]
        {
            internal get => this.GetDigit(index);
            set => this.SetDigit(index, value);
        }

        public int MaxLength { get; }

        public int Length { get; private set; }

        public PasswordToken(int maxLength = 6)
        {
            this.MaxLength = maxLength;
            this.Init();
        }

        public static PasswordToken FromPassCode(int passcode)
        {
            int count = 0;
            int copy = passcode;
            while (copy > 0)
            {
                count++;
                copy /= 10;
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

        public void Clear()
        {
            if (_storage == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            this.Zero(_storage);
            this.Init();
        }

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