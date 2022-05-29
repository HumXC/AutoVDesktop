using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace AutoVDesktop.DesktopRestorer
{
    internal class Desktop
    {
        private readonly IntPtr _desktopHandle;
        private readonly List<string> _currentIconsOrder;
        public readonly int IconCount;
        public Desktop()
        {
            _desktopHandle = Win32.GetDesktopWindow(Win32.DesktopWindow.SysListView32);

            AutomationElement el = AutomationElement.FromHandle(_desktopHandle);

            TreeWalker walker = TreeWalker.ContentViewWalker;
            _currentIconsOrder = new List<string>();
            for (AutomationElement child = walker.GetFirstChild(el);
                child != null;
                child = walker.GetNextSibling(child))
            {
                _currentIconsOrder.Add(child.Current.Name);
            }
            Program.Logger.Debug($"创建新的桌面对象，当前获取桌面图标个数: {_currentIconsOrder.Count}");
            IconCount = _currentIconsOrder.Count;
        }

        private int GetIconsNumber()
        {
            return (int)Win32.SendMessage(_desktopHandle, Win32.LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
        }

        public NamedDesktopPoint[] GetIconsPositions()
        {
            _ = Win32.GetWindowThreadProcessId(_desktopHandle, out uint desktopProcessId);

            IntPtr desktopProcessHandle = IntPtr.Zero;
            try
            {
                desktopProcessHandle = Win32.OpenProcess(Win32.ProcessAccess.VmOperation | Win32.ProcessAccess.VmRead |
                    Win32.ProcessAccess.VmWrite, false, desktopProcessId);

                return GetIconsPositions(desktopProcessHandle);
            }
            finally
            {
                if (desktopProcessHandle != IntPtr.Zero)
                { Win32.CloseHandle(desktopProcessHandle); }
            }
        }

        private NamedDesktopPoint[] GetIconsPositions(IntPtr desktopProcessHandle)
        {
            IntPtr sharedMemoryPointer = IntPtr.Zero;

            try
            {
                sharedMemoryPointer = Win32.VirtualAllocEx(desktopProcessHandle, IntPtr.Zero, 4096, Win32.AllocationType.Reserve | Win32.AllocationType.Commit, Win32.MemoryProtection.ReadWrite);

                return GetIconsPositions(desktopProcessHandle, sharedMemoryPointer);
            }
            finally
            {
                if (sharedMemoryPointer != IntPtr.Zero)
                {
                    Win32.VirtualFreeEx(desktopProcessHandle, sharedMemoryPointer, 0, Win32.FreeType.Release);
                }
            }

        }

        private NamedDesktopPoint[] GetIconsPositions(IntPtr desktopProcessHandle, IntPtr sharedMemoryPointer)
        {
            var listOfPoints = new LinkedList<NamedDesktopPoint>();

            var numberOfIcons = GetIconsNumber();
            Program.Logger.Debug("获取到当前桌面图标个数: " + numberOfIcons);

            for (int itemIndex = 0; itemIndex < numberOfIcons; itemIndex++)
            {
                uint numberOfBytes = 0;
                DesktopPoint[] points = new DesktopPoint[1];

                Win32.WriteProcessMemory(desktopProcessHandle, sharedMemoryPointer,
                    Marshal.UnsafeAddrOfPinnedArrayElement(points, 0),
                    Marshal.SizeOf(typeof(DesktopPoint)),
                    ref numberOfBytes);

                Win32.SendMessage(_desktopHandle, Win32.LVM_GETITEMPOSITION, itemIndex, sharedMemoryPointer);

                _ = Win32.ReadProcessMemory(desktopProcessHandle, sharedMemoryPointer,
                    Marshal.UnsafeAddrOfPinnedArrayElement(points, 0),
                    Marshal.SizeOf(typeof(DesktopPoint)),
                    ref numberOfBytes);

                var point = points[0];
                try
                {
                    listOfPoints.AddLast(new NamedDesktopPoint(_currentIconsOrder[itemIndex], point.X, point.Y));
                }
                catch (Exception e)
                {
                    Program.Logger.Debug(e.Message + " index = " + itemIndex);
                    Program.Logger.Debug(listOfPoints.Count.ToString());
                }
            }
            return listOfPoints.ToArray();
        }

        public void SetIconPositions(IEnumerable<NamedDesktopPoint> iconPositions)
        {
            foreach (var position in iconPositions)
            {
                var iconIndex = _currentIconsOrder.IndexOf(position.Name);
                if (iconIndex == -1)
                { continue; }
                Win32.SendMessage(_desktopHandle, Win32.LVM_SETITEMPOSITION, iconIndex, Win32.MakeLParam(position.X, position.Y));
            }
        }

        // 确保恢复时图标位置的准确性
        public void EnsureSetIconPositions(IEnumerable<NamedDesktopPoint> iconPositions)
        {
            SetIconPositions(iconPositions);

            while (true)
            {
                var nowPostions = new List<NamedDesktopPoint>(GetIconsPositions());
                var bad = new List<NamedDesktopPoint>();
                foreach (var item in iconPositions)
                {
                    if (nowPostions.IndexOf(item) == -1)
                    {
                        bad.Add(item);
                    }
                }
                if (bad.Count == 0)
                {
                    return;
                }
                foreach (var position in bad)
                {
                    var iconIndex = _currentIconsOrder.IndexOf(position.Name);
                    if (iconIndex == -1)
                    { continue; }
                    Win32.SendMessage(_desktopHandle, Win32.LVM_SETITEMPOSITION, iconIndex, Win32.MakeLParam(position.X, position.Y));
                }
            }
        }


        public static void Refresh()
        {
            // 使用f5刷新会导致无法获取到桌面图标信息
            //_ = Win32.PostMessage(_desktopHandle, Win32.WM_KEYDOWN, Win32.VK_F5, 0);
            _ = Win32.SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
