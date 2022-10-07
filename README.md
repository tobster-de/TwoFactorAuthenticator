# TwoFactorAuthenticator
Simple, easy to use server-side/desktop two-factor authentication library for .NET that works with authenticator apps
e.g. from [Google](https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2), 
from [Microsoft](https://play.google.com/store/apps/details?id=com.azure.authenticator), 
[Authy](https://play.google.com/store/apps/details?id=com.authy.authy) 
or [LastPass](https://play.google.com/store/apps/details?id=com.lastpass.authenticator).

[![Build Status](https://dev.azure.com/tkolb80/TwoFactorAuthenticator/_apis/build/status/TwoFactorAuthenticator?branchName=master)](https://dev.azure.com/tkolb80/TwoFactorAuthenticator/_build/latest?definitionId=2&branchName=master)
[![NuGet Status](https://buildstats.info/nuget/TwoFactorAuthenticator)](https://www.nuget.org/packages/TwoFactorAuthenticator/)

[`Install-Package TwoFactorAuthenticator`](https://www.nuget.org/packages/TwoFactorAuthenticator)

## Usage

*Also see additional example projects at 
[TwoFactorAuthenticator.WinTest](https://github.com/tobster-de/TwoFactorAuthenticator/tree/master/TwoFactorAuthenticator.WinTest) 
and [TwoFactorAuthenticator.WebSample](https://github.com/tobster-de/TwoFactorAuthenticator/tree/master/TwoFactorAuthenticator.WebSample)*

`key` should be stored by your application for future authentication and shouldn't be regenerated for 
each request. The process of storing the private key is outside the scope of this library and is the 
responsibility of the application.

### Generate setup code
```csharp
using TwoFactorAuthenticator;
using TwoFactorAuthenticator.QrCoder;

string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

Authenticator tfa = new Authenticator();
QrCoderSetupCodeGenerator qrscg = new QrCoderSetupCodeGenerator { PixelsPerModule = 3 };

SetupCode setupInfo = tfa.GenerateSetupCode("Test Two Factor", "user@example.com", key, false);

string qrCodeImageUrl = setupInfo.GenerateQrCodeUrl(qrscg);

using (MemoryStream ms = new MemoryStream(setupCode.GetQrCodeImageData(qrscg)))
{
    qrCodePictureBox.Image = Image.FromStream(ms);
}

this.setupInfo.Text = "Account: " + setupCode.Account + System.Environment.NewLine +
                      "Encoded Key: " + setupCode.ManualEntryKey;
```

### Generation
```csharp
Authenticator tfa = new Authenticator();
PasswordToken token = tfa.GetCurrentPIN(key);
using (var unsafeToken = UnsafeToken.FromPasswordToken(token))
{
    string pin = unsafeToken.ToString();
}
```

### Verification
```csharp
string input = "123456";

Authenticator tfa = new Authenticator();
PasswordToken token = PasswordToken.FromPassCode(int.Parse(input));

bool result = tfa.ValidateTwoFactorPIN(key, token);
```

## History

### 1.1.0

- **Breaking changes**:
  - ```TwoFactorAuthenticator``` should not be named like its namespace (created collision);
  new name is just ```Authenticator```
  - Changed interface to use secured ```PasswordToken``` instead of primitive string
- Introduced ```UnsafeToken``` for generation / UI purposes
- Introduced ```FactorControl``` for WinForms

### 1.0.1

- Forked and separated into two packages
- Lowest supported versions are now netstandard2.0 and .Net 4.7.2.

## Common Pitfalls

* Don't use the secret key and `ManualEntryKey` interchangeably. `ManualEntryKey` is used to enter into 
  the authenticator app when scanning a QR code is impossible and is derived from the secret key
  ([discussion example](https://github.com/BrandonPotter/GoogleAuthenticator/issues/54))
