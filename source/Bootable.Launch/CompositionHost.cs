using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bootable.Launch
{
    public class CompositionHost
    {
        public IEnumerable<IHostProvider> HostProviders => _hostProviders.Value;
        public IEnumerable<IDebugConnectorProvider> DebugConnectorProviders => _debugConnectorProviders.Value;

        private Lazy<IEnumerable<IHostProvider>> _hostProviders;
        private Lazy<IEnumerable<IDebugConnectorProvider>> _debugConnectorProviders;

        public CompositionHost()
        {
            _hostProviders = new Lazy<IEnumerable<IHostProvider>>(ComposeHostProviders);
            _debugConnectorProviders = new Lazy<IEnumerable<IDebugConnectorProvider>>(ComposeDebugConnectorProviders);
        }

        private IEnumerable<IHostProvider> ComposeHostProviders() =>
            ComposeProviders<IHostProvider, ExportHostProviderAttribute>();
        private IEnumerable<IDebugConnectorProvider> ComposeDebugConnectorProviders() =>
            ComposeProviders<IDebugConnectorProvider, ExportDebugConnectorProviderAttribute>();

        private IEnumerable<T> ComposeProviders<T, TAttribute>() where TAttribute : Attribute
        {
            foreach (var type in typeof(CompositionHost).Assembly.GetTypes())
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    var attribute = type.GetCustomAttribute<TAttribute>();

                    if (attribute != null)
                    {
                        T provider = default(T);

                        try
                        {
                            provider = (T)Activator.CreateInstance(type);
                        }
                        catch (Exception e)
                        {
                            // todo: log exception
                        }

                        if (provider != null)
                        {
                            yield return provider;
                        }
                    }
                }
            }
        }
    }
}
