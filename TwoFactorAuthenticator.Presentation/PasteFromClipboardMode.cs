namespace TwoFactorAuthenticator.Presentation;

public enum PasteFromClipboardMode
{
    /// <summary>
    /// Do not allow paste from clipboard
    /// </summary>
    PasteNotAllowed,
    /// <summary>
    /// Only paste token from clipboard with exact length
    /// </summary>
    PasteFullToken,
    /// <summary>
    /// Paste any digit that is contained in the clipboard text 
    /// </summary>
    PasteAnyDigit
}