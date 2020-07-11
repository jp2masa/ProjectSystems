using System;
using System.Threading.Tasks;

namespace Bootable.Launch
{
    public interface IHost
    {
        event EventHandler ShutDown;

        Task StartAsync();
        Task KillAsync();
    }
}
