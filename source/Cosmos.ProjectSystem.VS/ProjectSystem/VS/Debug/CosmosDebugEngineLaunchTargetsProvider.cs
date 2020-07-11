using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debug;
using Microsoft.VisualStudio.ProjectSystem.VS.Debug;

using Bootable.ProjectSystem;
using Bootable.ProjectSystem.Debug;
using System.Linq;

namespace Cosmos.ProjectSystem.VS.Debug
{
    [Export(typeof(IDebugProfileLaunchTargetsProvider))]
    [AppliesTo(ProjectCapability.Cosmos)]
    [Order(Order.OverrideManaged)]
    internal class CosmosDebugEngineLaunchTargetsProvider : IDebugProfileLaunchTargetsProvider
    {
        private static readonly Guid CosmosDebugEngineGuid = new Guid("fa1da3a6-66ff-4c65-b077-e65f7164ef83");

        private ConfiguredProject _configuredProject;
        private UnconfiguredProject _unconfiguredProject;
        private ProjectProperties _projectProperties;
        private IBootableProperties _bootableProperties;

        // this has to be exported to the configured project scope
        [ImportingConstructor]
        public CosmosDebugEngineLaunchTargetsProvider(
            ConfiguredProject configuredProject,
            UnconfiguredProject unconfiguredProject,
            ProjectProperties projectProperties,
            IBootableProperties bootableProperties)
        {
            _configuredProject = configuredProject;
            _unconfiguredProject = unconfiguredProject;
            _projectProperties = projectProperties;
            _bootableProperties = bootableProperties;
        }

        public Task OnAfterLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile) => Task.CompletedTask;
        public Task OnBeforeLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile) => Task.CompletedTask;

        public async Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(
            DebugLaunchOptions launchOptions,
            ILaunchProfile profile)
        {
            var projectPath = _unconfiguredProject.FullPath;
            var projectDirectory = Path.GetDirectoryName(projectPath);

            var bootableLaunchProfile = new BootableLaunchProfile(profile);
            var hostProvider = bootableLaunchProfile.HostProvider;

            if (String.IsNullOrWhiteSpace(hostProvider))
            {
                throw new InvalidOperationException("Host is null or empty!");
            }

            var debugLaunchSettings = new DebugLaunchSettings(launchOptions)
            {
                LaunchOperation = DebugLaunchOperation.CreateProcess,
                LaunchDebugEngineGuid = CosmosDebugEngineGuid,
                CurrentDirectory = Path.GetDirectoryName(projectPath)
            };

            var values = new Dictionary<string, string>
            {
                ["ProjectFile"] = projectPath,
                ["ISOFile"] = await _bootableProperties.GetIsoFileFullPathAsync().ConfigureAwait(false),
                ["OutputPath"] = await GetPropertyAsync("OutputPath").ConfigureAwait(false),
                ["Launch"] = hostProvider
            };

            foreach (var propertyName in BuildPropertyNames)
            {
                values[propertyName] = await GetPropertyAsync(propertyName).ConfigureAwait(false);
            }

            debugLaunchSettings.Executable = String.Join(";", values.Select(v => $"{v.Key}={v.Value}"));

            return ImmutableArray.Create(debugLaunchSettings);
        }

        public bool SupportsProfile(ILaunchProfile profile) => StringEqualsIgnoreCase(profile.CommandName, "Bootable");

        private Task<string> GetPropertyAsync(string propertyName)
        {
            var properties = _configuredProject.Services.ProjectPropertiesProvider.GetCommonProperties();
            return properties.GetEvaluatedPropertyValueAsync(propertyName);
        }

        private static bool StringEqualsIgnoreCase(string a, string b) =>
            String.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        // todo: remove this when possible
        private static List<string> BuildPropertyNames = new List<string>()
        {
            "StackCorruptionDetectionEnabled",
            "StackCorruptionDetectionLevel",
            "Profile",
            "Name",
            "Description",
            "Deployment",
            "ShowLaunchConsole",
            "DebugEnabled",
            "DebugMode",
            "IgnoreDebugStubAttribute",
            "CosmosDebugPort",
            "VisualStudioDebugPort",
            "PxeInterface",
            "SlavePort",
            "VMwareEdition",
            "Framework",
            "UseInternalAssembler",
            "TraceAssemblies",
            "EnableGDB",
            "StartCosmosGDB",
            "EnableBochsDebug",
            "StartBochsDebugGui",
            "BinFormat"
        };
    }
}
