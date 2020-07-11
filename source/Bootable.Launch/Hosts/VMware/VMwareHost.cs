using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Bootable.Launch.Hosts.VMware
{
    internal class VMwareHost : IHost
    {
        private const string VMwareConfigurationFile = "VMware.vmx";

        private VMwareHostSettings _launchSettings;

        private string _vmwareExe;

        private Process _process;

        public event EventHandler ShutDown;

        public VMwareHost(VMwareHostSettings launchSettings)
        {
            if (!(RuntimeHelper.IsWindows || RuntimeHelper.IsLinux))
            {
                throw new PlatformNotSupportedException();
            }

            _launchSettings = launchSettings;

            _vmwareExe = _launchSettings.VMwareExecutable;

            if (!File.Exists(_vmwareExe) && RuntimeHelper.IsWindows)
            {
                _vmwareExe = GetPathname("VMware Workstation", "vmware.exe")
                    ?? GetPathname("VMware Player", "vmplayer.exe");
            }

            if (!File.Exists(_vmwareExe))
            {
                throw new InvalidOperationException("VMware exe not found!");
            }
        }

        public Task StartAsync()
        {
            if (_launchSettings.OverwriteConfigurationFile || !File.Exists(_launchSettings.ConfigurationFile))
            {
                Cleanup();
                CreateVmx();
            }

            // Target exe or file
            _process = new Process();

            var vmwareStartInfo = _process.StartInfo;

            vmwareStartInfo.FileName = _launchSettings.VMwareExecutable;

            string vmxPath = "\"" + _launchSettings.ConfigurationFile + "\"";
            //if (mEdition == VMwareEdition.Player)
            {
                vmwareStartInfo.Arguments = vmxPath;
            }
            //else
            //{
            //    // -x: Auto power on VM. Must be small x, big X means something else.
            //    // -q: Close VMware when VM is powered off.
            //    // Options must come beore the vmx, and cannot use shellexecute
            //    xPSI.Arguments = "-x -q " + xVmxPath;
            //}
            vmwareStartInfo.UseShellExecute = false;  //must be true to allow elevate the process, sometimes needed if vmware only runs with admin rights
            _process.EnableRaisingEvents = true;

            _process.Exited += delegate
            {
                ShutDown?.Invoke(this, EventArgs.Empty);
            };

            _process.Start();

            return Task.CompletedTask;
        }

        public Task KillAsync()
        {
            try
            {
                if (_process != null)
                {
                    _process.Kill();
                    Cleanup();

                    return Task.Run((Action)_process.WaitForExit);
                }
            }
            catch (InvalidOperationException)
            {
            }

            Cleanup();
            return Task.CompletedTask;
        }

        private static void DeleteFiles(string path, string pattern)
        {
            foreach (var file in Directory.GetFiles(path, pattern))
            {
                File.Delete(file);
            }
        }

        private void Cleanup()
        {
            try
            {
                string xPath = Path.GetDirectoryName(_launchSettings.ConfigurationFile);
                // Delete the auto snapshots that latest vmware players create as default.
                // It creates them with suffixes though, so we need to wild card find them.
                DeleteFiles(xPath, "*.vmxf");
                DeleteFiles(xPath, "*.vmss");
                DeleteFiles(xPath, "*.vmsd");
                DeleteFiles(xPath, "*.vmem");
                // Delete log files so that logged data is only from last boot
                File.Delete(Path.Combine(xPath, "vmware.log"));
                File.Delete(Path.Combine(xPath, "vmware-0.log"));
                File.Delete(Path.Combine(xPath, "vmware-1.log"));
                File.Delete(Path.Combine(xPath, "vmware-2.log"));
            }
            catch (Exception)
            {
                // Ignore errors, users can stop VS while VMware is still running and files will be locked.
            }
        }

        private void CreateVmx()
        {
            //var xNvramFile = Path.ChangeExtension(mLaunchSettings.ConfigurationFile, ".nvram");

            //if (!File.Exists(xNvramFile))
            //{
            //    using (var xStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(VMware), "VMware.nvram"))
            //    {
            //        using (var xFile = File.Create(xNvramFile))
            //        {
            //            xStream.CopyTo(xFile);
            //        }
            //    }
            //}

            var xConfiguration = GetDefaultConfiguration();

            var xVariables = new Dictionary<string, string>()
            {
                { "$NVRAM_PATH$", Path.ChangeExtension(Path.GetFileName(_launchSettings.ConfigurationFile), ".nvram") },
                { "$ISO_PATH$", _launchSettings.IsoFile },
                { "$HARD_DISK_PATH$", _launchSettings.HardDiskFile },
                { "$PIPE_SERVER_NAME$", _launchSettings.PipeServerName }
            };

            xConfiguration = ReplaceConfigurationVariables(xConfiguration, xVariables);

            if (_launchSettings.UseGDB)
            {
                xConfiguration += Environment.NewLine;
                xConfiguration += "debugStub.hideBreakpoints = \"TRUE\"" + Environment.NewLine;
                xConfiguration += "debugStub.listen.guest32 = \"TRUE\"" + Environment.NewLine;
                xConfiguration += "debugStub.listen.guest32.remote = \"TRUE\"" + Environment.NewLine;
                xConfiguration += "monitor.debugOnStartGuest32 = \"TRUE\"" + Environment.NewLine;
            }

            var xConfigurationFile = _launchSettings.ConfigurationFile;

            using (var xSrc = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(VMwareHost), "VMware.vmx")))
            {
                using (var xDest = new StreamWriter(File.Open(xConfigurationFile, FileMode.Create)))
                {
                    string xLine;
                    while ((xLine = xSrc.ReadLine()) != null)
                    {
                        var xParts = xLine.Split('=');
                        if (xParts.Length == 2)
                        {
                            string xName = xParts[0].Trim();
                            string xValue = xParts[1].Trim();

                            if (String.Equals(xName, "uuid.location", StringComparison.Ordinal)
                             || String.Equals(xName, "uuid.bios", StringComparison.Ordinal))
                            {
                                // We delete uuid entries so VMware doesnt ask the user "Did you move or copy" the file
                                xValue = null;

                            }
                            else if (String.Equals(xName, "ide1:0.fileName", StringComparison.Ordinal))
                            {
                                // Set the ISO file for booting
                                xValue = "\"" + _launchSettings.IsoFile + "\"";
                            }
                            else if (String.Equals(xName, "ide0:0.fileName", StringComparison.Ordinal))
                            {
                                xValue = "\"" + _launchSettings.HardDiskFile + "\"";
                            }
                            else if (String.Equals(xName, "nvram", StringComparison.Ordinal))
                            {
                                // Point it to an initially non-existent nvram.
                                // This has the effect of disabling PXE so the boot is faster.
                                xValue = "\"" + Path.ChangeExtension(xConfigurationFile, ".nvram") + "\"";
                            }

                            if (xValue != null)
                            {
                                xDest.WriteLine(xName + " = " + xValue);
                            }
                        }
                    }

                    if (_launchSettings.UseGDB)
                    {
                        xDest.WriteLine();
                        xDest.WriteLine("debugStub.listen.guest32 = \"TRUE\"");
                        xDest.WriteLine("debugStub.hideBreakpoints = \"TRUE\"");
                        xDest.WriteLine("monitor.debugOnStartGuest32 = \"TRUE\"");
                        xDest.WriteLine("debugStub.listen.guest32.remote = \"TRUE\"");
                    }
                }
            }
        }

        private string GetDefaultConfiguration()
        {
            using (var xStream = GetType().Assembly.GetManifestResourceStream(typeof(VMwareHost), VMwareConfigurationFile))
            {
                using (var xReader = new StreamReader(xStream))
                {
                    return xReader.ReadToEnd();
                }
            }
        }

        private static string ReplaceConfigurationVariables(string configuration, Dictionary<string, string> variables)
        {
            foreach (var variable in variables)
            {
                configuration = configuration.Replace(variable.Key, variable.Value ?? String.Empty);
            }

            return configuration;
        }

        private static string GetPathname(string key, string exe)
        {
            using (var regKey = RegistryKey.OpenBaseKey(
                RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"Software\VMware, Inc.\" + key, false))
            {
                if (regKey != null)
                {
                    var installPath = (string)regKey.GetValue("InstallPath");

                    if (installPath == null)
                    {
                        return null;
                    }

                    string result = Path.Combine(installPath, exe);

                    if (File.Exists(result))
                    {
                        return result;
                    }
                }

                return null;
            }
        }
    }
}
