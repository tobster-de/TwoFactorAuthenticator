using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TwoFactorAuthenticator.QrCoder;

namespace TwoFactorAuthenticator.WinTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtIssuer.Text = "Example Org";
            this.txtAccountTitle.Text = "test@account.com";
            this.txtSecretKey.Text = "f68f1fe894d548a1bbc66165c46e61eb"; //Guid.NewGuid().ToString().Replace("-", "");
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfA = new TwoFactorAuthenticator();
            QrCoderSetupCodeGenerator qrscg = new QrCoderSetupCodeGenerator();
            
            SetupCode setupCode = tfA.GenerateSetupCode(this.txtIssuer.Text, this.txtAccountTitle.Text, this.txtSecretKey.Text, false);

            //WebClient wc = new WebClient();
            using (MemoryStream ms = new MemoryStream(setupCode.GetQrCodeImageData(qrscg)))
            {
                this.pbQR.Image = Image.FromStream(ms);
            }

            this.txtSetupCode.Text = "Account: " + setupCode.Account + System.Environment.NewLine +
                "Secret Key: " + this.txtSecretKey.Text + System.Environment.NewLine +
                "Encoded Key: " + setupCode.ManualEntryKey;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfA = new TwoFactorAuthenticator();
            var result = tfA.ValidateTwoFactorPIN(txtSecretKey.Text, this.txtCode.Text);

            MessageBox.Show(result ? "Validated!" : "Incorrect", "Result");
        }

        private void btnGetCurrentCode_Click(object sender, EventArgs e)
        {
            this.txtCurrentCodes.Text = string.Join(System.Environment.NewLine, new TwoFactorAuthenticator().GetCurrentPINs(this.txtSecretKey.Text));
        }

        private void btnDebugTest_Click(object sender, EventArgs e)
        {

        }
    }
}
