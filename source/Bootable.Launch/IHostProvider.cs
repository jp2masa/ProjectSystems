using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bootable.Launch
{
    public interface IHostProvider
    {
        /// <summary>
        /// The name of the host provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// An enumerable of supported debug modes.
        /// </summary>
        //IEnumerable<DebugMode> SupportedDebugModes { get; }

        /// <summary>
        /// Indicates whether this host provider is supported by the system.
        /// </summary>
        /// <returns>true if this host provider is supported by the system; otherwise, false.</returns>
        //bool IsHostSupported();

        /// <summary>
        /// Creates a new <see cref="IHost"/> for the specified settings and <see cref="DebugMode"/>.
        /// </summary>
        /// <param name="settings">The host settings to use.</param>
        /// <param name="debugMode">The <see cref="DebugMode"/> to use.</param>
        /// <returns>A <see cref="Task"/> that represents the new host instance.</returns>
        Task<IHost> CreateHostAsync(IReadOnlyDictionary<string, string> settings, DebugMode debugMode);
    }
}
