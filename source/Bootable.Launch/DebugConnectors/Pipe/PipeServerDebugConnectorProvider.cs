using System.Threading.Tasks;

namespace Bootable.Launch.DebugConnectors.Pipe
{
    [ExportDebugConnectorProvider]
    internal class PipeServerDebugConnectorProvider : IDebugConnectorProvider
    {
        public DebugMode DebugMode => DebugMode.PipeServer;

        public Task<IDebugConnector> CreateDebugConnectorAsync()
        {
            return Task.FromResult<IDebugConnector>(new PipeServerDebugConnector(""));
        }
    }
}
