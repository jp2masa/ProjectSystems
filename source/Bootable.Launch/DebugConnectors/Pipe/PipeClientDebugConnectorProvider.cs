using System.Threading.Tasks;

namespace Bootable.Launch.DebugConnectors.Pipe
{
    [ExportDebugConnectorProvider]
    internal class PipeClientDebugConnectorProvider : IDebugConnectorProvider
    {
        public DebugMode DebugMode => DebugMode.PipeClient;

        public Task<IDebugConnector> CreateDebugConnectorAsync()
        {
            return Task.FromResult<IDebugConnector>(new PipeClientDebugConnector(""));
        }
    }
}
