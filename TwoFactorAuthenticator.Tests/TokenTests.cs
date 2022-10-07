using System;
using Shouldly;
using TwoFactorAuthenticator.Security;
using Xunit;

namespace TwoFactorAuthenticator.Tests
{
    public class TokenTest
    {
        [Fact]
        public void AppendDigit_GetDigit()
        {
            PasswordToken token = new PasswordToken();
            token.AppendDigit(6);
            token.AppendDigit(3);

            token.GetDigit(0).ShouldBe((byte)6);
            token.GetDigit(1).ShouldBe((byte)3);
            token.GetDigit(2).ShouldBe((byte)0);
        }

        [Fact]
        public void AppendDigit_WithMaxLength_IsIgnored()
        {
            int length = 6;
            PasswordToken token = new PasswordToken(length);
            for (byte i = 0; i < length + 2; i++)
            {
                token.AppendDigit(i);
            }

            for (byte i = 0; i < length; i++)
            {
                token.GetDigit(i).ShouldBe(i);
            }
        }

        [Fact]
        public void Indexer()
        {
            PasswordToken token = new PasswordToken();
            token[0] = 8;
            token[1] = 4;
            token[2] = 2;

            token[0].ShouldBe((byte)8);
            token[1].ShouldBe((byte)4);
            token[2].ShouldBe((byte)2);
        }

        [Fact]
        public void Indexer_Throws()
        {
            int length = 6;
            PasswordToken token = new PasswordToken(length);

            Assert.Throws<IndexOutOfRangeException>(() => token[length + 1] = 8);
            Assert.Throws<IndexOutOfRangeException>(() => token[-1] = 8);
            Assert.Throws<IndexOutOfRangeException>(() =>
                                                    {
                                                        int _ = token[length + 1];
                                                    });
            Assert.Throws<IndexOutOfRangeException>(() =>
                                                    {
                                                        int _ = token[-1];
                                                    });
        }

        [Fact]
        public void RemoveDigit_RemoveFirstDigits()
        {
            PasswordToken token = new PasswordToken();
            for (byte i = 1; i <= 6; i++)
            {
                token.AppendDigit(i);
            }

            token.Length.ShouldBe(6);

            for (byte i = 0; i < 3; i++)
            {
                token.RemoveDigit(0);
            }

            token.Length.ShouldBe(3);
            token.GetDigit(0).ShouldBe((byte)4);
            token.GetDigit(1).ShouldBe((byte)5);
            token.GetDigit(2).ShouldBe((byte)6);
            token.GetDigit(3).ShouldBe((byte)0);
        }

        [Fact]
        public void RemoveDigit_RemoveLastDigits()
        {
            PasswordToken token = new PasswordToken();
            for (byte i = 1; i <= 6; i++)
            {
                token.AppendDigit(i);
            }

            token.Length.ShouldBe(6);

            for (byte i = 3; i < 6; i++)
            {
                token.RemoveDigit(i);
            }

            token.Length.ShouldBe(3);

            token.GetDigit(0).ShouldBe((byte)1);
            token.GetDigit(1).ShouldBe((byte)2);
            token.GetDigit(2).ShouldBe((byte)3);
            token.GetDigit(3).ShouldBe((byte)0);
        }

        [Fact]
        public void RemoveDigit_RemoveLastDigitsReverse()
        {
            PasswordToken token = new PasswordToken();
            for (byte i = 1; i <= 6; i++)
            {
                token.AppendDigit(i);
            }

            token.Length.ShouldBe(6);

            for (byte i = 5; i > 2; i--)
            {
                token.RemoveDigit(i);
            }

            token.Length.ShouldBe(3);

            token.GetDigit(0).ShouldBe((byte)1);
            token.GetDigit(1).ShouldBe((byte)2);
            token.GetDigit(2).ShouldBe((byte)3);
            token.GetDigit(3).ShouldBe((byte)0);
        }

        [Fact]
        public void RemoveDigit_RemoveMoreThanAdded_IsIgnored()
        {
            PasswordToken token = new PasswordToken();
            for (byte i = 1; i <= 3; i++)
            {
                token.AppendDigit(i);
            }

            for (byte i = 0; i < 6; i++)
            {
                token.RemoveDigit(0);
            }

            token.Length.ShouldBe(0);

            for (byte i = 0; i < 6; i++)
            {
                token.GetDigit(i).ShouldBe<byte>(0);
            }
        }

