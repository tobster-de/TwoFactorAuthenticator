using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TwoFactorAuthenticator;
using TwoFactorAuthenticator.QrCoder;
using TwoFactorAuthenticator.Security;

namespace WpfExample
{
    public class MainViewModel : ObservableObject
    {
        private string _issuer;

        public string Issuer
        {
            get => _issuer;
            set => this.SetProperty(ref _issuer, value);
        }

        private string _account;

        public string Account
        {
            get => _account ?? string.Empty;
            set => this.SetProperty(ref _account, value);
        }

        private string _secret;

        public string Secret
        {
            get => _secret ?? string.Empty;
            set => this.SetProperty(ref _secret, value);
        }

        private string _setupCode;

        public string SetupCode
        {
            get => _setupCode ?? string.Empty;
            set => this.SetProperty(ref _setupCode, value);
        }

        private BitmapSource _qrImageSource;

        public BitmapSource QrImageSource
        {
            get => _qrImageSource;
            set => this.SetProperty(ref _qrImageSource, value);
        }

        private ICommand _generateCodeCommand;

        public ICommand GenerateCodeCommand =>
            _generateCodeCommand ??= new RelayCommand(this.ExecuteGenerateCode);

        private void ExecuteGenerateCode()
        {
            var tfA = new TwoFactorAuthenticator.TwoFactorAuthenticator();
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

        private string _testCode;

        public string TestCode
        {
            get => _testCode;
            set => this.SetProperty(ref _testCode, value);
        }

        private ICommand _testCommand;

        public ICommand TestCommand =>
            _testCommand ??= new RelayCommand(this.ExecuteTestCommand);

        private void ExecuteTestCommand()
        {
            var tfA = new TwoFactorAuthenticator.TwoFactorAuthenticator();
            PasswordToken token = PasswordToken.FromPassCode(int.Parse(this.TestCode));
            bool result = tfA.ValidateTwoFactorPIN(this.Secret, token);

            MessageBox.Show(result ? "Validated!" : "Incorrect", "Result");
        }

        private string _currentCodes;

        public string CurrentCodes
        {
            get => _currentCodes;
            set => this.SetProperty(ref _currentCodes, value);
        }

        private ICommand _getCodesCommand;

        public ICommand GetCodesCommand =>
            _getCodesCommand ??= new RelayCommand(this.ExecuteGetCodesCommand);

        private void ExecuteGetCodesCommand()
        {
            this.CurrentCodes = string.Join(System.Environment.NewLine,
                new TwoFactorAuthenticator.TwoFactorAuthenticator()
                    .GetCurrentPINs(this.Secret));
        }
    }
}