using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debug;

namespace Bootable.ProjectSystem.VS.Debug
{
    [Export(typeof(ILaunchSettingsUIProvider))]
    [AppliesTo(ProjectCapability.BootableAndLaunchProfiles)]
    [Order(Order.Highest)]
    internal class BootableDebugUIProvider : ILaunchSettingsUIProvider
    {
        private BootableDebugSettingsControl _bootableDebugSettingsControl;
        private BootableDebugSettingsViewModel _bootableDebugSettingsViewModel;

        // this has to be exported to the unconfigured project scope
        [ImportingConstructor]
        public BootableDebugUIProvider(
#pragma warning disable CA1801
            UnconfiguredProject unconfiguredProject)
#pragma warning restore CA1801
        {
            _bootableDebugSettingsViewModel = new BootableDebugSettingsViewModel();

            _bootableDebugSettingsControl = new BootableDebugSettingsControl();
            _bootableDebugSettingsControl.DataContext = _bootableDebugSettingsViewModel;
        }

        public string CommandName => "Bootable";
        public string FriendlyName => "Bootable";
        public UserControl CustomUI => _bootableDebugSettingsControl;

        public void ProfileSelected(IWritableLaunchSettings curSettings) =>
            _bootableDebugSettingsViewModel.SetLaunchSettings(curSettings);

        public bool ShouldEnableProperty(string propertyName) => false;
    }
}
