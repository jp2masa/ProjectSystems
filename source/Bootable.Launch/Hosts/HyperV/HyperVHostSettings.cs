using System.Collections.Generic;

namespace Bootable.Launch.Hosts.HyperV
{
    internal class HyperVHostSettings : HostSettingsBase
    {
        public HyperVHostSettings(IReadOnlyDictionary<string, string> settings)
            : base(settings)
        {
            IsoFile = GetProperty<string>(nameof(IsoFile));
            HardDiskFile = GetProperty<string>(nameof(HardDiskFile));
        }

        public string IsoFile { get; set; }
        public string HardDiskFile { get; set; }
    }
}
