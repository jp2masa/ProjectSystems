using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.ProjectSystem.Build;

namespace Bootable.ProjectSystem.VS.Build
{
    internal abstract class BuildTargetBootablePublishProviderBase : IBootablePublishProvider
    {
        public abstract string Name { get; }
        public abstract ImageMoniker Icon { get; }
        public abstract UserControl SettingsControl { get; }

        public abstract string TargetName { get; }

        private readonly IBuildSupport _buildSupport;
        private readonly IBuildProject _buildProject;

        protected BuildTargetBootablePublishProviderBase(
            IBuildSupport buildSupport,
            IBuildProject buildProject)
        {
            _buildSupport = buildSupport;
            _buildProject = buildProject;
        }

        public abstract Task<ImmutableDictionary<string, string>> GetPropertiesAsync(CancellationToken cancellationToken);

        public Task<bool> CanPublishAsync(CancellationToken cancellationToken) =>
            _buildSupport.IsBuildTargetSupportedAsync(TargetName, cancellationToken);

        public async Task PublishAsync(TextWriter outputPaneWriter, CancellationToken cancellationToken)
        {
            var properties = await GetPropertiesAsync(cancellationToken).ConfigureAwait(false);

            await _buildProject.BuildAsync(
                ImmutableArray.Create(TargetName),
                cancellationToken,
                true,
                properties).ConfigureAwait(false);
        }
    }
}
