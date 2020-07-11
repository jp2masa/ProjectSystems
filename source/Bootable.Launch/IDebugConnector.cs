using System.Threading;
using System.Threading.Tasks;

namespace Bootable.Launch
{
    public interface IDebugConnector
    {
        bool IsConnected { get; }

        Task ConnectAsync(CancellationToken cancellationToken);
    }
}