        [Fact]
        public void RemoveDigit_RemoveOutOfRange_IsIgnored()
        {
            int length = 6;
            PasswordToken token = new PasswordToken(length);
            for (byte i = 0; i < length; i++)
            {
                token.AppendDigit(i);
            }

            token.RemoveDigit(-1);
            token.RemoveDigit(length + 1);

            token.Length.ShouldBe(6);

            for (byte i = 0; i < length; i++)
            {
                token.GetDigit(i).ShouldBe(i);
            }
        }

        [Fact]
        public void InsertDigit_IntoExistingDigits()
        {
            PasswordToken token = new PasswordToken();
            for (byte i = 1; i <= 4; i++)
            {
                token.AppendDigit(i);
            }

            token.InsertDigit(2, 9);

            token.Length.ShouldBe(5);
            token.GetDigit(0).ShouldBe((byte)1);
            token.GetDigit(1).ShouldBe((byte)2);
            token.GetDigit(2).ShouldBe((byte)9);
            token.GetDigit(3).ShouldBe((byte)3);
            token.GetDigit(4).ShouldBe((byte)4);
            token.GetDigit(5).ShouldBe((byte)0);
        }

        [Fact]
        public void InsertDigit_WithoutExistingDigits()
        {
            PasswordToken token = new PasswordToken();

            token.InsertDigit(2, 9);

            token.Length.ShouldBe(3);
            token.GetDigit(0).ShouldBe((byte)0);
            token.GetDigit(1).ShouldBe((byte)0);
            token.GetDigit(2).ShouldBe((byte)9);
            token.GetDigit(3).ShouldBe((byte)0);
            token.GetDigit(4).ShouldBe((byte)0);
            token.GetDigit(5).ShouldBe((byte)0);
        }

        [Fact]
        public void InsertDigit_WithMaxLength_IsIgnored()
        {
            PasswordToken token = new PasswordToken();
            for (byte i = 0; i < 6; i++)
            {
                token.AppendDigit(i);
            }

            token.InsertDigit(2, 9);

            token.Length.ShouldBe(6);
            for (byte i = 0; i < 6; i++)
            {
                token.GetDigit(i).ShouldBe(i);
            }
        }

        [Fact]
        public void InsertDigit_Throws()
        {
            int length = 6;
            PasswordToken token = new PasswordToken(length);

            Assert.Throws<IndexOutOfRangeException>(() => token.InsertDigit(length + 1, 9));
            Assert.Throws<IndexOutOfRangeException>(() => token.InsertDigit(-1, 8));
        }

        [Fact]
        public void Length_Clear()
        {
            PasswordToken token = new PasswordToken();
            token.Length.ShouldBe(0);

            for (byte i = 1; i <= 3; i++)
            {
                token.AppendDigit(i);
            }

            token.Length.ShouldBe(3);

            token.Clear();

            token.Length.ShouldBe(0);
        }

        [Fact]
        public void Validate_IsValid_SameMaxLength()
        {
            int length = 6;
            PasswordToken token = new PasswordToken(length);
            PasswordToken other = new PasswordToken(length);
            for (byte i = 0; i < length; i++)
            {
                token.AppendDigit(i);
                other.AppendDigit(i);
            }

            token.Validate(other).ShouldBe(true);
        }

        [Fact]
        public void Validate_IsValid_DifferentMaxLength()
        {
            int length = 6;
            PasswordToken token = new PasswordToken(length);
            PasswordToken other = new PasswordToken(length + 2);
            for (byte i = 0; i < length; i++)
            {
                token.AppendDigit(i);
                other.AppendDigit(i);
            }

            token.Validate(other).ShouldBe(true);
        }

        [Fact]
        public void Validate_IsInvalid()
        {
            PasswordToken token = new PasswordToken();
            PasswordToken other = new PasswordToken();
            for (byte i = 0; i < 6; i++)
            {
                token.AppendDigit(i);
                other.AppendDigit((byte)(i + 1));
            }

            token.Validate(other).ShouldBe(false);
        }

