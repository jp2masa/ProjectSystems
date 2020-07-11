using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Bootable.Launch.Hosts.Bochs
{
    internal class BochsHost : IHost
    {
        private const string BochsConfigurationFile = "Bochs.bxrc";

        private BochsHostSettings _settings;

        private string _bochsDir;
        private string _bochsExe;

        private Process _process;

        public event EventHandler ShutDown;

        public string ConfigInterface
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_settings.ConfigurationInterface))
                {
                    switch (DisplayLibrary)
                    {
                        case "win32":
                            return "win32config";
                        case "wx":
                            return "wx";
                        default:
                            return "textconfig";
                    }
                }

                return _settings.ConfigurationInterface;
            }
        }

        public string DisplayLibrary
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_settings.DisplayLibrary))
                {
                    // from Bochs docs:
                    // "gui_debug" - use GTK debugger gui (sdl, x) / Win32 debugger gui (sdl, sdl2, win32)
                    if (RuntimeHelper.IsWindows)
                    {
                        return "win32";
                    }
                    else if (RuntimeHelper.IsOSX || RuntimeHelper.IsLinux)
                    {
                        return "x";
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                return _settings.DisplayLibrary;
            }
        }

        public string BochsDebugSymbolsPath => Path.ChangeExtension(_settings.IsoFile, ".sym");
        
        public BochsHost(BochsHostSettings launchSettings)
        {
            _settings = launchSettings ?? throw new ArgumentNullException(nameof(launchSettings));

            _bochsDir = launchSettings.BochsDirectory;

            if (!Directory.Exists(_bochsDir) && RuntimeHelper.IsWindows)
            {
                _bochsDir = BochsSupport.BochsDirectory;
            }

            if (!Directory.Exists(_bochsDir))
            {
                throw new InvalidOperationException("Bochs installation directory not found!");
            }

            if (RuntimeHelper.IsWindows)
            {
                _bochsExe = Path.Combine(_bochsDir, launchSettings.UseDebugVersion ? "bochsdbg.exe" : "bochs.exe");
            }
            else
            {
                _bochsExe = Path.Combine(_bochsDir, launchSettings.UseDebugVersion ? "bochsdbg" : "bochs");
            }

            if (_settings.OverwriteConfigurationFile || !File.Exists(launchSettings.ConfigurationFile))
            {
                GenerateConfiguration();
            }
        }

        public Task StartAsync()
        {
            var mapFile = Path.ChangeExtension(_settings.IsoFile, ".map");
            BochsSupport.TryExtractBochsDebugSymbols(mapFile, BochsDebugSymbolsPath);

            // Start Bochs without displaying the configuration interface (-q) and using the specified
            // configuration file (-f).
            var args = $"-q -f \"{_settings.ConfigurationFile}\"";
            var processStartInfo = new ProcessStartInfo(_bochsExe, args);

            _process = new Process();

            _process.StartInfo = processStartInfo;
            _process.EnableRaisingEvents = true;

            _process.Exited += delegate
            {
                var lockFile = _settings.HardDiskFile + ".lock";

                if (File.Exists(lockFile))
                {
                    try
                    {
                        File.Delete(lockFile);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"The lock file couldn't be deleted! It has to be deleted manually. Lock file location: '{lockFile}'.{Environment.NewLine}Exception:{Environment.NewLine}{ex.ToString()}");
                    }
                }

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
                    return Task.Run((Action)_process.WaitForExit);
                }
            }
            catch (InvalidOperationException)
            {
            }

            return Task.CompletedTask;
        }

        private void GenerateConfiguration()
        {
            var configuration = GetDefaultConfiguration();

            var romImage = Path.Combine(_bochsDir, "BIOS-bochs-latest");
            var vgaRomImage = Path.Combine(_bochsDir, "VGABIOS-lgpl-latest");

            var variables = new Dictionary<string, string>()
            {
                { "$CONFIG_INTERFACE$", ConfigInterface },
                { "$DISPLAY_LIBRARY$", DisplayLibrary },
                { "$DISPLAY_LIBRARY_OPTIONS$", _settings.DisplayLibraryOptions.ToString() },
                { "$DEBUG_SYMBOLS_PATH$", BochsDebugSymbolsPath },
                { "$ROM_IMAGE$", romImage },
                { "$VGA_ROM_IMAGE$", vgaRomImage },
                { "$CDROM_BOOT_PATH$", _settings.IsoFile },
                { "$HARD_DISK_PATH$", _settings.HardDiskFile },
                { "$PIPE_SERVER_NAME$", _settings.PipeServerName }
            };

            configuration = ReplaceConfigurationVariables(configuration, variables);

            if (_settings.UseDebugVersion)
            {
                configuration = configuration + "magic_break: enabled = 1" + Environment.NewLine;
            }

            using (var writer = File.CreateText(_settings.ConfigurationFile))
            {
                writer.Write(configuration);
            }
        }
        
        private string GetDefaultConfiguration()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(typeof(BochsHost), BochsConfigurationFile))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
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
    }
}
