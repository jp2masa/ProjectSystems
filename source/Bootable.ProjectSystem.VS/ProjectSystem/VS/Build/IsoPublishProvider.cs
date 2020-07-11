using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Build;

namespace Bootable.ProjectSystem.VS.Build
{
    [Export(typeof(IBootablePublishProvider))]
    [AppliesTo(ProjectCapability.Bootable)]
    internal class IsoPublishProvider : BuildTargetBootablePublishProviderBase
    {
        private const string IsoPublishTargetName = "IsoPublish";

        private IsoPublishSettingsControl _settingsControl;
        private IsoPublishSettingsViewModel _viewModel;

        [ImportingConstructor]
        public IsoPublishProvider(
            IBuildSupport buildSupport,
            IBuildProject buildProject,
            IProjectThreadingService projectThreadingService,
            IBootableProperties bootableProperties)
            : base (buildSupport, buildProject)
        {
            _viewModel = new IsoPublishSettingsViewModel();
            _viewModel.PublishPath = projectThreadingService.ExecuteSynchronously(bootableProperties.GetIsoFileFullPathAsync);

            _settingsControl = new IsoPublishSettingsControl();
            _settingsControl.DataContext = _viewModel;
        }

        public override string Name => "ISO";
        public override ImageMoniker Icon => BootableImagesMonikers.IsoPublishProviderIcon;
        public override UserControl SettingsControl => _settingsControl;

        public override string TargetName => IsoPublishTargetName;

        public override Task<ImmutableDictionary<string, string>> GetPropertiesAsync(
            CancellationToken cancellationToken)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            builder.Add("IsoPublishOutputPath", _viewModel.PublishPath);

            return Task.FromResult(builder.ToImmutableDictionary());
        }
    }
}
