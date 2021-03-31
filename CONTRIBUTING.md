# Contributing to DNSimple/C#

## Getting Started

#### 1. Clone the repository
Clone the repository and move into it:

```shell
git clone git@github.com:dnsimple/dnsimple-csharp.git
cd dnsimple-csharp
```

#### 2. Install dependencies

- .NET Core SDK

    From https://dotnet.microsoft.com/download/dotnet-core/

    You can either use to install the .NET Core SDK
        - Installers
        - Binaries
        - [Scripts](https://dotnet.microsoft.com/download/dotnet-core/scripts)
        - Install [Visual Studio](https://visualstudio.microsoft.com/)

#### 3. Build and test

[Run the test suite](#testing) to check everything is working as expected and to install the project specific
dependencies (the first time you'll run the script it will install all the dependencies for you).

To run the test suite:

```shell
dotnet test
```

## Releasing

The following instructions uses $VERSION as a placeholder, where $VERSION is a MAJOR.MINOR.BUGFIX release such as 1.2.0.

1. Run the test suite and ensure all the tests pass.
1. Update `PackageReleaseNotes` in `dnsimple.csproj` (located in `./src/dnsimple`).
1. Finalize the `## main` section in `CHANGELOG.md` assigning the version.
1. Commit and push the changes
    ```shell
    git commit -a -m "Release $VERSION"
    git push origin main
    ```
1. Wait for the CI to complete.
1. Create a signed tag.
    ```shell
    git tag -a v$VERSION -s -m "Release $VERSION"
    git push origin --tags
    ```
1. Create the NuGet Package
    ```shell
    dotnet pack /p:PackageVersion=$VERSION
    ```
1. Upload the package to [NuGet](https://www.nuget.org/) by using the web interface or pushing the package
    ```shell
    dotnet nuget push <path/to/file.nupkg>
    ```

## Testing

Submit unit tests for your changes. You can test your changes on your machine by [running the test suite](#testing).

When you submit a PR, tests will also be run on the [continuous integration environment via Travis](https://travis-ci.com/dnsimple/dnsimple-csharp).
