# TwoFactorAuthenticator
Simple, easy to use server-side/desktop two-factor authentication library for .NET that works with authenticator apps
e.g. from [Google](https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2), 
from [Microsoft](https://play.google.com/store/apps/details?id=com.azure.authenticator), 
[Authy](https://play.google.com/store/apps/details?id=com.authy.authy) 
or [LastPass](https://play.google.com/store/apps/details?id=com.lastpass.authenticator).

<!---
[![Build Status](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_apis/build/status/BrandonPotter.GoogleAuthenticator?branchName=master)](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_build/latest?definitionId=1&branchName=master)
[![NuGet Status](https://buildstats.info/nuget/GoogleAuthenticator)](https://www.nuget.org/packages/GoogleAuthenticator/)

[`Install-Package GoogleAuthenticator`](https://www.nuget.org/packages/GoogleAuthenticator)
--->

## Usage

*Additional examples at [Google.Authenticator.WinTest](https://github.com/BrandonPotter/GoogleAuthenticator/tree/master/Google.Authenticator.WinTest) and [Google.Authenticator.WebSample](https://github.com/BrandonPotter/GoogleAuthenticator/tree/master/Google.Authenticator.WebSample)*

`key` should be stored by your application for future authentication and shouldn't be regenerated for each request. The process of storing the private key is outside the scope of this library and is the responsibility of the application.

```csharp
using TwoFactorAuthenticator;

string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
SetupCode setupInfo = tfa.GenerateSetupCode("Test Two Factor", "user@example.com", key, false, 3);

string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
string manualEntrySetupCode = setupInfo.ManualEntryKey;

imgQrCode.ImageUrl = qrCodeImageUrl;
lblManualSetupCode.Text = manualEntrySetupCode;

// verify
TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
bool result = tfa.ValidateTwoFactorPIN(key, txtCode.Text)
```

<!---
## Update history

### 3.0.0

- Removed support for legacy .Net Framework. Lowest supported versions are now netstandard2.0 and .Net 4.6.2.  
- All use of System.Drawing has been removed. In 2.5, only Net 6.0 avoided System.Drawing.
- Linux installations no longer need to ensure `libgdiplus` is installed as it is no longer used.
- Changed from using `EscapeUriString` to `EscapeDataString` to encode the "account title" as the former is [obsolete in .Net 6](https://docs.microsoft.com/en-us/dotnet/fundamentals/syslib-diagnostics/syslib0013). This changes the value in the generated data string from `a@b.com` to `a%40b.com`. We have tested this with Google Authenticator, Lastpass Authenticator and Microsoft Authenticator. All three of them handle it correctly and all three recognise that it is still the same account so this should be safe in most cases.

### 2.5.0

Now runs on .Net 6.0.  
Technically the QR Coder library we rely on still does not fully support .Net 6.0 so it is possible there will be other niggling issues, but for now all tests pass for .Net 6.0 on both Windows and Linux.
--->

## Common Pitfalls

* Don't use the secret key and `ManualEntryKey` interchangeably. `ManualEntryKey` is used to enter into the authenticator app when scanning a QR code is impossible and is derived from the secret key ([discussion example](https://github.com/BrandonPotter/GoogleAuthenticator/issues/54))
