class BuildProject : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter(List = false)] readonly bool DotnetRunningInContainer;
    [GlobalJson] readonly GlobalJson GlobalJson;

    [Parameter("Don't open the coverage report")]
    readonly bool NoBrowse;

    [Solution] readonly Solution Solution;
    [Parameter] readonly string TestResultFile = "test_result.xml";

    AbsolutePath CoverageFiles => RootDirectory / "**" / "coverage.cobertura.xml";
    AbsolutePath TestReportDirectory => RootDirectory / "TestReport";

    Target Clean => _ => _
        .Description("Clean project directories")
        .Executes(() => RootDirectory
            .GlobDirectories("**/bin", "**/obj", "**/TestResults")
            .Where(x => !x.ToString().StartsWith(BuildProjectDirectory))
            .ForEach(EnsureCleanDirectory));

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

    Target Test => _ => _
        .Description("Run all tests")
        .DependsOn(Build)
        .Executes(() => Solution
            .GetProjects("*.Tests")
            .ForEach(project =>
                DotNetTest(s => s
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetConfiguration(Configuration)
                    .SetProjectFile(project))));

    Target TestCoverage => _ => _
        .Description("Run tests with coverage")
        .DependsOn(Build)
        .Executes(() => DotNetTest(s => s
            .SetVerbosity(DotNetVerbosity.Minimal)
            .EnableNoBuild()
            .EnableNoRestore()
            .SetConfiguration(Configuration)
            .SetProjectFile(Solution)
            .SetLoggers($"trx;LogFileName={TestResultFile}")
            .SetSettingsFile(RootDirectory / "coverlet.runsettings")
        ))
        .Executes(() =>
        {
            ReportGenerator(r => r.LocalTool("reportgenerator")
                .SetReports(CoverageFiles)
                .SetTargetDirectory(TestReportDirectory)
                .SetReportTypes(ReportTypes.TextSummary));
            ReadAllLines(TestReportDirectory / "Summary.txt").ForEach(l => Console.WriteLine(l));
        });

    Target Lint => _ => _
        .Description("Check for codebase formatting and analysers")
        .DependsOn(Build)
        .Executes(() => DotNet($"format -v normal --no-restore --verify-no-changes \"{Solution.Path}\""));

    Target Format => _ => _
        .Description("Try fix codebase formatting and analysers")
        .DependsOn(Build)
        .Executes(() => DotNet($"format -v normal --no-restore \"{Solution.Path}\""));

    Target Report => _ => _
        .Description("Run tests and generate coverage report")
        .DependsOn(TestCoverage)
        .Triggers(GenerateReport, BrowseReport);

    Target GenerateReport => _ => _
        .Description("Generate test coverage report")
        .After(TestCoverage)
        .OnlyWhenDynamic(() => CoverageFiles.GlobFiles().Any())
        .Executes(() =>
            ReportGenerator(r => r
                .LocalTool("reportgenerator")
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
        .Executes(() =>
        {
            var path = TestReportDirectory / "index.htm";
            Assert.FileExists(path);
            try
            {
                BrowseHtml(path.ToString().DoubleQuoteIfNeeded());
            }
            catch (Exception e)
            {
                if (!IsWin) // Windows explorer always return 1
                    Log.Error(e, "Unable to open report");
            }
        });

    Target GenerateBadges => _ => _
        .Description("Generate cool badges for readme")
        .After(TestCoverage)
        .Requires(() => CoverageFiles.GlobFiles().Any())
        .Executes(() =>
        {
            var output = RootDirectory / "Badges";
            EnsureCleanDirectory(output);
            Badges.ForCoverage(output, CoverageFiles);
            Badges.ForDotNetVersion(output, GlobalJson);
            Badges.ForTests(output, TestResultFile);
        });


    public static int Main() => Execute<BuildProject>();

    protected override void OnBuildInitialized()
    {
        DockerLogger = (_, msg) => Log.Information(msg);
        DotNetToolRestore(c => c.DisableProcessLogOutput());
    }
}