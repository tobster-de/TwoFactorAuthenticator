﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TwoFactorAuthenticator.Security;

// ReSharper disable InconsistentNaming

namespace TwoFactorAuthenticator
{
    /// <summary>
    /// modified from
    /// https://github.com/brandonpotter/TwoFactorAuthenticator
    /// </summary>
    public class Authenticator
    {
        private static readonly DateTime Epoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public TimeSpan ClockDriftTolerance { get; }

        public HashType HashType { get; }

        public int TimeStepDuration { get; }

        /// <summary>
        /// Create a new default instance with disabled clock drift tolerance requiring the user 
        /// to have the exact code for the time of validation.
        ///
        /// Defaults to hash algorithm SHA1 and time step duration of 30 seconds.
        /// </summary>        
        public Authenticator() : this(HashType.SHA1)
        {
        }

        /// <summary>
        /// Create a new instance with disabled clock drift tolerance and the provided time step duration.
        /// The user will be forced to have the exact code for the time of validation.
        ///
        /// Defaults to hash algorithm SHA1.
        /// </summary>
        /// <param name="timeStepDuration">The TOTP time step duration to use.</param>
        public Authenticator(int timeStepDuration) : this(HashType.SHA1, timeStepDuration)
        {
        }

        /// <summary>
        /// Create a new instance with disabled clock drift tolerance and the provided time step duration.
        /// The user will be forced to have the exact code for the time of validation.
        /// The provided hash algorithm is used. 
        /// </summary>
        /// <param name="hashType">Hash algorithm to use.</param>
        /// <param name="timeStepDuration">The TOTP time step duration to use.</param>
        public Authenticator(HashType hashType, int timeStepDuration = 30) : this(TimeSpan.FromSeconds(30), hashType, timeStepDuration)
        {
        }

        /// <summary>
        /// Create a new instance with clock drift tolerance set to provided timespan. The user will be allowed
        /// to have any code that matches valid codes within the time range before and after current time. 
        /// </summary>
        /// <param name="clockDriftTolerance">The clock is allowed to be off this time span.</param>
        /// <param name="hashType">Hash algorithm to use.</param>
        /// <param name="timeStepDuration">The TOTP time step duration to use.</param>
        public Authenticator(TimeSpan clockDriftTolerance, HashType hashType = HashType.SHA1, int timeStepDuration = 30)
        {
            this.HashType = hashType;
            this.ClockDriftTolerance = clockDriftTolerance;
            this.TimeStepDuration = timeStepDuration;
        }

        /// <summary>
        /// Generate a setup code for a Authenticator user to scan
        /// </summary>
        /// <param name="issuer">Issuer ID (the name of the system, i.e. 'MyApp'),
        /// can be omitted but not recommended https://github.com/google/google-authenticator/wiki/Key-Uri-Format
        /// </param>
        /// <param name="accountTitleNoSpaces">Account Title (no spaces)</param>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns>SetupCode object</returns>
        public SetupCode GenerateSetupCode(
            string issuer,
            string accountTitleNoSpaces,
            string accountSecretKey,
            bool secretIsBase32 = false)
            => this.GenerateSetupCode(issuer, accountTitleNoSpaces, ConvertSecretToBytes(accountSecretKey, secretIsBase32));

