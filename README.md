# DNSimple C# Client

A C# client for the [DNSimple API v2](https://developer.dnsimple.com/v2/).

[![Build Status](https://github.com/dnsimple/dnsimple-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/dnsimple/dnsimple-csharp/actions/workflows/ci.yml)
[![NuGet version](https://badge.fury.io/nu/dnsimple.svg)](https://badge.fury.io/nu/dnsimple)

## Installation

Where `<version>` denotes the version of the client you want to install.

### Package Manager

```shell
PM> Install-Package DNSimple -Version <version>
```

## Usage

This library is a C# client you can use to interact with the [DNSimple API v2](https://developer.dnsimple.com/v2/). Here are some examples.

```c#
using dnsimple;

var client = new Client();
var credentials = new OAuth2Credentials("...");
client.AddCredentials(credentials);

// Fetch your details
var response = client.Identity.Whoami();   // execute the call
var account = response.Data.Account;       // extract the relevant data from the response or
client.Identity.Whoami().Data.Account;     // execute the call and get the data in one line

// You can also fetch it from the whoami response
// as long as you authenticate with an Account access token
var whoami = client.Identity.Whoami().Data;
var accountId = whoami.Account.Id;
```

### List your domains

```c#
using dnsimple;

var client = new Client();
client.AddCredentials(new OAuth2Credentials("..."));

var accountId = client.Identity.Whoami().Data.Account.Id;
var domains = client.Domains.ListDomains(accountId).Data;
```

### Create a domain

```c#
using dnsimple;

var client = new Client();
client.AddCredentials(new OAuth2Credentials("..."));

var accountId = client.Identity.Whoami().Data.Account.Id;
var domain = client.Domains.CreateDomain(accountId, new Domain{ Name = "example.com" }).Data;
```

### Retrieve a domain

```c#
using dnsimple;

var client = new Client();
client.AddCredentials(new OAuth2Credentials("..."));

var accountId = client.Identity.Whoami().Data.Account.Id;
var domainId = client.Domains.ListDomains(accountId).Data.First().Id;
var domain = client.Domains.GetDomain(accountId, domainId).Data;
```

## Sandbox Environment

We highly recommend testing against our [sandbox environment](https://developer.dnsimple.com/sandbox/) before using our production environment.
This will allow you to avoid real purchases, live charges on your credit card, and reduce the chance of your running up against rate limits.

The client supports both the production and sandbox environment.
To switch to sandbox pass the sandbox API host using the `ChangeBaseUrlTo(...)` method when you construct the client:

```c#
var client = new Client();
client.ChangeBaseUrlTo("https://api.sandbox.dnsimple.com");

var credentials = new OAuth2Credentials("...");
client.AddCredentials(credentials);
```

You will need to ensure that you are using an access token created in the sandbox environment. Production tokens will *not* work in the sandbox environment.

## Setting a custom `User-Agent` header

You can customize the `User-Agent` header for the calls made to the DNSimple API:

```c#
var client = new Client();
client.SetUserAgent("my-app/1.0");
```

The value you provide will be prepended to the default `User-Agent` the client uses. For example, if you use `my-app/1.0`, the final header value will be `my-app/1.0 dnsimple-csharp/0.14.0` (note that it will vary depending on the client version).

We recommend to customize the user agent. If you are building a library or integration on top of the official client, customizing the client will help us to understand what is this client used for, and allow to contribute back or get in touch.

## License

Copyright (c) 2022 DNSimple Corporation. This is Free Software distributed under the MIT license.
