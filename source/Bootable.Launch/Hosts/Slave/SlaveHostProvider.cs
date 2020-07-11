using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.Slave
{
    [ExportHostProvider]
    internal class SlaveHostProvider : IHostProvider
    {
        public string Name => "Slave";

        public bool IsDebugModeSupported(DebugMode debugMode) => debugMode == DebugMode.Serial;

        public Task<IHost> CreateHostAsync(IReadOnlyDictionary<string, string> settings, DebugMode debugMode = null)
        {
            var hostSettings = new SlaveHostSettings(settings);
            return Task.FromResult<IHost>(new SlaveHost(hostSettings));
        }
    }
}
