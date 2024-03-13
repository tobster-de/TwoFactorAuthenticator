using Xunit;
using Shouldly;
using System.Collections.Generic;
using System.Text;
using System;
using TwoFactorAuthenticator.Security;

namespace TwoFactorAuthenticator.Tests
{
    public class SecretTypeTests
    {
        const string secret = "ggggjhG&^*&^jfSSSddd";
        private readonly static byte[] secretAsBytes = Encoding.UTF8.GetBytes(secret);
        private readonly static string secretAsBase32 = Base32Encoding.ToString(secretAsBytes);

        [Theory]
        [MemberData(nameof(GetPins), HashType.SHA1)]
        public void ValidateWorksWithDifferentSecretTypes(PasswordToken pin, int irrelevantNumberToAvoidDuplicatePinsBeingRemoved)
        {
            // We can't directly test that the different overloads for GetCurrentPIN creates the same result,
            // as the time difference may may cause different PINS to be created.
            // So instead we generate the PINs by each method and validate each one by each method.
            var subject = new Authenticator();

            subject.ValidateTwoFactorPIN(secret, pin, false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secret, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved))
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secret, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2)
                   .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetPins), HashType.SHA256)]
        public void ValidateWorksWithDifferentSecretTypesSHA256(PasswordToken pin, int irrelevantNumberToAvoidDuplicatePinsBeingRemoved)
        {
            // We can't directly test that the different overloads for GetCurrentPIN creates the same result,
            // as the time difference may may cause different PINS to be created.
            // So instead we generate the PINs by each method and validate each one by each method.
            var subject = new Authenticator(HashType.SHA256);

            subject.ValidateTwoFactorPIN(secret, pin, false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secret, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved))
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secret, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2)
                   .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetPins), HashType.SHA512)]
        public void ValidateWorksWithDifferentSecretTypesSHA512(PasswordToken pin, int irrelevantNumberToAvoidDuplicatePinsBeingRemoved)
        {
            // We can't directly test that the different overloads for GetCurrentPIN creates the same result,
            // as the time difference may may cause different PINS to be created.
            // So instead we generate the PINs by each method and validate each one by each method.
            var subject = new Authenticator(HashType.SHA512);

            subject.ValidateTwoFactorPIN(secret, pin, false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secret, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved))
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secret, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, false)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, true)
                   .ShouldBeTrue();
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2)
                   .ShouldBeTrue();
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