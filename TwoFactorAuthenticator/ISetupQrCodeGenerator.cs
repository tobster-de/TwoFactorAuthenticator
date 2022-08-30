namespace TwoFactorAuthenticator
{
    public interface ISetupQrCodeGenerator
    {
        /// <summary>
        /// Create a URL that contains image data of the setup QR code.
        /// </summary>
        /// <param name="provisionUrl">The base URL containing auth properties.</param>
        string GenerateQrCodeUrl(string provisionUrl);
        
        /// <summary>
        /// Returns the image data of the setup QR code.
        /// </summary>
        /// <param name="provisionUrl">The base URL containing auth properties.</param>
        byte[] GetQrCodeImageData(string provisionUrl);
    }
}