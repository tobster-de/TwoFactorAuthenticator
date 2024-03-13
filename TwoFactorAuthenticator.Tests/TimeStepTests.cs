using Xunit;
using Shouldly;
using System;
using TwoFactorAuthenticator.Security;

namespace TwoFactorAuthenticator.Tests
{
    public class TimeStepTests
    {
        [Fact]
        public void DefaultPINHasNotBeenChangedByAddingTimeStepConfig()
        {
            var now = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            string secret = "12314241234342342";
            PasswordToken defaultPin = new Authenticator().GetCurrentPIN(secret, now);
            
            // This pin was created with the code from before the timestep config was added
            PasswordToken expected = PasswordToken.FromPassCode(668182);

            defaultPin.Validate(expected).ShouldBeTrue();
        }

        [Fact]
        public void DifferentTimeStepsReturnsDifferentPINs()
        {
            var now = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            string secret = "12314241234342342";
            PasswordToken defaultPin = new Authenticator().GetCurrentPIN(secret, now);
            PasswordToken pinWith15SecondTimeStep = new Authenticator(15).GetCurrentPIN(secret, now);
            PasswordToken pinWith60SecondTimeStep = new Authenticator(60).GetCurrentPIN(secret, now);

            defaultPin.Validate(pinWith15SecondTimeStep).ShouldBeFalse();
            defaultPin.Validate(pinWith60SecondTimeStep).ShouldBeFalse();
            pinWith15SecondTimeStep.Validate(pinWith60SecondTimeStep).ShouldBeFalse();
        }

        [Fact]
        public void DefaultTimeStepGivesSamePinAs30()
        {
            var now = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            string secret = "12314241234342342";
            PasswordToken defaultPin = new Authenticator().GetCurrentPIN(secret, now);
            PasswordToken pinWith30SecondTimeStep = new Authenticator(30).GetCurrentPIN(secret, now);

            defaultPin.Validate(pinWith30SecondTimeStep).ShouldBeTrue();
        }
    }
}