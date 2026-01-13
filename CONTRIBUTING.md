# Contributing to DNSimple/C\#

## Getting Started

Clone the repository and move into it:

```shell
git clone git@github.com:dnsimple/dnsimple-csharp.git
cd dnsimple-csharp
```

Install the .NET Core SDK from [https://dotnet.microsoft.com/download/](https://dotnet.microsoft.com/download/). You can use any of the following methods:

- Installers
- Binaries
- [Scripts](https://dotnet.microsoft.com/download/dotnet/scripts)
- [Visual Studio](https://visualstudio.microsoft.com/)

## Changelog

We follow the [Common Changelog](https://common-changelog.org/) format for changelog entries.

## Testing

Submit unit tests for your changes. You can test your changes on your machine by running:

```shell
dotnet test
```

When you submit a PR, tests will also be run on the [continuous integration environment via GitHub Actions](https://github.com/dnsimple/dnsimple-csharp/actions).
