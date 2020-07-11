using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace Bootable.ProjectSystem.VS.Build
{
    internal sealed class UsbPublishSettingsViewModel : PublishSettingsViewModelBase, IDisposable
    {
        public DriveInfo SelectedDrive
        {
            get => _selectedDrive;
            set => SetAndRaiseIfChanged(ref _selectedDrive, value);
        }

        // todo: format usb drive should be true by default?
        public bool FormatDrive
        {
            get => _formatDrive;
            set => SetAndRaiseIfChanged(ref _formatDrive, value);
        }

        public IEnumerable<DriveInfo> Drives
        {
            get
            {
                var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable);

                if (!drives.Any(d => String.Equals(d.VolumeLabel, SelectedDrive?.VolumeLabel, StringComparison.OrdinalIgnoreCase)))
                {
                    SelectedDrive = drives.FirstOrDefault();
                }

                return drives;
            }
        }

        private ManagementEventWatcher _deviceInsertedWatcher;
        private ManagementEventWatcher _deviceRemovedWatcher;

        private DriveInfo _selectedDrive;
        private bool _formatDrive;

        public UsbPublishSettingsViewModel()
        {
            InitializeUsbDrivesWatcher();
        }

        private void InitializeUsbDrivesWatcher()
        {
            var deviceInsertedQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            _deviceInsertedWatcher = new ManagementEventWatcher(deviceInsertedQuery);
            _deviceInsertedWatcher.EventArrived += DrivesChanged;
            _deviceInsertedWatcher.Start();

            var deviceRemovedQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            _deviceRemovedWatcher = new ManagementEventWatcher(deviceRemovedQuery);
            _deviceRemovedWatcher.EventArrived += DrivesChanged;
            _deviceRemovedWatcher.Start();
        }

        private void DrivesChanged(object sender, EventArrivedEventArgs e) => OnPropertyChanged(nameof(Drives));

        public void Dispose()
        {
            _deviceInsertedWatcher.Dispose();
            _deviceRemovedWatcher.Dispose();
        }
    }
}
