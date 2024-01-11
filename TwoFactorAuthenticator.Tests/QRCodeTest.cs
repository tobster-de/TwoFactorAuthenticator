using System.Collections.Generic;
using Xunit;
using Shouldly;
using TwoFactorAuthenticator.QrCoder;
using ZXing;

namespace TwoFactorAuthenticator.Tests
{
    public class QRCodeTest
    {
        [Theory]
        [InlineData("issuer", "otpauth://totp/issuer:a%40b.com?secret=ONSWG4TFOQ&issuer=issuer")]
        [InlineData("Foo & Bar", "otpauth://totp/Foo%20%26%20Bar:a%40b.com?secret=ONSWG4TFOQ&issuer=Foo%20%26%20Bar")]
        [InlineData("个", "otpauth://totp/%E4%B8%AA:a%40b.com?secret=ONSWG4TFOQ&issuer=%E4%B8%AA")]
        public void CanGenerateQRCode(string issuer, string expectedUrl)
        {
            var authenticator = new Authenticator();
            var setupCodeInfo = authenticator.GenerateSetupCode(issuer, "a@b.com", "secret");

            var subject = new QrCoderSetupCodeGenerator { PixelsPerModule = 2 };
            byte[] imageData = subject.GetQrCodeImageData(setupCodeInfo.ProvisionUrl);
            string actualUrl = ExtractUrlFromQRImage(imageData);

            setupCodeInfo.ProvisionUrl.ShouldBe(expectedUrl);
            actualUrl.ShouldBe(expectedUrl);
        }


        [Theory]
        [InlineData("issuer", "otpauth://totp/issuer:a%40b.com?secret=ONSWG4TFOQ&issuer=issuer&algorithm=SHA256")]
        [InlineData("Foo & Bar", "otpauth://totp/Foo%20%26%20Bar:a%40b.com?secret=ONSWG4TFOQ&issuer=Foo%20%26%20Bar&algorithm=SHA256")]
        [InlineData("个", "otpauth://totp/%E4%B8%AA:a%40b.com?secret=ONSWG4TFOQ&issuer=%E4%B8%AA&algorithm=SHA256")]
        public void CanGenerateSHA256QRCode(string issuer, string expectedUrl)
        {
            var authenticator = new Authenticator(HashType.SHA256);
            var setupCodeInfo = authenticator.GenerateSetupCode(
                issuer,
                "a@b.com",
                "secret");
            
            var subject = new QrCoderSetupCodeGenerator { PixelsPerModule = 2 };
            byte[] imageData = subject.GetQrCodeImageData(setupCodeInfo.ProvisionUrl);
            string actualUrl = ExtractUrlFromQRImage(imageData);

            actualUrl.ShouldBe(expectedUrl);
        }

        [Theory]
        [InlineData("issuer", "otpauth://totp/issuer:a%40b.com?secret=ONSWG4TFOQ&issuer=issuer&algorithm=SHA512")]
        [InlineData("Foo & Bar", "otpauth://totp/Foo%20%26%20Bar:a%40b.com?secret=ONSWG4TFOQ&issuer=Foo%20%26%20Bar&algorithm=SHA512")]
        [InlineData("个", "otpauth://totp/%E4%B8%AA:a%40b.com?secret=ONSWG4TFOQ&issuer=%E4%B8%AA&algorithm=SHA512")]
        public void CanGenerateSHA512QRCode(string issuer, string expectedUrl)
        {
            var authenticator = new Authenticator(HashType.SHA512);
            var setupCodeInfo = authenticator.GenerateSetupCode(
                issuer,
                "a@b.com",
                "secret");

            var subject = new QrCoderSetupCodeGenerator { PixelsPerModule = 2 };
            byte[] imageData = subject.GetQrCodeImageData(setupCodeInfo.ProvisionUrl);
            string actualUrl = ExtractUrlFromQRImage(imageData);

            actualUrl.ShouldBe(expectedUrl);
        }

        private static string ExtractUrlFromQRImage(byte[] imageData)
        {
            // var headerLength = "data:image/png;base64,".Length;
            // var rawImageData = qrCodeSetupImageUrl.Substring(headerLength, qrCodeSetupImageUrl.Length - headerLength);
            // var imageData = Convert.FromBase64String(rawImageData);
        
            var reader = new BarcodeReaderGeneric();
            reader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };
            var image = new ImageMagick.MagickImage(imageData);
            var wrappedImage = new ZXing.Magick.MagickImageLuminanceSource(image);
            return reader.Decode(wrappedImage).Text;
        }
    }
}