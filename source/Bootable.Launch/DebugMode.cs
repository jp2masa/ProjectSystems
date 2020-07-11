using System;

namespace Bootable.Launch
{
    public class DebugMode : IEquatable<DebugMode>
    {
        public static readonly DebugMode None = new DebugMode(nameof(None), "None");
        public static readonly DebugMode PipeClient = new DebugMode(nameof(PipeClient), "Pipe Client");
        public static readonly DebugMode PipeServer = new DebugMode(nameof(PipeServer), "Pipe Server");
        public static readonly DebugMode Serial = new DebugMode(nameof(Serial), "Serial");

        public string Name { get; }
        public string DisplayName { get; }
        
        private DebugMode(string name, string displayName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public bool Equals(DebugMode other) => Name.Equals(other.Name);
    }
}
