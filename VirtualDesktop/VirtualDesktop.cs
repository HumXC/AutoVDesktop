using Microsoft.Win32;
using System;
using System.Management;
using System.Security.Principal;

namespace AutoVDesktop.VirtualDesktop
{
    internal class VirtualDesktop
    {
        private static readonly WqlEventQuery changedQuery;
        public static Desktop NowDesktop { get; private set; }

        // 当前桌面改变事件
        public delegate void CurrentChangedHandler(Desktop lastDesktop, Desktop newDesktop);
        private static event CurrentChangedHandler? EventCurrentChanged;
        public static event CurrentChangedHandler? CurrentChanged
        {
            add { EventCurrentChanged += value; }
            remove
            {
                if (value == null) { return; }
                EventCurrentChanged -= value;
            }
        }
        static VirtualDesktop()
        {
            NowDesktop = GetNowDesktop();
            // 注册 注册表监听器
            var currentUser = WindowsIdentity.GetCurrent();

            if (currentUser.User == null)
            {
                throw new Exception("无法获取用户信息");
            }
            // 查询当前桌面的更改
            changedQuery = new WqlEventQuery(string.Format(
                         "SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\\\{1}' AND ValueName='{2}'",
                 currentUser.User.Value, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops".Replace("\\", "\\\\"), "CurrentVirtualDesktop"));
            var _watcher = new ManagementEventWatcher(changedQuery);
            // 切换桌面的时候就会触发，但是事件的参数没有什么有价值的内容，所以使用丢弃
            _watcher.EventArrived += (_, _) =>
                    {
                        var oldDesktop = NowDesktop;
                        NowDesktop = GetNowDesktop();
                        VirtualDesktop.EventCurrentChanged?.Invoke(oldDesktop, NowDesktop);
                    };
            _watcher.Start();
        }

        // 获取当前所在的桌面
        private static Desktop GetNowDesktop()
        {
            var reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops");
            using (reg)
            {
                if (reg == null)
                {
                    throw new Exception("无法获取当前桌面, 注册表项为空。");
                }
                if (reg.GetValue("CurrentVirtualDesktop") is not byte[] b)
                {
                    throw new Exception("当前桌面的值为空");
                }
                return new Desktop(ByteToGuid(b));
            }
        }

        /*
         * Guid的字节数组的字符串形式转换回GUID
         * 用于转换 Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops 中 CurrentVirtualDesktop 的值为 GUID
        */
        public static Guid ByteToGuid(byte[] bytes)
        {
            string[] strs = new string[8] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, };
            strs[1] = Convert.ToString(BitConverter.ToInt16(new byte[2] { bytes[0], bytes[1] }, 0), 16).PadLeft(4, '0');
            strs[0] = Convert.ToString(BitConverter.ToInt16(new byte[2] { bytes[2], bytes[3] }, 0), 16).PadLeft(4, '0');
            strs[2] = Convert.ToString(BitConverter.ToInt16(new byte[2] { bytes[4], bytes[5] }, 0), 16).PadLeft(4, '0');
            strs[3] = Convert.ToString(BitConverter.ToInt16(new byte[2] { bytes[6], bytes[7] }, 0), 16).PadLeft(4, '0');
            strs[4] = Convert.ToString(BitConverter.ToInt16(new byte[2] { bytes[9], bytes[8] }, 0), 16).PadLeft(4, '0');
            strs[5] = Convert.ToString(bytes[10], 16).PadLeft(2, '0') + Convert.ToString(bytes[11], 16).PadLeft(2, '0');
            strs[6] = Convert.ToString(bytes[12], 16).PadLeft(2, '0') + Convert.ToString(bytes[13], 16).PadLeft(2, '0');
            strs[7] = Convert.ToString(bytes[14], 16).PadLeft(2, '0') + Convert.ToString(bytes[15], 16).PadLeft(2, '0');
            return new Guid(String.Format("{0}{1}-{2}-{3}-{4}-{5}{6}{7}", strs[0], strs[1], strs[2], strs[3], strs[4], strs[5], strs[6], strs[7]));
        }
    }
    class Desktop
    {
        public string Name { get; private set; }
        public string Wallpaper { get; private set; }
        public Guid Guid { get; private set; }
        public Desktop(Guid guid)
        {
            Guid = guid;
            var reg = Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops\{{{guid}}}", true);
            using (reg)
            {
                if (reg == null)
                {
                    throw new Exception("无法打开桌面的注册表项: Guid=" + guid);
                }
                var _name = reg.GetValue("Name");
                if (_name == null)
                {
                    _name = String.Empty;
                }
                var _wallpaper = reg.GetValue("Wallpaper");
                if (_wallpaper == null)
                {
                    _wallpaper = String.Empty;
                }
                Name = (string)_name;
                Wallpaper = (string)_wallpaper;
            }
        }
        public override string ToString()
        {
            return $"Name=\"{Name}\",Wallpaper=\"{Wallpaper}\"";
        }
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) { return false; }
            return Guid.GetHashCode() == obj.GetHashCode();
        }
    }

}
