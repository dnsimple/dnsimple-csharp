#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

var target = Argument("target", "Default");

var buildDir = Directory("./src/dnsimple/bin");

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
      MSBuild("./dnsimple-csharp.sln");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./src/**/bin/**/*_test.dll", new NUnit3Settings {
        NoResults = true
        });
});

Task("Package")
	.IsDependentOn("Test")
    .Does(() =>
{
        var nuGetPackSettings = new NuGetPackSettings
        	{
        		OutputDirectory = buildDir,
        		IncludeReferencedProjects = true,
        	};
        MSBuild("./dnsimple-csharp.sln");
        NuGetPack("./src/dnsimple/dnsimple.csproj", nuGetPackSettings);
});

Task("Default")
    .IsDependentOn("Test");
    
RunTarget(target);
