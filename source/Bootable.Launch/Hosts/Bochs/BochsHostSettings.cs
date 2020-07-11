using System.Collections.Generic;

namespace Bootable.Launch.Hosts.Bochs
{
    internal class BochsHostSettings : HostSettingsBase
    {
        public string BochsDirectory { get; }

        public string ConfigurationFile { get; }
        public bool OverwriteConfigurationFile { get; }

        public string IsoFile { get; }
        public string HardDiskFile { get; }

        public string ConfigurationInterface { get; }

        public string DisplayLibrary { get; }
        public BochsDisplayLibraryOptions DisplayLibraryOptions { get; }

        public bool UseDebugVersion { get; }
        public bool StartDebugGui { get; }

        public string PipeClientName { get; }
        public string PipeServerName { get; }

        public BochsHostSettings(IReadOnlyDictionary<string, string> settings)
            : base(settings)
        {
            BochsDirectory = GetProperty<string>(nameof(BochsDirectory));

            ConfigurationFile = GetProperty<string>(nameof(ConfigurationFile));
            OverwriteConfigurationFile = GetProperty<bool>(nameof(OverwriteConfigurationFile));

            IsoFile = GetProperty<string>(nameof(IsoFile));
            HardDiskFile = GetProperty<string>(nameof(HardDiskFile));

            ConfigurationInterface = GetProperty<string>(nameof(ConfigurationInterface));

            DisplayLibrary = GetProperty<string>(nameof(DisplayLibrary));
            DisplayLibraryOptions = new BochsDisplayLibraryOptions(settings);

            UseDebugVersion = GetProperty<bool>(nameof(UseDebugVersion));
            StartDebugGui = GetProperty<bool>(nameof(StartDebugGui));

            PipeClientName = GetProperty<string>(nameof(PipeClientName));
            PipeServerName = GetProperty<string>(nameof(PipeServerName));
        }

        public class BochsDisplayLibraryOptions : HostSettingsBase
        {
            public BochsDisplayLibraryOptions(IReadOnlyDictionary<string, string> settings)
                : base(settings, nameof(DisplayLibraryOptions))
            {
                GuiDebug = GetProperty<bool>(nameof(GuiDebug));
                HideIps = GetProperty<bool>(nameof(HideIps));
                NoKeyRepeat = GetProperty<bool>(nameof(NoKeyRepeat));
                Timeout = GetProperty<bool>(nameof(Timeout));
            }

            public bool GuiDebug { get; }
            public bool HideIps { get; }
            public bool NoKeyRepeat { get; }
            public bool Timeout { get; }
        }
    }
}
