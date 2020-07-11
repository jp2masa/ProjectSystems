using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Bootable.Launch.DebugConnectors.Pipe
{
    internal class PipeClientDebugConnector : PipeDebugConnectorBase
    {
        protected override PipeStream Stream => _stream;

        private NamedPipeClientStream _stream;

        public PipeClientDebugConnector(string pipeName)
        {
            _stream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.WriteThrough);
        }

        public override Task ConnectAsync(CancellationToken cancellationToken) => _stream.ConnectAsync(cancellationToken);
        
        //protected override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        //{
        //    if (!_stream.IsConnected)
        //    {
        //        throw new InvalidOperationException();
        //    }
            
        //    return _stream.ReadAsync(buffer, offset, count, cancellationToken);
        //}
    }
}
