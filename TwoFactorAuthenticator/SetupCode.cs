namespace TwoFactorAuthenticator
{
    public class SetupCode
    {
        /// <summary>
        /// Identifiable account name
        /// </summary>
        public string Account { get; internal set; }
        
        /// <summary>
        /// Generated setup key for manual entry into mobile application.
        /// </summary>
        public string ManualEntryKey { get; internal set; }

        /// <summary>
        /// Setup URL in standardized format
        /// </summary>
        public string ProvisionUrl { get; internal set; }
        
        /// <summary>
        /// Create a URL that contains image data of the setup QR code.
        /// </summary>
        /// <param name="generator">The code generator instance that creates the QR code URL.</param>
        public string GenerateQrCodeUrl(ISetupQrCodeGenerator generator)
            => generator.GenerateQrCodeUrl(this.ProvisionUrl);

        /// <summary>
        /// Returns the image data of the setup QR code.
        /// </summary>
        /// <param name="generator">The code generator instance that creates the QR code image data.</param>
        public byte[] GetQrCodeImageData(ISetupQrCodeGenerator generator)
            => generator.GetQrCodeImageData(this.ProvisionUrl);
    }
}