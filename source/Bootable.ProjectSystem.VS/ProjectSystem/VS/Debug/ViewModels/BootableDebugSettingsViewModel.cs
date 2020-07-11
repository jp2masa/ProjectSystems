using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.ProjectSystem.Debug;

using Bootable.ProjectSystem.ProjectSystem.VS.Debug.Models;
using static Bootable.ProjectSystem.Debug.BootableLaunchProfile;
using System.Linq;

namespace Bootable.ProjectSystem.VS.Debug
{
    internal sealed class BootableDebugSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _selectedHost;
        private ObservableCollection<Property> _hostSettings;
        
        private IWritableLaunchSettings _launchSettings;

        public string SelectedHost
        {
            get => _selectedHost;
            set => SetAndRaiseIfChanged(ref _selectedHost, value);
        }

        public ObservableCollection<Property> HostSettings
        {
            get => _hostSettings;
            set => SetAndRaiseIfChanged(ref _hostSettings, value);
        }

        public IEnumerable<string> Hosts { get; } = new List<string>()
        {
            "Bochs",
            "Hyper-V",
            "VMware",
            "PXE"
        };

        public void SetLaunchSettings(IWritableLaunchSettings launchSettings)
        {
            _launchSettings = launchSettings;

            var hostSettingsData = GetProperty<Dictionary<string, object>>(HostSettingsObject);

            _selectedHost = GetProperty<string>(HostProperty);
            _hostSettings = new ObservableCollection<Property>(
                hostSettingsData.Select(s => new Property() { Name = s.Key, Value = s.Value?.ToString() }));

            OnPropertyChanged(String.Empty);
        }

        private T GetProperty<T>(string propertyName)
        {
            var activeProfile = _launchSettings?.ActiveProfile;

            if (activeProfile != null && activeProfile.OtherSettings.TryGetValue(propertyName, out var valueObject))
            {
                if (valueObject is T value)
                {
                    return value;
                }
            }

            return default(T);
        }

        private void SetProperty<T>(string propertyName, T value)
        {
            var activeProfile = _launchSettings?.ActiveProfile;

            if (activeProfile?.OtherSettings != null)
            {
                activeProfile.OtherSettings[propertyName] = value;
            }
        }

        private void SetAndRaiseIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static bool StringEqualsIgnoreCase(string a, string b) =>
            String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
