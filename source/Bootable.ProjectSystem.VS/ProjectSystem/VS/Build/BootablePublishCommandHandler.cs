using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Build;

namespace Bootable.ProjectSystem.VS.Build
{
    [ExportCommandGroup(BootableProjectSystemPackage.BootableProjectSystemCommandSet)]
    [AppliesTo(ProjectCapability.Bootable)]
    internal class BootablePublishCommandHandler : IAsyncCommandGroupHandler
    {
        private ConfiguredProject _configuredProject;

        [ImportingConstructor]
        public BootablePublishCommandHandler(ConfiguredProject configuredProject)
        {
            _configuredProject = configuredProject;
        }
        
        public Task<CommandStatusResult> GetCommandStatusAsync(
            IImmutableSet<IProjectTree> nodes,
            long commandId,
            bool focused,
            string commandText,
            CommandStatus progressiveStatus)
        {
            var node = nodes.First();
            
            if (node.IsRoot()
                && commandId == BootableProjectSystemPackage.PublishBootableProjectContextMenuCmdId)
            {
                return Task.FromResult(
                    new CommandStatusResult(true, "Publish OS", CommandStatus.Supported | CommandStatus.Enabled));
            }
            else
            {
                return Task.FromResult(
                    new CommandStatusResult(false, commandText, progressiveStatus));
            }
        }

        public async Task<bool> TryHandleCommandAsync(
            IImmutableSet<IProjectTree> nodes,
            long commandId,
            bool focused,
            long commandExecuteOptions,
            IntPtr variantArgIn,
            IntPtr variantArgOut)
        {
            var publishProvider = new PublishProvider(_configuredProject);

            try
            {
                if (await publishProvider.ShowPublishPromptAsync().ConfigureAwait(false))
                {
                    await _configuredProject.Services.Build.BuildAsync(GetBuildAction(), CancellationToken.None, true).ConfigureAwait(false);
                    await publishProvider.PublishAsync(CancellationToken.None, null).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
            }

            return true;
        }

        private static IEnumerable<BuildAction> GetBuildAction()
        {
            yield return BuildAction.Build;
        }
    }
}
