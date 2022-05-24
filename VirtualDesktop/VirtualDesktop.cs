using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AutoVDesktop.VirtualDesktop
{
    internal class VirtualDesktop
    {
        private static WqlEventQuery changedQuery;
        private static WqlEventQuery creatOrRmQuery;

        public static Desktop NowDesktop { get; private set; }
        public static List<Desktop> Desktops { get; private set; } = new List<Desktop>();
        static VirtualDesktop()
        {
            // 注册 注册表监听器
            var currentUser = WindowsIdentity.GetCurrent();
            if (currentUser.User == null)
            {
                throw new Exception("无法获取用户信息");
            }
            VirtualDesktop.changedQuery = new WqlEventQuery(string.Format(
                "SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\\\{1}' AND ValueName='{2}'",
                currentUser.User.Value, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops".Replace("\\", "\\\\"), "CurrentVirtualDesktop"));
            VirtualDesktop.creatOrRmQuery = new WqlEventQuery(string.Format(
                "SELECT * FROM RegistryTreeChangeEvent WHERE Hive='HKEY_USERS' AND RootPath='{0}\\\\{1}'",
                 currentUser.User.Value, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops".Replace("\\", "\\\\")));

            var _watcher = new ManagementEventWatcher(VirtualDesktop.changedQuery);
            _watcher.EventArrived += (sender, args) =>
            {
                Program.Logger.Debug("切换桌面");
            };
            _watcher.Start();
            var _watcher2 = new ManagementEventWatcher(VirtualDesktop.creatOrRmQuery);
            _watcher2.EventArrived += (sender, args) =>
            {
                Program.Logger.Debug("创建桌面");
            };
            _watcher2.Start();
            var guidList = GetDeskGuid();
            foreach (var guid in guidList)
            {
                var desk = new Desktop(guid);
                VirtualDesktop.Desktops.Add(desk);
            }

            NowDesktop = GetNowDesktop();
        }
        static private List<Guid> GetDeskGuid()
        {
            List<Guid> list = new();
            var reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops");
            if (reg == null)
            {
                return list;
            }
            foreach (string guid in reg.GetSubKeyNames())
            {
                list.Add(Guid.Parse(guid));
            }
            return list;
        }

        private static Desktop GetNowDesktop()
        {
            var reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops");
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
        class CurrentChangedObj
        {
            public Desktop OldDesktop { get; private set; }
            public Desktop NewDesktop { get; private set; }
            public CurrentChangedObj(Desktop oldDesk, Desktop newDesk)
            {
                OldDesktop = oldDesk;
                NewDesktop = newDesk;
            }
        }
    }
    class Desktop
    {
        public string Name { get; private set; }
        public string Wallpaper { get; private set; }
        public Guid Guid { get; private set; }
        private readonly RegistryKey reg;
        public Desktop(Guid guid)
        {

            Guid = guid;

            var r = Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops\{{{guid}}}",true);
            if (r == null)
            {
                throw new Exception("无法打开桌面的注册表项: Guid=" + guid);
            }
            reg = r;
            var _name = r.GetValue("Name");
            if (_name == null)
            {
                throw new Exception("桌面名称为空");
            }
            var _wallpaper = r.GetValue("Wallpaper");
            if (_wallpaper == null)
            {
                throw new Exception("桌面壁纸为空");
            }
            Name = (string)_name;
            Wallpaper = (string)_wallpaper;
        }
    }

}
