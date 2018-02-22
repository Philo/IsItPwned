# IsItPwned

Provides a simple interface to the I Have Been Pwned V2 breached password endpoint using the enhanced anonymity provided by the k-Anonymity method (/range endpoint).  

Clearly all the credit should goto Troy Hunt, I would encourage you to read his blog on regarding the release of the V2 API.

[I've Just Launched "Pwned Passwords" V2 With Half a Billion Passwords for Download](https://www.troyhunt.com/ive-just-launched-pwned-passwords-version-2/)

The exact documentation for the API is also available on the [Have I Been Pwned](https://haveibeenpwned.com/API/v2#PwnedPasswords) website.

## Build Status

[![Build status](https://ci.appveyor.com/api/projects/status/p4idjcts2me0rwnu/branch/master?svg=true)](https://ci.appveyor.com/project/Philo/isitpwned/branch/master)

## Install via NuGet

```
Install-Package IsItPwned
```

## Usage

Take a look at the unit tests for examples of the usage, there really isnt anything to it!

```c#
    // Create a client, optionally you can pass in your own HttpClient
    var client = new PwnedPasswordClient();

    var countOfPasswordInBreaches = await client.IsPwnedAsync(password);

    Assert.True(countOfPasswordInBreaches == 0, "Hooray, this password does not exist in half a billion breaches passwords");
    Assert.True(countOfPasswordInBreaches > 0, "Oh no, this password exists in at least one data breach");
    Assert.True(countOfPasswordInBreaches > 1000, "Oh dear, this really is a popular password!!");
```

As you can see, the `.IsPwnedAsync()` call simply returns the number of breaches found, you can then determine whether you would be accept or deny the use of the password based on your requirements.

## Credit

Troy Hunt [Website](https://www.troyhunt.com) | [Twitter](https://twitter.com/troyhunt)