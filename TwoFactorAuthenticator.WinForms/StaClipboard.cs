using System.Threading;

namespace TwoFactorAuthenticator.WinForms
{
    /// <summary>
    /// Ensures access to the clipboard occurs with a STA thread.
    /// </summary>
    internal static class StaClipboard
    {
        public static void SetText(string text)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                System.Windows.Forms.Clipboard.SetText(text);
                return;
            }

            Thread staThread = new Thread(() => System.Windows.Forms.Clipboard.SetText(text));
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }

        public static string GetText()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                return System.Windows.Forms.Clipboard.GetText();
            }

            string text = string.Empty;
            Thread staThread = new Thread(() => text = System.Windows.Forms.Clipboard.GetText());
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            return text;
        }
    }
}