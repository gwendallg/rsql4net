#addin nuget:?package=Cake.Coverlet&version=2.5.4
#addin nuget:?package=Cake.FileHelpers&version=5.0.0
#addin nuget:?package=Cake.Sonar&version=1.1.30

#tool "dotnet:?package=GitVersion.Tool&version=5.10.3"
#tool "nuget:?package=ReportGenerator&version=5.1.10"
#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0"

var configuration = Argument("configuration", "Debug");
// coverage configuration
var coverageDirectory = Directory(@"./coverage-results/");
var coverageFileName = "coverage.xml";
var coverageFilePath = coverageDirectory + File(coverageFileName);
var mergeCoverageFileName = "merge-coverage.xml";
var mergeCoverageFilePath = coverageDirectory + File(mergeCoverageFileName);
var coverageReportTypes = "Html";

// arfifact configuration
var artifactDirectory = Directory(@"./artifacts/");
var artifactFileName = "RSql4Net.nuspec";
var artifactFilePath = artifactDirectory + File(artifactFileName);

GitVersion version;

// sonarcloud configuration
var sonarCloudLogin = EnvironmentVariable("SONAR_CLOUD_LOGIN") ?? "";

Task("Clean")
    .Does(() =>
{
    CleanDirectory("src/RSql4Net/bin/" + configuration);
    CleanDirectory("src/RSql4Net.Tests/bin/" + configuration);
    CleanDirectory("src/RSql4Net.Samples/bin/" + configuration);
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
    .IsDependentOn("Version")
    .Does(()=>{
      DotNetRestore("./RSql4Net.sln");
});

Task("SonarBegin")
    .IsDependentOn("Restore")
    .Does(() => {
        if(BuildSystem.AzurePipelines.IsRunningOnAzurePipelines)
        {
            SonarBegin(new SonarBeginSettings{
                Language = "C#",
                Organization = "gwendallg",
                Key = "gwendallg_rsql4net",
                Branch = version.BranchName,
                OpenCoverReportsPath = EnvironmentVariable("SYSTEM_DEFAULTWORKINGDIRECTORY") + "/coverage-results/coverage.xml",
                Url = "https://sonarcloud.io",
                Login = sonarCloudLogin
            });
        }
});

Task("Build")
    .IsDependentOn("SonarBegin")
    .Does(()=>{
      DotNetBuild(
          "RSql4Net.sln",
          new DotNetBuildSettings(){
            Configuration = configuration,
            ArgumentCustomization = args => args.Append("/nowarn:CS3021,S3776,S4823,MSB3277")
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
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = coverageDirectory,
        CoverletOutputName =  coverageFileName,
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

    Coverlet(
        File("./src/RSql4Net.Tests/RSql4Net.Tests.csproj"),
        coverletSettings);
});

Task("SonarEnd")
    .IsDependentOn("Tests")
    .Does(() => {
        if(BuildSystem.AzurePipelines.IsRunningOnAzurePipelines)
        {
            SonarEnd(new SonarEndSettings(){
                Login = sonarCloudLogin
            });
        }
    });
    

Task("Package")
    .IsDependentOn("SonarEnd")
    .Does(() =>{
        CopyFile("./src/RSql4Net/RSql4Net.nuspec",artifactFilePath);
        ReplaceTextInFiles(artifactFilePath,"{{version}}",version.SemVer);
        ReplaceTextInFiles(artifactFilePath,"{{configuration}}",configuration);
        NuGetPack(artifactFilePath, new NuGetPackSettings{
            OutputDirectory = artifactDirectory,
            Verbosity = NuGetVerbosity.Detailed,
        });
    });

var target = Argument("target", "Package");
RunTarget(target);