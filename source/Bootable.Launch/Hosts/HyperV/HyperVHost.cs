using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.HyperV
{
    internal class HyperVHost : IHost
    {
        private const string VmName = "Cosmos";

        private HyperVHostSettings _settings;

        private Process _process;
        
        private static bool IsProcessAdministrator => (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);

        public event EventHandler ShutDown;

        public HyperVHost(HyperVHostSettings settings)
        {
            _settings = settings;
            
            if (!IsProcessAdministrator)
            {
                throw new Exception("Visual Studio must be run as administrator for Hyper-V to work");
            }
        }
        
        public Task StartAsync()
        {
            CreateVirtualMachine();

            var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var vmConnectPath = Path.Combine(windowsPath, "sysnative", "VmConnect.exe");

            var processStartInfo = new ProcessStartInfo(vmConnectPath, @"""localhost"" ""Cosmos""");

            _process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            _process.Exited += (sender, args) => ShutDown?.Invoke(this, EventArgs.Empty);

            _process.Start();

            RunPowerShellScript("Start-VM -Name Cosmos");

            return Task.CompletedTask;
        }
        
        public Task KillAsync()
        {
            RunPowerShellScript("Stop-VM -Name Cosmos -TurnOff -ErrorAction Ignore");

            try
            {
                if (_process != null)
                {
                    _process.Kill();
                    return Task.Run((Action)_process.WaitForExit);
                }
            }
            catch (InvalidOperationException)
            {
            }

            return Task.CompletedTask;
        }
        
        private void CreateVirtualMachine()
        {
            RunPowerShellScript("Stop-VM -Name Cosmos -TurnOff -ErrorAction Ignore");

            RunPowerShellScript("Remove-VM -Name Cosmos -Force -ErrorAction Ignore");
            RunPowerShellScript("New-VM -Name Cosmos -MemoryStartupBytes 268435456 -BootDevice CD");

            RunPowerShellScript($@"Add-VMHardDiskDrive -VMName Cosmos -ControllerNumber 0 -ControllerLocation 0 -Path ""{_settings.HardDiskFile}""");
            RunPowerShellScript($@"Set-VMDvdDrive -VMName Cosmos -ControllerNumber 1 -ControllerLocation 0 -Path ""{_settings.IsoFile}""");
            RunPowerShellScript(@"Set-VMComPort -VMName Cosmos -Path \\.\pipe\CosmosSerial -Number 1");
        }

        private static void RunPowerShellScript(string text)
        {
            //using (Runspace runspace = RunspaceFactory.CreateRunspace())
            //{
            //    runspace.Open();

            //    var pipeline = runspace.CreatePipeline();

            //    pipeline.Commands.AddScript(text);
            //    pipeline.Commands.Add("Out-String");

            //    var results = pipeline.Invoke();

            //    foreach (var obj in results)
            //    {
            //        Debug.WriteLine(obj.ToString());
            //    }
            //}
        }
    }
}
