using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.Bochs
{
    [ExportHostProvider]
    internal class BochsHostProvider : IHostProvider
    {
        public string Name => "Bochs";

        public bool IsHostSupported() => true;

        public bool IsDebugModeSupported(DebugMode debugMode) =>
            debugMode == DebugMode.None || debugMode == DebugMode.PipeClient || debugMode == DebugMode.PipeServer;

        public Task<IHost> CreateHostAsync(IReadOnlyDictionary<string, string> settings, DebugMode debugMode)
        {
            var hostSettings = new BochsHostSettings(settings);
            return Task.FromResult<IHost>(new BochsHost(hostSettings));
        }
    }
}
