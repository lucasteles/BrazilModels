class MainBuild : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration =
        IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter(List = false)] readonly bool DotnetRunningInContainer;
    [GlobalJson] readonly GlobalJson GlobalJson;

    [Parameter("Don't open the coverage report")]
    readonly bool NoBrowse;

    [Solution] readonly Solution Solution;
    [Parameter] readonly string TestResultFile = "test_result.xml";
    AbsolutePath CoverageFiles => RootDirectory / "**" / "coverage.cobertura.xml";
    AbsolutePath TestReportDirectory => RootDirectory / "TestReport";
    AbsolutePath DocsPath => RootDirectory / "docfx";
    AbsolutePath DocsSitePath => DocsPath / "_site";
    const string MainBranch = "master";

    Target Clean => _ => _
        .Description("Clean project directories")
        .Executes(() => new[] { "src", "tests" }
            .Select(path => RootDirectory / path)
            .SelectMany(dir => dir
                .GlobDirectories("**/bin", "**/obj", "**/TestResults"))
            .Append(TestReportDirectory)
            .ForEach(x => x.CreateOrCleanDirectory()));

    Target Restore => _ => _
        .Description("Run dotnet restore in every project")
        .DependsOn(Clean)
        .Executes(() => DotNetRestore(s => s
            .SetProjectFile(Solution)));

    Target Build => _ => _
        .Description("Builds Solution")
        .DependsOn(Restore)
        .Executes(() =>
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoLogo()
                .EnableNoRestore()));

    Target Lint => _ => _
        .Description("Check for codebase formatting and analysers")
        .DependsOn(Build)
        .Executes(() => DotNetFormat(c => c
            .EnableNoRestore()
            .EnableVerifyNoChanges()
            .SetProject(Solution)));

    Target Format => _ => _
        .Description("Try fix codebase formatting and analysers")
        .DependsOn(Build)
        .Executes(() => DotNetFormat(c => c.SetProject(Solution)));

    Target Test => _ => _
        .Description("Run tests with coverage")
        .DependsOn(Build)
        .Executes(() => DotNetTest(s => s
            .SetVerbosity(DotNetVerbosity.minimal)
            .SetFilter("FullyQualifiedName!~Acceptance")
            .EnableNoBuild()
            .EnableNoRestore()
            .SetConfiguration(Configuration)
            .SetProjectFile(Solution)
            .SetLoggers($"trx;LogFileName={TestResultFile}")
            .SetSettingsFile(RootDirectory / "coverlet.runsettings")
        ))
        .Executes(() =>
        {
            ReportGenerator(r => r
                .SetReports(CoverageFiles)
                .SetTargetDirectory(TestReportDirectory)
                .SetReportTypes(ReportTypes.TextSummary));
            (TestReportDirectory / "Summary.txt")
                .ReadAllLines()
                .ForEach(l => Console.WriteLine(l));
        });

    Target GenerateReport => _ => _
        .Description("Generate test coverage report")
        .After(Test)
        .OnlyWhenDynamic(() => CoverageFiles.GlobFiles().Any())
        .Executes(() => ReportGenerator(r => r
            .SetReports(CoverageFiles)
            .SetTargetDirectory(TestReportDirectory)
            .SetReportTypes(
                ReportTypes.Html,
                ReportTypes.Clover,
                ReportTypes.Cobertura,
                ReportTypes.MarkdownSummary
            )));

    Target BrowseReport => _ => _
        .Description("Open coverage report")
        .OnlyWhenStatic(() => !NoBrowse && !DotnetRunningInContainer)
        .After(GenerateReport, GenerateBadges)
        .Unlisted()
        .Executes(() => OpenBrowser(TestReportDirectory / "index.htm"));

    Target Report => _ => _
        .Description("Run tests and generate coverage report")
        .DependsOn(Test)
        .Triggers(GenerateReport, BrowseReport);

    Target BuildDocs => _ => _
        .Description("Build DocFX")
        .Executes(() =>
        {
            DocsSitePath.CreateOrCleanDirectory();
            (DocsPath / "api").CreateOrCleanDirectory();
            DotNetBuild(s => s.SetProjectFile(Solution).SetConfiguration(Configuration.Release));
            DocFX.Build(c => c
                .SetProcessWorkingDirectory(DocsPath)
                .SetProcessEnvironmentVariable(DocFX.DocFXSourceBranchName, MainBranch)
            );
        });

    Target Docs => _ => _
        .Description("View DocFX")
        .Executes(() =>
        {
            DotNetBuild(s => s.SetProjectFile(Solution).SetConfiguration(Configuration.Release));
            DocFX.Serve(c => c
                .SetProcessWorkingDirectory(DocsPath)
                .SetProcessEnvironmentVariable(DocFX.DocFXSourceBranchName, MainBranch));
        });

    Target GenerateBadges => _ => _
        .Description("Generate cool badges for readme")
        .After(Test)
        .Requires(() => CoverageFiles.GlobFiles().Any())
        .Executes(() =>
        {
            var output = RootDirectory / "docfx" / "_site";
            if (!output.DirectoryExists()) output.CreateDirectory();
            Badges.ForCoverage(output, CoverageFiles);
            Badges.ForDotNetVersion(output, GlobalJson);
            Badges.ForTests(output, TestResultFile);
        });

    Target UpdateTools => _ => _
        .Description("Update all project .NET tools")
        .Executes(UpdateLocalTools);

    public static int Main() => Execute<MainBuild>();

    protected override void OnBuildInitialized() =>
        DotNetToolRestore(c => c.DisableProcessLogOutput());
}