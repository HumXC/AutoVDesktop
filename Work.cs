using AutoVDesktop.IconsRestorer;

namespace AutoVDesktop
{
    internal class Work
    {
        private static readonly DesktopRegistry _registry = new();
        private static readonly Desktop _desktop = new();
        private static readonly Storage _storage = new();

        public Work()
        {
        /*    _registry = new();
            _desktop = new();
            _storage = new();*/

        }
        public static bool Start(string oldDesktopName, string newDesktopName, bool changeIcon)
        {
            SaveIcon(oldDesktopName);
            string? desktopPath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            if (desktopPath == null || desktopPath == null)
            {
                MessageBox.Show("错误的桌面路径:" + desktopPath);
                return false;
            }

            Win32.changeDesktopPath(Path.Combine(desktopPath, newDesktopName));
            _desktop.Refresh();
            if (changeIcon)
            {


                LoadIcon(newDesktopName);
            }

            return true;
        }
        public static void SaveIcon(string desktopName)
        {
            var registryValues = _registry.GetRegistryValues();
            var iconPositions = _desktop.GetIconsPositions();
           _storage.SaveIconPositions(iconPositions, registryValues, desktopName);

        }
        public static void LoadIcon(string desktopName)
        {
            var registryValues = _storage.GetRegistryValues(desktopName);
            _registry.SetRegistryValues(registryValues);
            var iconPositions = _storage.GetIconPositions(desktopName);
            _desktop.SetIconPositions(iconPositions);
            _desktop.Refresh();
        }


        //无参数
        public static void SaveIcon()
        {
            var registryValues = _registry.GetRegistryValues();
            var iconPositions = _desktop.GetIconsPositions();
            _storage.SaveIconPositions(iconPositions, registryValues);

        }
        public static void LoadIcon()
        {
            var registryValues = _storage.GetRegistryValues();
            _registry.SetRegistryValues(registryValues);
            var iconPositions = _storage.GetIconPositions();
            _desktop.SetIconPositions(iconPositions);
            _desktop.Refresh();
        }
    }
}
