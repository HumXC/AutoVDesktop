namespace AutoVDesktop
{
    using System.Threading;
    using System;
    using AutoVDesktop.DesktopRestorer;
    using System.Diagnostics;
    using Microsoft.Win32;
    internal static class Program
    {

        public static Config config = new();
        private static readonly object lockObj = new();
        private static int threadID = 0;

        private static readonly string dataPath;
        static Program()
        {
            var path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (path == null)
            {
                MessageBox.Show("错误", "无法读取程序运行目录", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            dataPath = Path.Combine(path, "Desktops");
        }
        [STAThread]
        static void Main()
        {
            try
            {
                Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.CompanyName);
                if (processes.Length > 1)
                {
                    MessageBox.Show("应用程序已经在运行中。。", "提示", MessageBoxButtons.OK);
                    Environment.Exit(0);
                }

                ApplicationConfiguration.Initialize();
                InitConf(config);
                VirtualDesktop.VirtualDesktop.CurrentChanged += (oldDesktop, newDesktop) =>
                {
                    Logger.Debug($"线程{threadID}: 切换桌面: {oldDesktop.Name} -> {newDesktop.Name}");
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        try
                        {
                            ChangeDesktop(newDesktop.Name, threadID);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("错误", "切换桌面时出现错误: \n" + e.Message + "\n" + e.StackTrace, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(1);
                        }
                    });
                    ++threadID;
                };
                Application.Run(new OptionView());
            }
            catch (Exception e)
            {
                MessageBox.Show("错误", "错误: \n" + e.Message + "\n" + e.StackTrace, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

        }
        static void ChangeDesktop(string newDesktopName, int _threadID)
        {
            Thread.Sleep(config.Delay);
            if (threadID != _threadID)
            {
                Logger.Debug($"线程{_threadID}: 运行中断,因为在等待时有新的进程...");
                return;
            }
            lock (lockObj)
            {
                Logger.Debug($"线程{_threadID}: 获得锁,开始运行...");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                Logger.Debug("Desktop_Path=" + path);
                string nowDesktopName = Path.GetFileName(path);
                string? desktopPath = Path.GetDirectoryName(path);
                if (newDesktopName.Equals(nowDesktopName))
                {
                    Logger.Debug($"线程{_threadID}: 运行中断,因为当前桌面已经是目标桌面...");
                    Logger.Debug($"线程{_threadID}: 解锁...");
                    return;
                }
                if (config.Desktops != null && desktopPath != null)
                    foreach (var item in config.Desktops)
                    {
                        if (item.Equals(newDesktopName))
                        {
                            var fullNewDesktopPath = Path.Combine(desktopPath, newDesktopName);
                            if (!Directory.Exists(fullNewDesktopPath))
                            {
                                Directory.CreateDirectory(fullNewDesktopPath);
                            }
                            if (config.RestoreDesktop)
                            {
                                var iconCount = SaveDesktop(nowDesktopName).IconCount;
                                DesktopRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                DesktopRestorer.Desktop.Refresh();
                                if (iconCount < 8) { Thread.Sleep(200); }
                                SetDesktop(newDesktopName);
                            }
                            else
                            {
                                DesktopRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                DesktopRestorer.Desktop.Refresh();
                            }

                            Logger.Debug($"线程{_threadID}: 运行完毕，解锁...");
                            return;
                        }
                    }
                Logger.Debug($"线程{_threadID}: 运行结束,因为目标桌面没有在配置中...:{newDesktopName}");
                return;
            }
        }
        static DesktopRestorer.Desktop SaveDesktop(string desktopName)
        {
            var desktop = new DesktopRestorer.Desktop();
            var iconPositions = desktop.GetIconsPositions();
            Program.Logger.Debug("开始保存桌面图标位置: " + desktopName);
            Storage.SaveIconPositions(iconPositions, Path.Combine(dataPath, desktopName + ".xml"));
            return desktop;
        }
        static DesktopRestorer.Desktop SetDesktop(string desktopName)
        {
            var desktop = new DesktopRestorer.Desktop();
            var iconPositions = Storage.GetIconPositions(Path.Combine(dataPath, desktopName + ".xml"));
            Program.Logger.Debug("开始恢复桌面图标位置: " + desktopName);
            if (config.EnsureRestore)
            {
                desktop.EnsureSetIconPositions(iconPositions);
                return desktop;
            }
            desktop.SetIconPositions(iconPositions);
            return desktop;
        }
        //初始化,检查配置文件
        static void InitConf(Config config)
        {
            config.LoadConfig();
            if (config.DebugMode)
            {
                Win32.AllocConsole();
                Logger.Debug("这里是Debug窗口,可以在配置文件里将[DebugMode]属性改为false关闭该窗口的显示.");
            }
            //开机自启
            var RKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (RKey == null)
            {
                MessageBox.Show("错误", "设置自启动失败, 无法打开注册表。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var appName = System.Windows.Forms.Application.ExecutablePath;
            if (appName == null)
            {
                MessageBox.Show("错误", "设置自启动失败, 程序名为 null 。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var value = RKey.GetValue("AutoVDesktop");
            if (value == null)
            {
                if (config.StartWithWindows)
                    RKey.SetValue("AutoVDesktop", appName);
            }
            else
            {
                if (config.StartWithWindows)
                {
                    if (!value.Equals(appName))
                    {
                        RKey.SetValue("AutoVDesktop", appName);
                    }
                }
                else
                {
                    RKey.DeleteValue("AutoVDesktop");
                }
            }
        }
        public static class Logger
        {
            public static void Debug(string msg)
            {
                if (config.DebugMode) { System.Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {msg}"); }
            }
        }
    }
}