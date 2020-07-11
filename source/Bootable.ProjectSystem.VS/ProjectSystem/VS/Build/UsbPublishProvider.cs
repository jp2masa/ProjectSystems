using System;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Globalization;
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
    internal sealed class UsbPublishProvider : BuildTargetBootablePublishProviderBase, IDisposable
    {
        private const string UsbPublishTargetName = "UsbPublish";

        private UsbPublishSettingsControl _settingsControl;
        private UsbPublishSettingsViewModel _viewModel;

        public UsbPublishProvider(
            IBuildSupport buildSupport,
            IBuildProject buildProject)
            : base(buildSupport, buildProject)
        {
            _viewModel = new UsbPublishSettingsViewModel();

            _settingsControl = new UsbPublishSettingsControl();
            _settingsControl.DataContext = _viewModel;
        }

        public override string Name => "USB";
        public override ImageMoniker Icon => BootableImagesMonikers.UsbPublishProviderIcon;
        public override UserControl SettingsControl => _settingsControl;

        public override string TargetName => UsbPublishTargetName;

        public override Task<ImmutableDictionary<string, string>> GetPropertiesAsync(
            CancellationToken cancellationToken)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            builder.Add("UsbPublishDrive", _viewModel.SelectedDrive.Name);
            builder.Add("UsbPublishFormatDrive", _viewModel.FormatDrive.ToString(CultureInfo.InvariantCulture));

            return Task.FromResult(builder.ToImmutableDictionary());
        }

        public void Dispose() => _viewModel.Dispose();
    }
}
