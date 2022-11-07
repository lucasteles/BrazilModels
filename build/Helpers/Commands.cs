namespace Helpers;

public static class Commands
{
    public static readonly Tool Git = GetTool("git");

    static readonly AbsolutePath Dir = NukeBuild.RootDirectory;

    public static Tool BrowseHtml => GetTool(
        Platform switch
        {
            PlatformFamily.Windows => "explorer",
            PlatformFamily.OSX => "open",
            _ => new[] { "google-chrome", "firefox" }
                .FirstOrDefault(CommandExists)
        });

    public static Tool GetTool(string name) =>
        ToolResolver.TryGetEnvironmentTool(name) ??
        ToolResolver.GetPathTool(name);

    public static IProcess RunCommand(string command, params string[] args) =>
        ProcessTasks.StartProcess(command,
            string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a.Trim()}\"" : a.Trim())),
            Dir);

    public static bool CommandExists(string command)
    {
        var process = RunCommand("which", command);
        process.WaitForExit();
        return process.ExitCode == 0;
    }

    public static void DotnetGlobalToolEnsureInstalled(string packageName)
    {
        var hasPackageInstalled = DotNet("tool list --global", logOutput: false)
            .Any(output => output.Text.Contains(packageName));

        if (!hasPackageInstalled)
            return;

        DotNetToolInstall(s => s
            .EnableGlobal()
            .SetPackageName(packageName));
    }
}