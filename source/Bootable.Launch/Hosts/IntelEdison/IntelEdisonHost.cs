using System;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.IntelEdison
{
    internal class IntelEdisonHost : IHost
    {
        public event EventHandler ShutDown;

        public Task StartAsync() => Task.CompletedTask;

        public Task KillAsync()
        {
            ShutDown?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }
    }
}
