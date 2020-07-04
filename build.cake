#addin Cake.Git&version=0.21.0
#addin Cake.Coverlet&version=2.4.2
#addin Cake.Coveralls&version=0.10.0
#addin Cake.Sonar&version=1.1.25
#addin Cake.FileHelpers&version=3.2.1

#tool nuget:?package=ReportGenerator&version=4.5.8
#tool nuget:?package=GitVersion.CommandLine&version=5.0.1
#tool nuget:?package=Coveralls.net&version=1.0.0
#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0

var target = Argument("target", "Report");
var configuration = Argument("configuration", "Debug");
var lastCommit = GitLogTip("./");

// coverallio configuration
var coverallsRepoToken = EnvironmentVariable("COVERALLS_REPO_TOKEN") ?? "";

// sonarcloud configuration
var sonarCloudLogin = EnvironmentVariable("SONAR_CLOUD_LOGIN") ?? "";

// nuget publish configuration
var nugetApiKey = EnvironmentVariable("NUGET_API_KEY") ?? "";
var nugetSource= "https://api.nuget.org/v3/index.json";

// coverage configuration
var coverageDirectory = Directory(@"./coverage-results/");
var coverageFileName = "coverage.xml";
var coverageFilePath = coverageDirectory + File(coverageFileName);
var mergeCoverageFileName = "merge-coverage.json";
var mergeCoverageFilePath = coverageDirectory + File(mergeCoverageFileName);
var coverageReportTypes = "Html";

// arfifact configuration
var artifactDirectory = Directory(@"./artifacts/");
var artifactFileName = "RSql4Net.nuspec";
var artifactFilePath = artifactDirectory + File(artifactFileName);

GitVersion version;

Task("Clean")
    .Does(() =>
{
    CleanDirectory("src/RSql4Net/bin/"+configuration);
    CleanDirectory("src/RSql4Net.Tests/bin/"+configuration);
    CleanDirectory("src/RSql4Net.Samples/bin/"+configuration);
    if (!DirectoryExists(coverageDirectory))
        CreateDirectory(coverageDirectory);
    else
        CleanDirectory(coverageDirectory);
    if (!DirectoryExists(artifactDirectory))
        CreateDirectory(artifactDirectory);
    else
        CleanDirectory(artifactDirectory);
});

Task("Version")
    .IsDependentOn("Clean")
    .Does(()=>{
        version = GitVersion(
            new GitVersionSettings {
                //UpdateAssemblyInfo = true
        });
        Information($"SemVer: {version.SemVer}");
});

Task("Restore")
    .Does(()=>{
      DotNetCoreRestore("./RSql4Net.sln");
});

Task("Sonar-begin")
    .IsDependentOn("Restore")
    .IsDependentOn("Version")
    .Does(() => {
        if(BuildSystem.TravisCI.IsRunningOnTravisCI)
        {
            SonarBegin(new SonarBeginSettings{
                Language = "C#",
                Organization = "gwendallg",
                Key = "gwendallg_rsql4net",
                Branch = version.BranchName,
                OpenCoverReportsPath = "./coverage-results/coverage.xml",
                Url = "https://sonarcloud.io",
                Login = sonarCloudLogin
            });
        }
});

Task("Build")
    .IsDependentOn("Sonar-begin")
    .Does(()=>{
      DotNetCoreBuild(
          "RSql4Net.sln",
          new DotNetCoreBuildSettings(){
            Configuration = configuration,
            ArgumentCustomization = args => args.Append("/nowarn:CS3021,S3776,S4823")
          }
     );
});

Task("Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
   var coverletSettings = new CoverletSettings
    {
        CollectCoverage = true,
        CoverletOutputDirectory = coverageDirectory,
        CoverletOutputName =  mergeCoverageFileName,
        ExcludeByFile = new List<string>( new string [] {
            "**/*/RSqlQueryBaseListener.cs",
            "**/*/RSqlQueryVisitor.cs.",
            "**/*/RSqlQueryParser.cs",
            "**/*/RSqlQueryLexer.cs",
            "**/*/ApplicationBuilderExtensions.cs",
            "**/*/ServiceCollectionExtensions.cs",
            "**/*/RSqlQueryBaseVisitor.cs",
            "**/*/RSqlQueryErrorNodeException.cs",
            "**/*/RSql4Net.Samples/Models/Customer.cs",
            "**/*/RSql4Net.Samples/Models/Address.cs",
            "**/*/Program.cs",
            "**/*/Startup.cs",
        })
    };
    // RSql4net.Tests
    Coverlet(
        File("./src/RSql4Net.Tests/RSql4Net.Tests.csproj"),
        coverletSettings);

     coverletSettings.CoverletOutputName = coverageFileName;
     coverletSettings.MergeWithFile = mergeCoverageFilePath;
     coverletSettings.CoverletOutputFormat = CoverletOutputFormat.opencover;

    // RSql4net.SamplesTests
    Coverlet(
        File("./src/RSql4Net.Samples.Tests/RSql4Net.Samples.Tests.csproj"),
         coverletSettings);
});

Task("Publish-Coverage-Report")
    .IsDependentOn("Tests")
    .Does(() =>
{
    if(BuildSystem.TravisCI.IsRunningOnTravisCI)
    {
        CoverallsNet("./coverage-results/coverage.xml", CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
        {
            CommitBranch = version.BranchName,
            CommitAuthor = lastCommit.Author.Name,
            CommitMessage = lastCommit.MessageShort,
            CommitId = version.Sha,
            RepoToken = coverallsRepoToken,
            JobId = TravisCI.Environment.Build.BuildNumber.ToString()
        });
     }
});

Task("Sonar-end")
    .IsDependentOn("Publish-Coverage-Report")
    .Does(() => {
        if(BuildSystem.TravisCI.IsRunningOnTravisCI)
        {
            SonarEnd(new SonarEndSettings(){
                Login = sonarCloudLogin
            });
        }
});

Task("Package")
    .IsDependentOn("Sonar-end")
    .Does(() =>{
        CopyFile("./src/RSql4Net/RSql4Net.nuspec",artifactFilePath);
        ReplaceTextInFiles(artifactFilePath,"{{version}}",version.SemVer);
        ReplaceTextInFiles(artifactFilePath,"{{configuration}}",configuration);
        NuGetPack(artifactFilePath, new NuGetPackSettings{
            OutputDirectory = artifactDirectory,
            Verbosity = NuGetVerbosity.Detailed,
        });
    });

Task("Publish-package")
    .IsDependentOn("Package")
    .Does(() => {
        if(BuildSystem.TravisCI.IsRunningOnTravisCI)
        {
            var pushSettings = new DotNetCoreNuGetPushSettings
            {
                Source = nugetSource,
                ApiKey = nugetApiKey
            };

            var pkgs = GetFiles("./artifacts/*.nupkg");
            foreach(var pkg in pkgs)
            {
                Information($"Publishing \"{pkg}\".");
                DotNetCoreNuGetPush(pkg.FullPath, pushSettings);
            }
        }
    });

Task("Report")
    .IsDependentOn("Publish-package")
    .Does(() =>
{
    if(!BuildSystem.TravisCI.IsRunningOnTravisCI)
    {
        var reportSettings = new ReportGeneratorSettings
        {
            ArgumentCustomization = args => args.Append($"-reportTypes:{coverageReportTypes}")
        };
        ReportGenerator(coverageFilePath, coverageDirectory, reportSettings);
    }
});

RunTarget(target);