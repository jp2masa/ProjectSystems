using System.Collections.Immutable;
using Microsoft.VisualStudio.ProjectSystem.Debug;

namespace Bootable.ProjectSystem.Debug
{
    public class BootableLaunchProfile : ILaunchProfile
    {
        public const string DebugModeProperty = "debugMode";
        public const string HostProperty = "host";
        public const string HostSettingsObject = "hostSettings";

        public string Name { get; }
        public string CommandName { get; }
        public string ExecutablePath { get; }
        public string CommandLineArgs { get; }
        public string WorkingDirectory { get; }
        public bool LaunchBrowser { get; }
#pragma warning disable CA1056 // Uri properties should not be strings
        public string LaunchUrl { get; }
#pragma warning restore CA1056 // Uri properties should not be strings
        public ImmutableDictionary<string, string> EnvironmentVariables { get; }
        public ImmutableDictionary<string, object> OtherSettings { get; }

        public string DebugMode { get; }
        public string HostProvider { get; }

        public BootableLaunchProfile(ILaunchProfile profile)
        {
            Name = profile.Name;
            CommandName = profile.CommandName;
            ExecutablePath = profile.ExecutablePath;
            CommandLineArgs = profile.CommandLineArgs;
            WorkingDirectory = profile.WorkingDirectory;
            LaunchBrowser = profile.LaunchBrowser;
            LaunchUrl = profile.LaunchUrl;
            EnvironmentVariables = profile.EnvironmentVariables;
            OtherSettings = profile.OtherSettings;

            if (OtherSettings.TryGetValue(DebugModeProperty, out var debugModeObj)
                && debugModeObj is string debugMode)
            {
                DebugMode = debugMode;
            }

            if (OtherSettings.TryGetValue(HostProperty, out var hostNameObj)
                && hostNameObj is string hostName)
            {
                HostProvider = hostName;
            }
        }
    }
}
