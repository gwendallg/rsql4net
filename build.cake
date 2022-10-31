#addin nuget:?package=Cake.Coverlet&version=2.5.4
#addin nuget:?package=Cake.FileHelpers&version=5.0.0
#addin nuget:?package=Cake.Sonar&version=1.1.30

#tool "dotnet:?package=GitVersion.Tool&version=5.10.3"
#tool "nuget:?package=ReportGenerator&version=5.1.10"
#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0"
#tool nuget:?package=Microsoft.Extensions.ApiDescription.Server&version=6.0.10

var configuration = Argument("configuration", "Debug");
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

Task("Build")
    .IsDependentOn("Restore")
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

Task("Package")
    .IsDependentOn("Tests")
    .Does(() =>{
        CopyFile("./src/RSql4Net/RSql4Net.nuspec",artifactFilePath);
        ReplaceTextInFiles(artifactFilePath,"{{version}}",version.SemVer);
        ReplaceTextInFiles(artifactFilePath,"{{configuration}}",configuration);
        NuGetPack(artifactFilePath, new NuGetPackSettings{
            OutputDirectory = artifactDirectory,
            Verbosity = NuGetVerbosity.Detailed,
        });
    });

Task("Report")
    .IsDependentOn("Package")
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

var target = Argument("target", "Report");
RunTarget(target);