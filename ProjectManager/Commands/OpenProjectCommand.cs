using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace ProjectManager.Commands;

class Resource
{
    public required string Name { get; set; }
    public required bool IsRemote { get; set; }

    public override string ToString()
    {
        if (IsRemote)
            return $"--remote {Name}";
        return "";
    }
}

partial class OpenProjectCommand : InvokableCommand
{
    public Resource Resource { get; set; }
    public string Path { get; set; }

    private static readonly Regex remoteRegex = new Regex(
        @"^vscode-remote://(.*?)(/.*)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public OpenProjectCommand(Project project)
    {
        Match match = remoteRegex.Match(project.RootPath);
        (Resource, Path, Icon) = remoteRegex.Match(project.RootPath) switch
        {
            { Success: true } m => (
                new Resource { Name = m.Groups[1].Value, IsRemote = true },
                m.Groups[2].Value,
                IconHelpers.FromRelativePaths(
                    "Assets\\light\\remote.svg",
                    "Assets\\dark\\remote.svg"
                )
            ),
            _ => (
                new Resource { Name = "local", IsRemote = false },
                project.RootPath,
                IconHelpers.FromRelativePaths(
                    "Assets\\light\\folder.svg",
                    "Assets\\dark\\folder.svg"
                )
            ),
        };
    }

    public OpenProjectCommand(Project2 project)
    {
        Match match = remoteRegex.Match(project.FullPath);
        (Resource, Path, Icon) = remoteRegex.Match(project.FullPath) switch
        {
            { Success: true } m => (
                new Resource { Name = m.Groups[1].Value, IsRemote = true },
                m.Groups[2].Value,
                IconHelpers.FromRelativePaths(
                    "Assets\\light\\remote.svg",
                    "Assets\\dark\\remote.svg"
                )
            ),
            _ => (
                new Resource { Name = "local", IsRemote = false },
                project.FullPath,
                IconHelpers.FromRelativePaths(
                    "Assets\\light\\folder.svg",
                    "Assets\\dark\\folder.svg"
                )
            ),
        };
    }

    public override ICommandResult Invoke()
    {
        string command = $"code {this.Resource} {this.Path}";
        Process.Start(
            new ProcessStartInfo("powershell.exe")
            {
                Arguments = $"-NoProfile -ExecutionPolicy Bypass {command}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            }
        );

        return CommandResult.Hide();
    }
}
