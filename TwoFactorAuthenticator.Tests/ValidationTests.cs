using Xunit;
using Shouldly;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using TwoFactorAuthenticator.Security;

namespace TwoFactorAuthenticator.Tests
{
    public class ValidationTests
    {
        const string secret = "ggggjhG&^*&^jfSSSddd";
        private readonly static byte[] secretAsBytes = Encoding.UTF8.GetBytes(secret);
        private readonly static string secretAsBase32 = Base32Encoding.ToString(secretAsBytes);

        [Theory]
        [MemberData(nameof(GetPins))]
        public void ValidateWorksWithDifferentSecretTypes(PasswordToken pin, int irrelevantNumberToAvoidDuplicatePinsBeingRemoved)
        {
            // We can't directly test that the different overloads for GetCurrentPIN creates the same result,
            // as the time difference may may cause different PINS to be created.
            // So instead we generate the PINs by each method and validate each one by each method.
            var subject = new Authenticator();

            subject.ValidateTwoFactorPIN(secret, pin, false);
            subject.ValidateTwoFactorPIN(secret, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), false);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved));
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, true);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), true);
            subject.ValidateTwoFactorPIN(secret, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, false);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, true);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2);
        }

        [Theory]
        [MemberData(nameof(GetPins), HashType.SHA256)]
        public void ValidateWorksWithDifferentSecretTypesSHA256(PasswordToken pin, int irrelevantNumberToAvoidDuplicatePinsBeingRemoved)
        {
            // We can't directly test that the different overloads for GetCurrentPIN creates the same result,
            // as the time difference may may cause different PINS to be created.
            // So instead we generate the PINs by each method and validate each one by each method.
            var subject = new Authenticator(HashType.SHA256);

            subject.ValidateTwoFactorPIN(secret, pin, false);
            subject.ValidateTwoFactorPIN(secret, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), false);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved));
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, true);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), true);
            subject.ValidateTwoFactorPIN(secret, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, false);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, true);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2);
        }

        [Theory]
        [MemberData(nameof(GetPins), HashType.SHA512)]
        public void ValidateWorksWithDifferentSecretTypesSHA512(PasswordToken pin, int irrelevantNumberToAvoidDuplicatePinsBeingRemoved)
        {
            // We can't directly test that the different overloads for GetCurrentPIN creates the same result,
            // as the time difference may may cause different PINS to be created.
            // So instead we generate the PINs by each method and validate each one by each method.
            var subject = new Authenticator(HashType.SHA512);

            subject.ValidateTwoFactorPIN(secret, pin, false);
            subject.ValidateTwoFactorPIN(secret, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), false);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved));
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, true);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), true);
            subject.ValidateTwoFactorPIN(secret, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, false);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, true);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2);
        }

        [Fact]
        public void GetCurrentPinsHandles15SecondInterval()
        {
            // This is nonsensical, really, as anything less than 30 == 0 in practice.
            var subject = new Authenticator();

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(15)).Count().ShouldBe(1);
        }

        [Fact]
        public void GetCurrentPinsHandles15SecondIntervalSHA256()
        {
            // This is nonsensical, really, as anything less than 30 == 0 in practice.
            var subject = new Authenticator(HashType.SHA256);

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(15)).Count().ShouldBe(1);
        }

        [Fact]
        public void GetCurrentPinsHandles15SecondIntervalSHA512()
        {
            // This is nonsensical, really, as anything less than 30 == 0 in practice.
            var subject = new Authenticator(HashType.SHA512);

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(15)).Count().ShouldBe(1);
        }

        [Fact]
        public void GetCurrentPinsHandles30SecondInterval()
        {
            var subject = new Authenticator();

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(30)).Count().ShouldBe(3);
        }

        [Fact]
        public void GetCurrentPinsHandles30SecondIntervalSHA256()
        {
            var subject = new Authenticator(HashType.SHA256);

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(30)).Count().ShouldBe(3);
        }

        [Fact]
        public void GetCurrentPinsHandles30SecondIntervalSHA512()
        {
            var subject = new Authenticator(HashType.SHA512);

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(30)).Count().ShouldBe(3);
        }

        [Fact]
        public void GetCurrentPinsHandles60SecondInterval()
        {
            var subject = new Authenticator();

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(60)).Count().ShouldBe(5);
        }

        [Fact]
        public void GetCurrentPinsHandles60SecondIntervalSHA256()
        {
            var subject = new Authenticator(HashType.SHA256);

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(60)).Count().ShouldBe(5);
        }

        [Fact]
        public void GetCurrentPinsHandles60SecondIntervalSHA512()
        {
            var subject = new Authenticator(HashType.SHA512);

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(60)).Count().ShouldBe(5);
        }

        public static IEnumerable<object[]> GetPins()
        {
            var subject = new Authenticator();

            yield return new object[] { subject.GetCurrentPIN(secret), 2 };
            yield return new object[] { subject.GetCurrentPIN(secret, DateTime.UtcNow), 3 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBytes), 4 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBytes, DateTime.UtcNow), 5 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBase32, true), 6 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBase32, DateTime.UtcNow, true), 7 };
        }

        public static IEnumerable<object[]> GetPins(HashType hashType)
        {
            var subject = new Authenticator(hashType);

            yield return new object[] { subject.GetCurrentPIN(secret), 2 };
            yield return new object[] { subject.GetCurrentPIN(secret, DateTime.UtcNow), 3 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBytes), 4 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBytes, DateTime.UtcNow), 5 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBase32, true), 6 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBase32, DateTime.UtcNow, true), 7 };
        }
    }
}