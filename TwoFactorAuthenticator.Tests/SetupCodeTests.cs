using Xunit;
using Shouldly;
using System.Text;

namespace TwoFactorAuthenticator.Tests
{
    public class SetupCodeTests
    {
        [Fact]
        public void ByteAndStringGeneratesSameSetupCode()
        {
            var secret = "12345678901234567890123456789012";
            var secretAsByteArray = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsByteArray);
            var issuer = "Test";
            var accountName = "TestAccount";
            var expected = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZA";

            var subject = new Authenticator();

            var setupCodeFromString = subject.GenerateSetupCode(issuer, accountName, secret);
            var setupCodeFromByteArray = subject.GenerateSetupCode(issuer, accountName, secretAsByteArray);
            var setupCodeFromBase32 = subject.GenerateSetupCode(issuer, accountName, secretAsBase32, true);

            setupCodeFromString.ManualEntryKey.ShouldBe(expected);
            setupCodeFromByteArray.ManualEntryKey.ShouldBe(expected);
            setupCodeFromBase32.ManualEntryKey.ShouldBe(expected);
        }

        [Fact]
        public void ByteAndStringGeneratesSameSetupCodeSHA256()
        {
            var secret = "12345678901234567890123456789012";
            var secretAsByteArray = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsByteArray);
            var issuer = "Test";
            var accountName = "TestAccount";
            var expected = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZA";

            var subject = new Authenticator(HashType.SHA256);

            var setupCodeFromString = subject.GenerateSetupCode(issuer, accountName, secret);
            var setupCodeFromByteArray = subject.GenerateSetupCode(issuer, accountName, secretAsByteArray);
            var setupCodeFromBase32 = subject.GenerateSetupCode(issuer, accountName, secretAsBase32, true);

            setupCodeFromString.ManualEntryKey.ShouldBe(expected);
            setupCodeFromByteArray.ManualEntryKey.ShouldBe(expected);
            setupCodeFromBase32.ManualEntryKey.ShouldBe(expected);
        }

        [Fact]
        public void ByteAndStringGeneratesSameSetupCodeSHA512()
        {
            var secret = "12345678901234567890123456789012";
            var secretAsByteArray = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsByteArray);
            var issuer = "Test";
            var accountName = "TestAccount";
            var expected = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZA";

            var subject = new Authenticator(HashType.SHA512);

            var setupCodeFromString = subject.GenerateSetupCode(issuer, accountName, secret);
            var setupCodeFromByteArray = subject.GenerateSetupCode(issuer, accountName, secretAsByteArray);
            var setupCodeFromBase32 = subject.GenerateSetupCode(issuer, accountName, secretAsBase32, true);

            setupCodeFromString.ManualEntryKey.ShouldBe(expected);
            setupCodeFromByteArray.ManualEntryKey.ShouldBe(expected);
            setupCodeFromBase32.ManualEntryKey.ShouldBe(expected);
        }
    }
}