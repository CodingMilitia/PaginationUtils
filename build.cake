#tool "nuget:?package=coveralls.io&version=1.4.2"
#addin Cake.Git
#addin nuget:?package=Nuget.Core
#addin "nuget:?package=Cake.Coveralls&version=0.9.0"

using NuGet;


//////////////////////////////////////////////////////
//      CONSTANTS AND ENVIRONMENT VARIABLES         //
//////////////////////////////////////////////////////

var target = Argument("target", "Default");
var artifactsDir = "./artifacts/";
var solutionPath = "./CodingMilitia.Utils.Pagination.sln";
var currentBranch = Argument<string>("currentBranch", GitBranchCurrent("./").FriendlyName);
var isReleaseBuild = string.Equals(currentBranch, "master", StringComparison.OrdinalIgnoreCase);
var isDevelopBuild = string.Equals(currentBranch, "develop", StringComparison.OrdinalIgnoreCase);
var configuration = "Release";
var publicNugetApiKey = Argument<string>("publicNugetApiKey", null);
var privateNugetApiKey = Argument<string>("privateNugetApiKey", null);
var publicNugetSource = "https://api.nuget.org/v3/index.json";
var privateNugetSource = "https://pkgs.dev.azure.com/CodingMilitiaOrg/_packaging/CodingMilitia/nuget/v3/index.json";

//////////////////////////////////////////////////////
//                     TASKS                        //
//////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        if (DirectoryExists(artifactsDir))
        {
            DeleteDirectory(
                artifactsDir, 
                new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                }
            );
        }
        CreateDirectory(artifactsDir);
        DotNetCoreClean(solutionPath);
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionPath);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(
            solutionPath,
            new DotNetCoreBuildSettings 
            {
                Configuration = configuration
            }
        );
    });

Task("Test")
    .Does(() => {
        DotNetCoreTest(solutionPath);
    });

Task("PackagePublic")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = artifactsDir,
            NoBuild = true,
            IncludeSymbols = true,
            IncludeSource = true,
        };
        DotNetCorePack(solutionPath, settings);
    });

Task("PackagePrivate")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = artifactsDir,
            NoBuild = true,
            IncludeSymbols = true,
            IncludeSource = true,
            VersionSuffix = DateTime.UtcNow.ToString("yyyyMMddhhmmss")
        };
        DotNetCorePack(solutionPath, settings);
    });

Task("PublishPublic")
    .IsDependentOn("PackagePublic")
    .Does(() => {
        PublishToNuget(publicNugetSource, publicNugetApiKey);
    });

Task("PublishPrivate")
    .IsDependentOn("PackagePrivate")
    .Does(() => {
        PublishToNuget(privateNugetSource, privateNugetApiKey);
    });

//////////////////////////////////////////////////////
//                     TARGETS                      //
//////////////////////////////////////////////////////

Task("BuildAndTest")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("CompleteWithoutPublish")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

if(isReleaseBuild)
{
    Information("Release build - Will publish artifacts to public NuGet feed");       
    
    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("PublishPublic");
}
else if (isDevelopBuild) 
{
    Information("Develop build - Will publish artifacts to private NuGet feed");

    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("PublishPrivate");
}
else
{
    Information("Development build");
    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test");
}

Task("Default")
    .IsDependentOn("Complete");


RunTarget(target);


//////////////////////////////////////////////////////
//                      HELPERS                     //
//////////////////////////////////////////////////////

private bool IsNuGetPublished(FilePath packagePath) {
    var package = new ZipPackage(packagePath.FullPath);

    var latestPublishedVersions = NuGetList(
        package.Id,
        new NuGetListSettings 
        {
            Prerelease = true
        }
    );

    return latestPublishedVersions.Any(p => package.Version.Equals(new SemanticVersion(p.Version)));
}

private void PublishToNuget(string source, string apiKey)
{
    if(string.IsNullOrWhiteSpace(source))
    {
        throw new ArgumentNullException(nameof(source));
    }
    if(string.IsNullOrWhiteSpace(apiKey))
    {
        throw new ArgumentNullException(nameof(apiKey));
    }

    var pushSettings = new DotNetCoreNuGetPushSettings 
    {
        Source = source,
        ApiKey = apiKey
    };

    var pkgs = GetFiles(artifactsDir + "*.nupkg");
    foreach(var pkg in pkgs) 
    {
        if(!IsNuGetPublished(pkg)) 
        {
            Information($"Publishing \"{pkg}\".");
            DotNetCoreNuGetPush(pkg.FullPath, pushSettings);
        }
        else {
            Information($"Bypassing publishing \"{pkg}\" as it is already published.");
        }
        
    }
}