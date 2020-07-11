using System.Threading.Tasks;

namespace Bootable.Launch.DebugConnectors.Serial
{
    [ExportDebugConnectorProvider]
    internal class SerialDebugConnectorProvider : IDebugConnectorProvider
    {
        public DebugMode DebugMode => DebugMode.Serial;

        public Task<IDebugConnector> CreateDebugConnectorAsync() => Task.FromResult<IDebugConnector>(null);
    }
}
