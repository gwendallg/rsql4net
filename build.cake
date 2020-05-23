#addin Cake.Git&version=0.21.0
#addin Cake.Coverlet&version=2.4.2
#addin Cake.Coveralls&version=0.10.0

#tool nuget:?package=ReportGenerator&version=4.5.8
#tool nuget:?package=GitVersion.CommandLine&version=4.0.0
#tool nuget:?package=Coveralls.net&version=1.0.0

var target = Argument("target", "Report");
var configuration = Argument("configuration", "debug");
var lastCommit = GitLogTip("./");

GitVersion version;

var testProjectsRelativePaths = new string[]
{
    "RSql4Net.Tests/RSql4Net.Tests.csproj",
};

var coverageDirectory = Directory(@".\coverage-results\");
var artifactDirectory = Directory(@".\artifacts\");
var coverageFileName = "coverage.xml";
var reportTypes = "Html";
var coverageFilePath = coverageDirectory + File(coverageFileName);

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
});

Task("Version")
    .IsDependentOn("Clean")
    .Does(()=>{
        version = GitVersion(
            new GitVersionSettings {
                UpdateAssemblyInfo = true
        });
        Information($"SemVer: {version.SemVer}");
});

Task("Restore")
    .Does(()=>{
      DotNetCoreRestore("./RSql4Net.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Version")
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
        CoverletOutputName =  coverageFileName,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        ExcludeByFile = new List<string>( new string [] {
            "**/*/QueryBaseListener.cs",
            "**/*/QueryVisitor.cs.",
            "**/*/QueryParser.cs",
            "**/*/QueryLexer.cs",
            "**/*/ApplicationBuilderExtensions.cs",
            "**/*/ServiceCollectionExtensions.cs",
            "**/*/QueryBaseVisitor.cs",
            "**/*/QueryValueInvalidConversionException.cs",
            "**/*/QueryComparisonNotEnoughtArgumentException.cs",
            "**/*/QueryValueInvalidConversionException.cs",
            "**/*/QueryErrorNodeException.cs",
            "**/*/QueryValueException.cs",
            "**/*/RSql4Net.Samples*"
        })
    };
    Coverlet(
        File("./src/RSql4Net.Tests/RSql4Net.Tests.csproj"),
        coverletSettings);
});

Task("Upload-Coverage-Report")
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
            RepoToken = "Ee2WELHZk5rns4D9nGDBWgfroT48JNsUI"
        });
     }
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>{
        NuGetPack("./src/RSql4Net/RSql4Net.nuspec", new NuGetPackSettings{
            Version = version.SemVer,
            OutputDirectory = artifactDirectory,
            Verbosity = NuGetVerbosity.Detailed,
        }
    );
});

Task("Report")
    .IsDependentOn("Tests")
    .Does(() =>
{
    if(!BuildSystem.TravisCI.IsRunningOnTravisCI)
    {
        var reportSettings = new ReportGeneratorSettings
        {
            ArgumentCustomization = args => args.Append($"-reportTypes:{reportTypes}")
        };
        ReportGenerator(coverageFilePath, coverageDirectory, reportSettings);
    }
});


RunTarget(target);