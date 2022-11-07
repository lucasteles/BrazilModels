using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Helpers;

public static class Badges
{
    public static void ForCoverage(AbsolutePath output, AbsolutePath files) =>
        ReportGenerator(r => r
            .LocalTool("reportgenerator")
            .SetReports(files)
            .SetTargetDirectory(output)
            .SetReportTypes(ReportTypes.Badges));

    public static void ForDotNetVersion(AbsolutePath output, GlobalJson globalJson) =>
        DownloadShieldsIo(output / "dotnet_version_badge.svg", ".NET", globalJson.Sdk.Version.ToString(), "blue");

    public static void ForTests(AbsolutePath output, string resultName)
    {
        var (passed, failed, skipped) =
            (NukeBuild.RootDirectory / "tests" / "**" / resultName)
            .GlobFiles()
            .Select(ExtractResults)
            .Aggregate((a, b) => a + b);

        var color =
            (passed, failed, skipped) switch
            {
                (_, > 0, _) => "critical",
                (_, _, > 10) => "orange",
                (0, 0, _) => "yellow",
                _ => "success"
            };

        List<string> messageBuilder = new();
        if (passed > 0) messageBuilder.Add($"{passed} passed");
        if (failed > 0) messageBuilder.Add($"{failed} failed");
        if (skipped > 0) messageBuilder.Add($"{skipped} skipped");
        var message = string.Join(",", messageBuilder);

        DownloadShieldsIo(output / "test_report_badge.svg", "tests", message, color);
    }

    static void DownloadShieldsIo(AbsolutePath fileName, string label, string message, string color)
    {
        EnsureExistingParentDirectory(fileName.Parent);
        var url = "https://img.shields.io/badge/" + Uri.EscapeDataString($"{label}-{message}-{color}");
        HttpTasks.HttpDownloadFile(url, fileName);
    }

    static TestSummary ExtractResults(AbsolutePath testResult)
    {
        var counters =
            XDocument.Load(testResult)
                .XPathSelectElement("//*[local-name() = 'ResultSummary']")
                ?.XPathSelectElement("//*[local-name() = 'Counters']");

        var value = (string name) =>
            counters is not null && int.TryParse(counters.Attribute(name)?.Value, out var n) ? n : default;

        return new(value("passed"), value("failed"), value("total") - value("executed"));
    }

    record TestSummary(int Passed, int Failed, int Skipped)
    {
        public static TestSummary operator +(TestSummary s1, TestSummary s2)
            => new(s1.Passed + s2.Passed, s1.Failed + s2.Failed, s1.Skipped + s2.Skipped);
    }
}