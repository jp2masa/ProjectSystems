using System.Threading.Tasks;

namespace Bootable.Launch
{
    public interface IDebugConnectorProvider
    {
        DebugMode DebugMode { get; }

        Task<IDebugConnector> CreateDebugConnectorAsync();
    }
}
