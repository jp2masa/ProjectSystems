using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Bootable.Launch.DebugConnectors.Pipe
{
    internal abstract class PipeDebugConnectorBase : IDebugConnector
    {
        public bool IsConnected => Stream?.IsConnected ?? false;

        protected abstract PipeStream Stream { get; }

        public abstract Task ConnectAsync(CancellationToken cancellationToken);
    }
}
