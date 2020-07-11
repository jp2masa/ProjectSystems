using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debug;
using Microsoft.VisualStudio.ProjectSystem.VS.Debug;

using static XSharp.ProjectSystem.ConfigurationGeneral;

namespace XSharp.ProjectSystem.VS.Debug
{
    [Export(typeof(IDebugProfileLaunchTargetsProvider))]
    [AppliesTo(ProjectCapability.XSharp)]
    [Order(Order.OverrideManaged)]
    internal class XSharpLaunchTargetsProvider : IDebugProfileLaunchTargetsProvider
    {
        private readonly ConfiguredProject _configuredProject;
        private readonly ProjectProperties _projectProperties;

        [ImportingConstructor]
        public XSharpLaunchTargetsProvider(
            ConfiguredProject configuredProject,
            ProjectProperties projectProperties)
        {
            _configuredProject = configuredProject;
            _projectProperties = projectProperties;
        }

        public Task OnAfterLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile) => Task.CompletedTask;
        public Task OnBeforeLaunchAsync(DebugLaunchOptions launchOptions, ILaunchProfile profile) => Task.CompletedTask;

        public async Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(
            DebugLaunchOptions launchOptions,
            ILaunchProfile profile)
        {
            var projectProperties = await _projectProperties.GetConfigurationGeneralPropertiesAsync().ConfigureAwait(false);
            var outputType = await projectProperties.OutputType.GetEvaluatedValueAtEndAsync().ConfigureAwait(false);
            var isBootable = String.Equals(outputType, OutputTypeValues.Bootable, StringComparison.OrdinalIgnoreCase);

            if (!String.Equals(outputType, OutputTypeValues.Application, StringComparison.OrdinalIgnoreCase) && !isBootable)
            {
                throw new Exception($"Project cannot be launched! Output type: '{outputType}'.");
            }

            if (isBootable)
            {
                // todo: using debugger for this would be better
                await _configuredProject.Services.Build.BuildAsync(
                    ImmutableArray.Create("Run"), CancellationToken.None, true).ConfigureAwait(false);
            }

            if (!launchOptions.HasFlag(DebugLaunchOptions.NoDebug))
            {
                var xBinaryOutput = await projectProperties.BinaryOutput.GetEvaluatedValueAtEndAsync().ConfigureAwait(false);
                xBinaryOutput = Path.GetFullPath(xBinaryOutput);

                var xDebugSettings = new DebugLaunchSettings(launchOptions)
                {
                    LaunchOperation = DebugLaunchOperation.AlreadyRunning,
                    CurrentDirectory = Path.GetDirectoryName(xBinaryOutput)
                };

                if (isBootable)
                {
                    // todo: implement
                    //xDebugSettings.LaunchDebugEngineGuid = XSharpDebuggerGuid;

                    return Array.Empty<DebugLaunchSettings>();
                }
                else
                {
                    xDebugSettings.Executable = xBinaryOutput;
                }

                return ImmutableArray.Create(xDebugSettings);
            }

            return Array.Empty<DebugLaunchSettings>();
        }

        public bool SupportsProfile(ILaunchProfile profile)
        {
            return true;
        }
    }
}
