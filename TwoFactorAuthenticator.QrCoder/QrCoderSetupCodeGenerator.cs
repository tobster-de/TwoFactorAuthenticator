using System;
using QRCoder;

namespace TwoFactorAuthenticator.QrCoder
{
    public class QrCoderSetupCodeGenerator : ISetupQrCodeGenerator
    {
        private int pixelsPerModule = 3;

        /// <summary>
        /// This value influences the size of the resulting QR code image.
        /// </summary>
        public int PixelsPerModule
        {
            get => this.pixelsPerModule;
            set
            {
                System.Diagnostics.Debug.Assert(
                    value <= 10,
                    string.Format(Resources.QrCoderSetupCodeGenerator_PixelsPerModule_Value, nameof(this.PixelsPerModule)));
                this.pixelsPerModule = value;
            }
        }

        public string GenerateQrCodeUrl(string provisionUrl)
            => $"data:image/png;base64,{Convert.ToBase64String(this.GetQrCodeImageData(provisionUrl))}";
        
        public byte[] GetQrCodeImageData(string provisionUrl)
        {
            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(provisionUrl, QRCodeGenerator.ECCLevel.Q))
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(this.PixelsPerModule);
                }
            }
            catch (System.Runtime.InteropServices.ExternalException e)
            {
                if (e.Message.Contains("GDI+") && this.PixelsPerModule > 10)
                {
                    throw new QrException(
                        string.Format(
                            Resources.QrCoderSetupCodeGenerator_GenerateQrCodeImageData_QrException_PixelsPerModule,
                            nameof(this.PixelsPerModule)),
                        e);
                }

                throw;
            }
        }
    }
}