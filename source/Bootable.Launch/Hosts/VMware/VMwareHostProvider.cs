using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.VMware
{
    [ExportHostProvider]
    internal class VMwareHostProvider : IHostProvider
    {
        public string Name => "VMware";

        public bool IsDebugModeSupported(DebugMode debugMode) =>
            debugMode == DebugMode.None || debugMode == DebugMode.PipeClient || debugMode == DebugMode.PipeServer;

        public Task<IHost> CreateHostAsync(IReadOnlyDictionary<string, string> settings, DebugMode debugMode = null)
        {
            var hostSettings = new VMwareHostSettings(settings);
            return Task.FromResult<IHost>(new VMwareHost(hostSettings));
        }
    }
}
