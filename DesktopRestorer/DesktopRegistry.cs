﻿using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace AutoVDesktop.DesktopRestorer
{
    internal class DesktopRegistry
    {
        private const string KeyName = @"Software\Microsoft\Windows\Shell\Bags\1\Desktop";
        private readonly BinaryFormatter _formatter = new();
        public IDictionary<string, string>? GetRegistryValues()
        {
            using var registry = Registry.CurrentUser.OpenSubKey(KeyName);
            if (registry == null)
            {
                return null;
            }
            return registry.GetValueNames().ToDictionary(n => n, n => GetValue(registry, n));
        }

        private string GetValue(RegistryKey registry, string valueName)
        {
            var value = registry.GetValue(valueName);
            if (value == null)
            { return string.Empty; }

            using var stream = new MemoryStream();
            _formatter.Serialize(stream, value);
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
            return _formatter.Deserialize(stream);
        }
    }
}
