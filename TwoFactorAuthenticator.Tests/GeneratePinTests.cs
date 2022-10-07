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
    }
}