using System;
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