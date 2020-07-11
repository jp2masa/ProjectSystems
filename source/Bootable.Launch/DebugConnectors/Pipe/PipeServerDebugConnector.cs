using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Bootable.Launch.DebugConnectors.Pipe
{
    internal class PipeServerDebugConnector : PipeDebugConnectorBase
    {
        protected override PipeStream Stream => _stream;

        private NamedPipeServerStream _stream;

        public PipeServerDebugConnector(string pipeName)
        {
            _stream = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
        }

        public override Task ConnectAsync(CancellationToken cancellationToken) => _stream.WaitForConnectionAsync();
    }
}