        /// <summary>
        /// Generate a setup code for a Authenticator user to scan
        /// </summary>
        /// <param name="issuer">Issuer ID (the name of the system, i.e. 'MyApp'), can be omitted but not
        /// recommended https://github.com/google/google-authenticator/wiki/Key-Uri-Format </param>
        /// <param name="accountTitleNoSpaces">Account Title (no spaces)</param>
        /// <param name="accountSecretKey">Account Secret Key as byte[]</param>
        /// <returns>SetupCode object</returns>
        public SetupCode GenerateSetupCode(
            string issuer,
            string accountTitleNoSpaces,
            byte[] accountSecretKey)
        {
            if (string.IsNullOrWhiteSpace(accountTitleNoSpaces))
            {
                throw new ArgumentNullException(
                    nameof(accountTitleNoSpaces),
                    Resources.TwoFactorAuthenticator_GenerateSetupCode_ArgumentException_AccountTitle);
            }

            accountTitleNoSpaces = RemoveWhitespace(Uri.EscapeDataString(accountTitleNoSpaces));

            string secret = Base32Encoding.ToString(accountSecretKey).Trim('=');
            string encodedIssuer = UrlEncode(issuer);
            string algorithm = this.HashType == HashType.SHA1 ? "" : $"&algorithm={this.HashType}";
            string provisionUrl = string.IsNullOrWhiteSpace(issuer)
                                      ? $"otpauth://totp/{accountTitleNoSpaces}?secret={secret}{algorithm}"
                                      //  https://github.com/google/google-authenticator/wiki/Conflicting-Accounts
                                      // Added additional prefix to account otpauth://totp/Company:joe_example@gmail.com
                                      // for backwards compatibility
                                      : $"otpauth://totp/{encodedIssuer}:{accountTitleNoSpaces}?secret={secret}&issuer={encodedIssuer}{algorithm}";

            return new SetupCode
            {
                Account = accountTitleNoSpaces,
                ManualEntryKey = secret,
                ProvisionUrl = provisionUrl
            };
        }

        private static string RemoveWhitespace(string str)
            => new string(str.Where(c => !char.IsWhiteSpace(c)).ToArray());

        private static string UrlEncode(string value) => Uri.EscapeDataString(value);

        /// <summary>
        /// This method is generally called via <see cref="GetCurrentPIN(string,bool)" />/>
        /// </summary>
        /// <param name="accountSecretKey">The acount secret key as a string</param>
        /// <param name="counter">The number of 30-second (by default) intervals since the unix epoch</param>
        /// <param name="digits">The desired length of the returned PIN</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns>A 'PIN' that is valid for the specified time interval</returns>
        public PasswordToken GeneratePINAtInterval(
            string accountSecretKey,
            long counter,
            int digits = 6,
            bool secretIsBase32 = false)
            => this.GeneratePINAtInterval(ConvertSecretToBytes(accountSecretKey, secretIsBase32), counter, digits);

        /// <summary>
        /// This method is generally called via <see cref="GetCurrentPIN(string,bool)" />/>
        /// </summary>
        /// <param name="accountSecretKey">The acount secret key as a byte array</param>
        /// <param name="counter">The number of 30-second (by default) intervals since the unix epoch</param>
        /// <param name="digits">The desired length of the returned PIN</param>
        /// <returns>A 'PIN' that is valid for the specified time interval</returns>
        public PasswordToken GeneratePINAtInterval(byte[] accountSecretKey, long counter, int digits = 6)
            => this.GenerateHashedCode(accountSecretKey, counter, digits);

        private PasswordToken GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
        {
            byte[] counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(counter);

            HMAC hmac;
            if (HashType == HashType.SHA256)
                hmac = new HMACSHA256(key);
            else if (HashType == HashType.SHA512)
                hmac = new HMACSHA512(key);
            else
                hmac = new HMACSHA1(key);

            byte[] hash = hmac.ComputeHash(counter);
            int offset = hash[hash.Length - 1] & 0xf;

            // Convert the 4 bytes into an integer, ignoring the sign.
            int binary =
                ((hash[offset] & 0x7f) << 24)
                | (hash[offset + 1] << 16)
                | (hash[offset + 2] << 8)
                | hash[offset + 3];

            int password = binary % (int)Math.Pow(10, digits);

            PasswordToken token = new PasswordToken(digits);
            for (int i = 0; i < digits; i++)
            {
                byte value = (byte)(password % 10);
                token.InsertDigit(0, value);
                password /= 10;
            }

            return token;
        }

        private long GetCurrentCounter() => this.GetCurrentCounter(DateTime.UtcNow, Epoch);

        private long GetCurrentCounter(DateTime now, DateTime epoch) =>
            (long)(now - epoch).TotalSeconds / this.TimeStepDuration;

