using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debug;
using Microsoft.VisualStudio.ProjectSystem.VS.Debug;

namespace Bootable.ProjectSystem.VS.Debug
{
    [Export(typeof(IDebugProfileLaunchTargetsProvider))]
    [AppliesTo(ProjectCapability.BootableAndLaunchProfiles)]
    [Order(Order.Highest)]
    internal class MIEngineLaunchTargetsProvider : IDebugProfileLaunchTargetsProvider
    {
        private static readonly Guid MIEngineGuid = new Guid("ea6637c6-17df-45b5-a183-0951c54243bc");

        private readonly UnconfiguredProject _unconfiguredProject;

        // this has to be exported to the configured project scope
        [ImportingConstructor]
        public MIEngineLaunchTargetsProvider(
#pragma warning disable CA1801
            ConfiguredProject configuredProject,
#pragma warning restore CA1801
            UnconfiguredProject unconfiguredProject)
        {
            _unconfiguredProject = unconfiguredProject;
        }

        public Task OnBeforeLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile) => Task.CompletedTask;

        public Task OnAfterLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile) => Task.CompletedTask;

        public Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(
            DebugLaunchOptions launchOptions,
            ILaunchProfile profile)
        {
            var debugLaunchSettings = new DebugLaunchSettings(launchOptions)
            {
                //Arguments = miDebugLaunchSettings,
                LaunchDebugEngineGuid = MIEngineGuid
            };

            return Task.FromResult<IReadOnlyList<IDebugLaunchSettings>>(
                ImmutableList.Create(debugLaunchSettings));
        }

        public bool SupportsProfile(ILaunchProfile profile)
        {
            return String.Equals(profile.CommandName, "Bootable", StringComparison.OrdinalIgnoreCase)
                && profile.OtherSettings.TryGetValue("useMIEngine", out var useMIEngineObject)
                && useMIEngineObject is bool useMIEngine
                && useMIEngine;
        }
    }
}
