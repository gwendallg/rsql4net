#addin Cake.Git&version=0.21.0
#addin Cake.Coverlet&version=2.4.2
#addin Cake.Coveralls&version=0.10.0
#addin Cake.Sonar&version=1.1.25

#tool nuget:?package=ReportGenerator&version=4.5.8
#tool nuget:?package=GitVersion.CommandLine&version=4.0.0
#tool nuget:?package=Coveralls.net&version=1.0.0
#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0

var target = Argument("target", "Report");
var configuration = Argument("configuration", "release");
var lastCommit = GitLogTip("./");
var nugetApiKey = EnvironmentVariable("NUGET_API_KEY") ?? "";
var coverallsRepoToken = EnvironmentVariable("COVERALLS_REPO_TOKEN") ?? "";
var sonarCloudLogin = EnvironmentVariable("SONAR_CLOUD_LOGIN") ?? "";
var testProjectsRelativePaths = new string[]
{
    "RSql4Net.Tests/RSql4Net.Tests.csproj",
};

var coverageDirectory = Directory(@".\coverage-results\");
var artifactDirectory = Directory(@".\artifacts\");
var coverageFileName = "coverage.xml";
var reportTypes = "Html";
var coverageFilePath = coverageDirectory + File(coverageFileName);

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
        CoverletOutputName =  coverageFileName,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        ExcludeByFile = new List<string>( new string [] {
            "**/*/RSqlQueryBaseListener.cs",
            "**/*/RSqlQueryVisitor.cs.",
            "**/*/RSqlQueryParser.cs",
            "**/*/RSqlQueryLexer.cs",
            "**/*/ApplicationBuilderExtensions.cs",
            "**/*/ServiceCollectionExtensions.cs",
            "**/*/RSqlQueryBaseVisitor.cs",
            "**/*/RSqlQueryErrorNodeException.cs",
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
            RepoToken = coverallsRepoToken
        });
     }
});

Task("Sonar-end")
    .IsDependentOn("Tests")
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
        NuGetPack("./src/RSql4Net/RSql4Net.csproj", new NuGetPackSettings{
            Version = version.SemVer,
            OutputDirectory = artifactDirectory,
            IncludeReferencedProjects = true,
            Verbosity = NuGetVerbosity.Detailed,
            Copyright = "MIT",
            ProjectUrl = new Uri("https://github.com/gwendallg/rsql4net"),
            RequireLicenseAcceptance = true,
            Description = "Rsql4net is AspNet Core extension that will make it easier for you to write your REST APIs. Its purpose is to convert a query in RSQL format to lambda expression. Install it and test it!",
            KeepTemporaryNuSpecFile = true
        }
    );
});

Task("Report")
    .IsDependentOn("Package")
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