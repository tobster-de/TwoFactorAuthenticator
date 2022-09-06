using Xunit;
using Shouldly;
using System.Text;

namespace TwoFactorAuthenticator.Tests
{
    public class GeneratePinTests
    {
        [Fact]
        public void OverloadsReturnSamePIN()
        {
            var secret = "JBSWY3DPEHPK3PXP";
            var secretAsBytes = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsBytes);
            long counter = 54615912;
            var expected = "508826";

            var subject = new TwoFactorAuthenticator();

            var pinFromString = subject.GeneratePINAtInterval(secret, counter);
            var pinFromBytes = subject.GeneratePINAtInterval(secretAsBytes, counter);
            var pinFromBase32 = subject.GeneratePINAtInterval(secretAsBase32, counter, secretIsBase32: true);

            pinFromString.ShouldBe(expected);
            pinFromBytes.ShouldBe(expected);
            pinFromBase32.ShouldBe(expected);
        }
    }
}