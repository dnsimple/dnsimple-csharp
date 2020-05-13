# Contributing to DNSimple/C#

## Getting Started

#### 1. Clone the repository
Clone the repository and move into it:

```shell
git clone git@github.com:dnsimple/dnsimple-csharp.git
cd dnsimple-csharp
```

#### 2. Install dependencies

- Linux

    Please follow the instructions on the [Mono Project Page](https://www.mono-project.com/download/stable/#download-lin)
    
- macOS

    Install .NET [mono](https://www.mono-project.com/) either by [dowloading mono](https://www.mono-project.com/download/stable/#download-mac)
    directly or by using [Homebrew](https://brew.sh)

    ```
    brew install mono
    ```

    Make sure your dotnet installation is working.

    ```shell
    mono --version
    ```

- Windows

    Please follow the instructions on the [Microsoft Docs Website](https://docs.microsoft.com/en-us/dotnet/framework/install)

#### 3. Build and test

[Run the test suite](#testing) to check everything is working as expected and to install the project specific 
dependencies (the first time you'll run the script it will install all the dependencies for you).

To run the test suite: 

- macOS / *NIX
    ```shell
    ./build.sh
    ```
  
- Windows
    ```shell
    PS> .\build.ps1
    ```

## Releasing

The following instructions uses `$VERSION` as a placeholder, where `$VERSION` is a `MAJOR.MINOR.BUGFIX` release such as `1.2.0`.

1. Run the test suite and ensure all the tests pass.
2. Set the version in `AssemblyInfo.cs` (located in `./src/dnsimple/Properties`).
    ```c#
    [assembly: AssemblyVersion("$VERSION")]
    [assembly: AssemblyFileVersion("$VERSION")]
    ```
3. Run the test suite and ensure all tests pass (`./build.sh`).
4. Finalize the `## master` section in `CHANGELOG.md` assigning the version.
5. Commit and push the changes
    ```shell
    git commit -a -m "Release $VERSION"
    git push origin master
    ```
6. Wait for the CI to complete.
7. Create a signed tag.
    ```shell
    git tag -a v$VERSION -s -m "Release $VERSION"
    git push origin --tags
    ```
 8. Create the NuGet Package
    ```shell
    ./build.sh --target=Package
    ```
    
 9. Upload the package to [NuGet](https://www.nuget.org/) by using the web interface or pushing the package
    ```shell
    nuget push <path/to/file.nupkg>
    ```

## Testing

Submit unit tests for your changes. You can test your changes on your machine by [running the test suite](#testing).

When you submit a PR, tests will also be run on the [continuous integration environment via Travis](https://travis-ci.com/dnsimple/dnsimple-csharp).

