using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TwoFactorAuthenticator;
using TwoFactorAuthenticator.QrCoder;
using TwoFactorAuthenticator.Security;

namespace WpfExample
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _issuer;

        [ObservableProperty]
        private string _account;

        [ObservableProperty]
        private string _secret;

        [ObservableProperty]
        private string _setupCode;

        [ObservableProperty]
        private BitmapSource _qrImageSource;

        [RelayCommand]
        private void GenerateCode()
        {
            var tfA = new TwoFactorAuthenticator.Authenticator();
            var qrscg = new QrCoderSetupCodeGenerator();

            try
            {
                SetupCode setupCode = tfA.GenerateSetupCode(this.Issuer, this.Account, this.Secret);

                this.QrImageSource = ImageConverter.CreateImage(setupCode.GetQrCodeImageData(qrscg));

                this.SetupCode = "Account: " + setupCode.Account + System.Environment.NewLine +
                                 "Secret Key: " + this.Secret + System.Environment.NewLine +
                                 "Encoded Key: " + setupCode.ManualEntryKey;
            }
            catch (Exception)
            {
            }
        }

        [ObservableProperty]
        private PasswordToken _token;

        [RelayCommand]
        private void Test()
        {
            var tfA = new TwoFactorAuthenticator.Authenticator();
            bool result = tfA.ValidateTwoFactorPIN(this.Secret, this.Token, secretIsBase32: false);

            MessageBox.Show(result ? "Validated!" : "Incorrect", "Result");
        }

        [ObservableProperty]
        private string _currentCodes;

        [RelayCommand]
        private void GetCodes()
        {
            this.CurrentCodes = string.Join(System.Environment.NewLine,
                ToReadableText(new TwoFactorAuthenticator.Authenticator().GetCurrentPINs(this.Secret)));
        }

        private static IEnumerable<string> ToReadableText(IEnumerable<PasswordToken> currentPins)
        {
            foreach (PasswordToken token in currentPins)
            {
                using (var unsafeToken = UnsafeToken.FromPasswordToken(token))
                {
                    yield return unsafeToken.ToString();
                }
            }
        }
    }
}