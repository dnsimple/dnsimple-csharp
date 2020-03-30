#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

var configuration = Argument("configuration", "Debug");
var target = Argument("target", "Default");

var buildDir = Directory("./src/dnsimple/bin") + Directory(configuration);

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./dnsimple-csharp.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      MSBuild("./dnsimple-csharp.sln", settings =>
        settings.SetConfiguration(configuration));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./src/**/bin/" + configuration + "/*_test.dll", new NUnit3Settings {
        NoResults = true
        });
});

Task("Default")
    .IsDependentOn("Run-Unit-Tests");
    
RunTarget(target);
