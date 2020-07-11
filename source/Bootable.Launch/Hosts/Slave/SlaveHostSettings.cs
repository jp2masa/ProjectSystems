using System.Collections.Generic;

namespace Bootable.Launch.Hosts.Slave
{
    internal class SlaveHostSettings : HostSettingsBase
    {
        public string PortName { get; set; }

        public SlaveHostSettings(IReadOnlyDictionary<string, string> settings)
            : base(settings)
        {
            PortName = GetProperty<string>(nameof(PortName));
        }
    }
}
