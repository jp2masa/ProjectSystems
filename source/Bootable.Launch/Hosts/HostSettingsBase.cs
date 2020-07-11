using System;
using System.Collections.Generic;
#if NET471
using System.Linq;
#endif

namespace Bootable.Launch.Hosts
{
    internal abstract class HostSettingsBase
    {
        private IReadOnlyDictionary<string, string> _settings;
        private string _propertyPrefix;

        protected HostSettingsBase(IReadOnlyDictionary<string, string> settings, params string[] propertyPrefixes)
        {
#if NET471
            _settings = settings.ToDictionary(s => s.Key, s => s.Value, StringComparer.OrdinalIgnoreCase);
#else
            _settings = new Dictionary<string, string>(settings, StringComparer.OrdinalIgnoreCase);
#endif
            if (propertyPrefixes.Length != 0)
            {
                _propertyPrefix = String.Join(".", propertyPrefixes) + ".";
            }
        }

        protected T GetProperty<T>(string propertyName)
        {
            if (!String.IsNullOrEmpty(_propertyPrefix))
            {
                propertyName = _propertyPrefix + propertyName;
            }

            if (_settings.TryGetValue(propertyName, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch { }
            }

            return default(T);
        }
    }
}
