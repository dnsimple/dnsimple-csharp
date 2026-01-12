# Contributing to DNSimple/C\#

## Getting Started

### 1. Clone the repository

Clone the repository and move into it:

```shell
git clone git@github.com:dnsimple/dnsimple-csharp.git
cd dnsimple-csharp
```

### 2. Install dependencies

- .NET Core SDK

    From [https://dotnet.microsoft.com/download/](https://dotnet.microsoft.com/download/)

    You can either use to install the .NET Core SDK
        - Installers
        - Binaries
        - [Scripts](https://dotnet.microsoft.com/download/dotnet/scripts)
        - Install [Visual Studio](https://visualstudio.microsoft.com/)

### 3. Build and test

[Run the test suite](#testing) to check everything is working as expected and to install the project specific dependencies (the first time you'll run the script it will install all the dependencies for you).

## Changelog

We follow the [Common Changelog](https://common-changelog.org/) format for changelog entries.

## Testing

Submit unit tests for your changes. You can test your changes on your machine by running:

```shell
dotnet test
```

When you submit a PR, tests will also be run on the [continuous integration environment via GitHub Actions](https://github.com/dnsimple/dnsimple-csharp/actions).
