using System.Collections.Generic;

namespace Bootable.Launch.Hosts.VMware
{
    internal class VMwareHostSettings : HostSettingsBase
    {
        public string VMwareExecutable { get; }

        public string ConfigurationFile { get; }
        public bool OverwriteConfigurationFile { get; }

        public string IsoFile { get; }
        public string HardDiskFile { get; }

        public bool UseGDB { get; }

        public string PipeServerName { get; }

        public VMwareHostSettings(IReadOnlyDictionary<string, string> settings)
            : base(settings)
        {
            VMwareExecutable = GetProperty<string>(nameof(VMwareExecutable));

            ConfigurationFile = GetProperty<string>(nameof(ConfigurationFile));
            OverwriteConfigurationFile = GetProperty<bool>(nameof(OverwriteConfigurationFile));

            IsoFile = GetProperty<string>(nameof(IsoFile));
            HardDiskFile = GetProperty<string>(nameof(HardDiskFile));

            UseGDB = GetProperty<bool>(nameof(UseGDB));

            PipeServerName = GetProperty<string>(nameof(PipeServerName));
        }
    }
}