        /// <summary>
        /// Given a PIN from a client, check if it is valid at the current time.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="twoFactorCodeFromClient">The PIN from the client</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns>True if PIN is currently valid</returns>
        public bool ValidateTwoFactorPIN(
            string accountSecretKey,
            PasswordToken twoFactorCodeFromClient,
            bool secretIsBase32 = false)
            => this.ValidateTwoFactorPIN(
                accountSecretKey,
                twoFactorCodeFromClient,
                this.ClockDriftTolerance,
                secretIsBase32);

        /// <summary>
        /// Given a PIN from a client, check if it is valid at the current time.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="twoFactorCodeFromClient">The PIN from the client</param>
        /// <param name="timeTolerance">The time window within which to check to allow for clock drift between devices.</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns>True if PIN is currently valid</returns>
        public bool ValidateTwoFactorPIN(
            string accountSecretKey,
            PasswordToken twoFactorCodeFromClient,
            TimeSpan timeTolerance,
            bool secretIsBase32 = false)
            => this.ValidateTwoFactorPIN(ConvertSecretToBytes(accountSecretKey, secretIsBase32),
                twoFactorCodeFromClient,
                timeTolerance);

        /// <summary>
        /// Given a PIN from a client, check if it is valid at the current time.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="twoFactorCodeFromClient">The PIN from the client</param>
        /// <returns>True if PIN is currently valid</returns>
        public bool ValidateTwoFactorPIN(byte[] accountSecretKey, PasswordToken twoFactorCodeFromClient)
            => this.ValidateTwoFactorPIN(accountSecretKey, twoFactorCodeFromClient, this.ClockDriftTolerance);

        /// <summary>
        /// Given a PIN from a client, check if it is valid at the current time.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="twoFactorCodeFromClient">The PIN from the client</param>
        /// <param name="timeTolerance">The time window within which to check to allow for clock drift between devices.</param>
        /// <returns>True if PIN is currently valid</returns>
        public bool ValidateTwoFactorPIN(
            byte[] accountSecretKey,
            PasswordToken twoFactorCodeFromClient,
            TimeSpan timeTolerance)
            => this.GetCurrentPINs(accountSecretKey, timeTolerance)
                   .Any(token => token.Validate(twoFactorCodeFromClient));

        /// <summary>
        /// Given a PIN from a client, check if it is valid at the current time.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="twoFactorCodeFromClient">The PIN from the client</param>
        /// <param name="iterationOffset">The counter window within which to check to allow for clock drift between devices.</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns>True if PIN is currently valid</returns>
        public bool ValidateTwoFactorPIN(string accountSecretKey, PasswordToken twoFactorCodeFromClient, int iterationOffset,
            bool secretIsBase32 = false)
            => this.ValidateTwoFactorPIN(ConvertSecretToBytes(accountSecretKey, secretIsBase32), twoFactorCodeFromClient, iterationOffset);

        /// <summary>
        /// Given a PIN from a client, check if it is valid at the current time.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="twoFactorCodeFromClient">The PIN from the client</param>
        /// <param name="iterationOffset">The counter window within which to check to allow for clock drift between devices.</param>
        /// <returns>True if PIN is currently valid</returns>
        public bool ValidateTwoFactorPIN(byte[] accountSecretKey, PasswordToken twoFactorCodeFromClient, int iterationOffset)
            => this.GetCurrentPINs(accountSecretKey, iterationOffset).Any(token => token.Validate(twoFactorCodeFromClient));

        /// <summary>
        /// Get the PIN for current time; the same code that a 2FA app would generate for the current time.
        /// Do not validate directly against this as clockdrift may cause a a different PIN to be generated than one you did a second ago.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns>A 6-digit PIN</returns>
        public PasswordToken GetCurrentPIN(string accountSecretKey, bool secretIsBase32 = false)
            => this.GeneratePINAtInterval(accountSecretKey, this.GetCurrentCounter(), secretIsBase32: secretIsBase32);

