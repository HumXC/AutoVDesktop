using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace AutoVDesktop.IconsRestorer
{
    internal class DesktopRegistry
    {
        private const string KeyName = @"Software\Microsoft\Windows\Shell\Bags\1\Desktop";
        readonly XmlSerializer serializer = new (typeof(string));
        public IDictionary<string, string> GetRegistryValues()
        {
            using var registry = Registry.CurrentUser.OpenSubKey(KeyName);
            return registry.GetValueNames().ToDictionary(n => n, n => GetValue(registry, n));
        }

        private string GetValue(RegistryKey registry, string valueName)
        {
            var value = registry.GetValue(valueName);
            if (value == null)
            { return string.Empty; }

            using var stream = new MemoryStream();
            
            serializer.Serialize(stream, value);

            var bytes = stream.ToArray();

            return Convert.ToBase64String(bytes);
        }

        public void SetRegistryValues(IDictionary<string, string> values)
        {
            using var registry = Registry.CurrentUser.OpenSubKey(KeyName, true);
            foreach (var item in values)
            {
                var v = GetValue(item.Value);
                if (registry != null && v != null)
                {
                    registry.SetValue(item.Key, v);
                }

            }
        }

        private object? GetValue(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            { return null; }

            var bytes = Convert.FromBase64String(stringValue);

            using var stream = new MemoryStream(bytes);
            return serializer.Deserialize(stream);
        }
    }
}
