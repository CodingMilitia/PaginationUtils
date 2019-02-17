#addin Cake.Git
#addin nuget:?package=Nuget.Core

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

var releaseNugetSource = Argument<string>("releaseNugetSource", "https://api.nuget.org/v3/index.json");
var releaseNugetApiKey = Argument<string>("releaseNugetApiKey", null);

var developNugetSource = Argument<string>("developNugetSource", "https://pkgs.dev.azure.com/CodingMilitiaOrg/_packaging/CodingMilitia/nuget/v3/index.json");
var developNugetApiKey = Argument<string>("developNugetApiKey", null);

var releaseToAzureArtifacts = Argument<bool>("releaseToAzureArtifacts", false);
var developToAzureArtifacts = Argument<bool>("developToAzureArtifacts", false);

var azureAccessToken = Argument<string>("azureAccessToken", null);

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

Task("PackageMaster")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = artifactsDir,
            NoBuild = true,
            IncludeSymbols = true,
            IncludeSource = true,
            // only needed while there isn't out of the box support to package in this format
            ArgumentCustomization = args=>args.Append("--include-symbols").Append("-p:SymbolPackageFormat=snupkg")
        };
        DotNetCorePack(solutionPath, settings);
    });

Task("PackageDevelop")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = artifactsDir,
            NoBuild = true,
            IncludeSymbols = true,
            IncludeSource = true,
            // only needed while there isn't out of the box support to package in this format
            ArgumentCustomization = args=>args.Append("--include-symbols").Append("-p:SymbolPackageFormat=snupkg"),
            VersionSuffix = DateTime.UtcNow.ToString("yyyyMMddhhmmss")
        };
        DotNetCorePack(solutionPath, settings);
    });

Task("PublishMaster")
    .IsDependentOn("PackageMaster")
    .Does(() => {
        if(releaseToAzureArtifacts)
        {
            PublishToAzureArtifacts(releaseNugetSource);
        }
        else
        {
            PublishToNugetSource(releaseNugetSource, releaseNugetApiKey);
        }
        
    });

Task("PublishDevelop")
    .IsDependentOn("PackageDevelop")
    .Does(() => {
        if(developToAzureArtifacts)
        {
            PublishToAzureArtifacts(developNugetSource);
        }
        else
        {
            PublishToNugetSource(developNugetSource, developNugetApiKey);
        }        
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
        .IsDependentOn("PublishMaster");
}
else if (isDevelopBuild) 
{
    Information("Develop build - Will publish artifacts to private NuGet feed");

    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("PublishDevelop");
}
else
{
    Information("Non-publishable build");
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

private void PublishToNugetSource(string source, string apiKey)
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
        else 
        {
            Information($"Bypassing publishing \"{pkg}\" as it is already published.");
        }
        
    }
}

private void PublishToAzureArtifacts(string source)
{
    if (string.IsNullOrEmpty(azureAccessToken))
    {
        throw new InvalidOperationException("Could not resolve azureAccessToken");
    }

    // Add the authenticated feed source
    NuGetAddSource(
        "VSTS",
        source,
        new NuGetSourcesSettings
        {
            UserName = "VSTS",
            Password = azureAccessToken
        });

    var pkgs = GetFiles(artifactsDir + "*.nupkg");
    foreach(var pkg in pkgs) 
    {
        Information($"Publishing \"{pkg}\".");
        NuGetPush(pkg, new NuGetPushSettings 
        {
            Source = "VSTS",
            ApiKey = "VSTS"
        });
    }
}