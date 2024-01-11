using Xunit;
using Shouldly;
using System.Text;
using TwoFactorAuthenticator.Security;

namespace TwoFactorAuthenticator.Tests
{
    public class GeneratePinTests
    {
        [Fact]
        public void OverloadsReturnSamePIN()
        {
            string secret = "JBSWY3DPEHPK3PXP";
            byte[] secretAsBytes = Encoding.UTF8.GetBytes(secret);
            string secretAsBase32 = Base32Encoding.ToString(secretAsBytes);
            long counter = 54615912;
            PasswordToken expected = PasswordToken.FromPassCode(508826);

            var subject = new Authenticator();

            PasswordToken pinFromString = subject.GeneratePINAtInterval(secret, counter);
            PasswordToken pinFromBytes = subject.GeneratePINAtInterval(secretAsBytes, counter);
            PasswordToken pinFromBase32 = subject.GeneratePINAtInterval(secretAsBase32, counter, secretIsBase32: true);

            pinFromString.Validate(expected).ShouldBe(true);
            pinFromBytes.Validate(expected).ShouldBe(true);
            pinFromBase32.Validate(expected).ShouldBe(true);
        }

        [Fact]
        public void OverloadsReturnSamePINSHA256()
        {
            var secret = "JBSWY3DPEHPK3PXP";
            var secretAsBytes = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsBytes);
            long counter = 54615912;
            PasswordToken expected = PasswordToken.FromPassCode(087141, 6);

            var subject = new Authenticator(HashType.SHA256);

            var pinFromString = subject.GeneratePINAtInterval(secret, counter);
            var pinFromBytes = subject.GeneratePINAtInterval(secretAsBytes, counter);
            var pinFromBase32 = subject.GeneratePINAtInterval(secretAsBase32, counter, secretIsBase32: true);

            pinFromString.Validate(expected).ShouldBe(true);
            pinFromBytes.Validate(expected).ShouldBe(true);
            pinFromBase32.Validate(expected).ShouldBe(true);
        }


        [Fact]
        public void OverloadsReturnSamePINSHA512()
        {
            var secret = "JBSWY3DPEHPK3PXP";
            var secretAsBytes = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsBytes);
            long counter = 54615912;
            PasswordToken expected = PasswordToken.FromPassCode(397230);

            var subject = new Authenticator(HashType.SHA512);

            var pinFromString = subject.GeneratePINAtInterval(secret, counter);
            var pinFromBytes = subject.GeneratePINAtInterval(secretAsBytes, counter);
            var pinFromBase32 = subject.GeneratePINAtInterval(secretAsBase32, counter, secretIsBase32: true);

            pinFromString.Validate(expected).ShouldBe(true);
            pinFromBytes.Validate(expected).ShouldBe(true);
            pinFromBase32.Validate(expected).ShouldBe(true);
        }
    }
}