        [Fact]
        public void Validate_IsInvalid_ByLength()
        {
            PasswordToken token = new PasswordToken();
            PasswordToken other = new PasswordToken();
            for (byte i = 0; i < 6; i++)
            {
                token.AppendDigit(i);
                if (i < 4)
                {
                    other.AppendDigit(i);
                }
            }

            token.Validate(other).ShouldBe(false);
        }

        [Fact]
        public void Disposal()
        {
            PasswordToken token;
            PasswordToken other;
            using (token = new PasswordToken())
            {
                using (other = new PasswordToken())
                {
                    token.Length.ShouldBe(0);
                    token.AppendDigit(1);

                    token.Length.ShouldBe(1);
                }

                Assert.Throws<ObjectDisposedException>(() => token.Validate(other));
            }

            token.Length.ShouldBe(0);
            token.GetHashCode().ShouldBe(0);

            Assert.Throws<ObjectDisposedException>(() => token.AppendDigit(2));
            Assert.Throws<ObjectDisposedException>(() => token.InsertDigit(0, 3));
            Assert.Throws<ObjectDisposedException>(() => token.RemoveDigit(0));
            Assert.Throws<ObjectDisposedException>(() => token.GetDigit(0));
            Assert.Throws<ObjectDisposedException>(() => token.SetDigit(0, 4));
            Assert.Throws<ObjectDisposedException>(() => token.Clear());
            Assert.Throws<ObjectDisposedException>(() => token.Validate(other));
        }

        [Fact]
        public void Test_Equals()
        {
            int length = 6;
            PasswordToken token = new PasswordToken(length);
            PasswordToken other = new PasswordToken(length);
            for (byte i = 0; i < length; i++)
            {
                token.AppendDigit(i);
                other.AppendDigit(i);
            }

            token.GetHashCode().ShouldNotBe(0);
            token.GetHashCode().ShouldBe(other.GetHashCode());

            token.Equals(token).ShouldBe(true);
            token.Equals((object)token).ShouldBe(true);
            token.Equals(other).ShouldBe(true);
            token.Equals((object)other).ShouldBe(true);

            object obj = this;
            token.Equals(obj).ShouldBe(false);
            token.Equals(null).ShouldBe(false);
            token.Equals((object)null).ShouldBe(false);
        }

        [Fact]
        public void Test_GetHashCode()
        {
            PasswordToken token = new PasswordToken();
            PasswordToken other = new PasswordToken();

            token.GetHashCode().ShouldBe(0);

            token.AppendDigit(1);
            other.AppendDigit(1);

            int code1 = token.GetHashCode();

            token.AppendDigit(2);

            int code2 = token.GetHashCode();

            code1.ShouldBe(other.GetHashCode());
            code1.ShouldNotBe(code2);
        }

        [Fact]
        public void Test_CreatedDifferently_ShouldBeSame()
        {
            byte[] digits = { 5, 0, 8, 8, 2, 6 };

            PasswordToken token1 = new PasswordToken();
            PasswordToken token2 = new PasswordToken();
            PasswordToken token3 = new PasswordToken();

            for (int i = 0; i < digits.Length; i++)
            {
                token1.AppendDigit(digits[i]);
                token2.InsertDigit(i, digits[i]);
                token3.SetDigit(i, digits[i]);
            }

            token1.Validate(token2).ShouldBe(true);
            token2.Validate(token3).ShouldBe(true);
            token3.Validate(token1).ShouldBe(true);
        }

        [Fact]
        public void Test_EncodePinToToken()
        {
            int password = 508826;
            byte[] digits = { 5, 0, 8, 8, 2, 6 };

            PasswordToken token = PasswordToken.FromPassCode(password);
            PasswordToken token2 = new PasswordToken();

            foreach (byte value in digits)
            {
                token2.AppendDigit(value);
            }

            token.Validate(token2).ShouldBe(true);
        }

        [Fact]
        public void Test_DecodeToUnsafeToken()
        {
            int password = 508826;

            PasswordToken token = PasswordToken.FromPassCode(password);
            using (UnsafeToken unsafeToken = UnsafeToken.FromPasswordToken(token))
            {
                unsafeToken.ToString().ShouldBe(password.ToString());
            }
        }
    }
}