        /// <summary>
        /// Get the PIN for current time; the same code that a 2FA app would generate for the current time.
        /// Do not validate directly against this as clockdrift may cause a a different PIN to be generated than one you did a second ago.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="now">The time you wish to generate the pin for</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns>A 6-digit PIN</returns>
        public PasswordToken GetCurrentPIN(string accountSecretKey, DateTime now, bool secretIsBase32 = false)
            => this.GeneratePINAtInterval(accountSecretKey, this.GetCurrentCounter(now, Epoch),
                secretIsBase32: secretIsBase32);

        /// <summary>
        /// Get the PIN for current time; the same code that a 2FA app would generate for the current time.
        /// Do not validate directly against this as clockdrift may cause a a different PIN to be generated.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <returns>A 6-digit PIN</returns>
        public PasswordToken GetCurrentPIN(byte[] accountSecretKey)
            => this.GeneratePINAtInterval(accountSecretKey, this.GetCurrentCounter());

        /// <summary>
        /// Get the PIN for current time; the same code that a 2FA app would generate for the current time.
        /// Do not validate directly against this as clockdrift may cause a a different PIN to be generated.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="now">The time you wish to generate the pin for</param>
        /// <returns>A 6-digit PIN</returns>
        public PasswordToken GetCurrentPIN(byte[] accountSecretKey, DateTime now)
            => this.GeneratePINAtInterval(accountSecretKey, this.GetCurrentCounter(now, Epoch));

        /// <summary>
        /// Get all the PINs that would be valid within the time window allowed for by the default clock drift.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns></returns>
        public IEnumerable<PasswordToken> GetCurrentPINs(string accountSecretKey, bool secretIsBase32 = false)
            => this.GetCurrentPINs(accountSecretKey, this.ClockDriftTolerance, secretIsBase32);

        /// <summary>
        /// Get all the PINs that would be valid within the time window allowed for by the specified clock drift.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="timeTolerance">The clock drift size you want to generate PINs for</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <returns></returns>
        public IEnumerable<PasswordToken> GetCurrentPINs(string accountSecretKey, TimeSpan timeTolerance, bool secretIsBase32 = false)
            => this.GetCurrentPINs(ConvertSecretToBytes(accountSecretKey, secretIsBase32), timeTolerance);

        /// <summary>
        /// Get all the PINs that would be valid within the time window allowed for by the default clock drift.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <returns></returns>
        public IEnumerable<PasswordToken> GetCurrentPINs(byte[] accountSecretKey)
            => this.GetCurrentPINs(accountSecretKey, this.ClockDriftTolerance);

        /// <summary>
        /// Get all the PINs that would be valid within the time window allowed for by the specified clock drift.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="timeTolerance">The clock drift size you want to generate PINs for</param>
        /// <returns></returns>
        public IEnumerable<PasswordToken> GetCurrentPINs(byte[] accountSecretKey, TimeSpan timeTolerance)
        {
            int iterationOffset = 0;

            if (timeTolerance.TotalSeconds >= this.TimeStepDuration)
            {
                iterationOffset = Convert.ToInt32(timeTolerance.TotalSeconds / this.TimeStepDuration);
            }

            return GetCurrentPINs(accountSecretKey, iterationOffset);
        }

        /// <summary>
        /// Get all the PINs that would be valid within the time window allowed for by the specified clock drift.
        /// </summary>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="iterationOffset">The counter drift size you want to generate PINs for</param>
        /// <returns></returns>
        public IEnumerable<PasswordToken> GetCurrentPINs(byte[] accountSecretKey, int iterationOffset)
        {
            long iterationCounter = this.GetCurrentCounter();

            long iterationStart = iterationCounter - iterationOffset;
            long iterationEnd = iterationCounter + iterationOffset;

            for (long counter = iterationStart; counter <= iterationEnd; counter++)
            {
                yield return this.GeneratePINAtInterval(accountSecretKey, counter);
            }
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32)
            => secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);
    }
}