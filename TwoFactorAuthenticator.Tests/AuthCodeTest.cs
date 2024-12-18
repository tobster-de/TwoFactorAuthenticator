﻿using System.Text;
using Shouldly;
using TwoFactorAuthenticator.Security;
using Xunit;

namespace TwoFactorAuthenticator.Tests
{
    public class AuthCodeTest
    {
        [Fact]
        public void BasicAuthCodeTest()
        {
            string secretKey = "PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH";
            PasswordToken expected = PasswordToken.FromPassCode(551508);

            var tfa = new Authenticator();

            int currentTime = 1416643820;

            PasswordToken actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6);

            actual.Validate(expected).ShouldBe(true);
        }

        [Fact]
        public void BasicAuthCodeTestSHA256()
        {
            var secretKey = "PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH";
            PasswordToken expected = PasswordToken.FromPassCode(002939, 6);

            var tfa = new Authenticator(HashType.SHA256);

            var currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds?
            // Maybe need an overload that takes a DateTime?
            var actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6);

            actual.Validate(expected).ShouldBe(true);
        }

        [Fact]
        public void BasicAuthCodeTestSHA512()
        {
            var secretKey = "PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH";
            PasswordToken expected = PasswordToken.FromPassCode(461461);

            var tfa = new Authenticator(HashType.SHA512);

            var currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds?
            // Maybe need an overload that takes a DateTime?
            var actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6);

            actual.Validate(expected).ShouldBe(true);
        }

        [Fact]
        public void Base32AuthCodeTest()
        {
            string secretKey = Base32Encoding.ToString(Encoding.UTF8.GetBytes("PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH"));
            PasswordToken expected = PasswordToken.FromPassCode(551508);

            var tfa = new Authenticator();

            int currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds?
            // Maybe need an overload that takes a DateTime?
            PasswordToken actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6, true);

            actual.Validate(expected).ShouldBe(true);
        }

        [Fact]
        public void Base32AuthCodeTestSHA256()
        {
            var secretKey = Base32Encoding.ToString(Encoding.UTF8.GetBytes("PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH"));
            PasswordToken expected = PasswordToken.FromPassCode(002939, 6);

            var tfa = new Authenticator(HashType.SHA256);

            var currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds?
            // Maybe need an overload that takes a DateTime?
            var actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6, true);

            actual.Validate(expected).ShouldBe(true);
        }

        [Fact]
        public void Base32AuthCodeTestSHA512()
        {
            var secretKey = Base32Encoding.ToString(Encoding.UTF8.GetBytes("PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH"));
            PasswordToken expected = PasswordToken.FromPassCode(461461);

            var tfa = new Authenticator(HashType.SHA512);

            var currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds?
            // Maybe need an overload that takes a DateTime?
            var actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6, true);

            actual.Validate(expected).ShouldBe(true);
        }
    }
}