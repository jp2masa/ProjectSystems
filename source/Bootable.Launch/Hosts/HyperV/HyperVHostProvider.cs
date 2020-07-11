using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.HyperV
{
    [ExportHostProvider]
    internal class HyperVHostProvider : IHostProvider
    {
        public string Name { get; } = "Hyper-V";

        public bool IsHostSupported() => RuntimeHelper.IsWindows;

        public bool IsDebugModeSupported(DebugMode debugMode) =>
            debugMode == DebugMode.None || debugMode == DebugMode.PipeServer;

        public Task<IHost> CreateHostAsync(IReadOnlyDictionary<string, string> settings, DebugMode debugMode) =>
            Task.FromResult<IHost>(new HyperVHost(new HyperVHostSettings(settings)));
    }
}
