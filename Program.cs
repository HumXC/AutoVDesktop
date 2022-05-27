namespace AutoVDesktop
{
    using System.Threading;
    using System;
    using AutoVDesktop.IconsRestorer;
    using System.Diagnostics;
    using Microsoft.Win32;
    using System.Management;
    using AutoVDesktop.VirtualDesktop;
    using System.Security.Principal;
    using System.Text;

    internal static class Program
    {

        public static Config config = new();
        private static readonly object lockObj = new();
        private static int threadID = 0;
        static private void registryEventHandler(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("Received an event:");

            //Iterate over the properties received from the event and print them out.
            foreach (var prop in e.NewEvent.Properties)
            {
                Logger.Debug($"{prop.Name}:{prop.Value}");
            }

        }

        [STAThread]
        static void Main()
        {
           
       
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.CompanyName);
            if (processes.Length > 1)
            {
                MessageBox.Show("应用程序已经在运行中。。", "提示", MessageBoxButtons.OK);
                Environment.Exit(0);
            }

            ApplicationConfiguration.Initialize();
            InitConf(config);
            /*            VirtualDesktop.CurrentChanged += (_, args) =>
                        {
                            Logger.Debug($"线程{threadID}: 切换桌面: {args.OldDesktop.Name} -> {args.NewDesktop.Name}");
                            ThreadPool.QueueUserWorkItem((state) => { ChangeDesktop(args.NewDesktop.Name, threadID); });
                            ++threadID;
                        };*/

            VirtualDesktop.VirtualDesktop.CurrentChanged += (oldDesktop,newDesktop) =>
            {
                Logger.Debug("更换桌面"+VirtualDesktop.VirtualDesktop.NowDesktop.Name);
            };
            VirtualDesktop.VirtualDesktop.Created += (desktop) =>
            {
                Logger.Debug("新增桌面" + desktop.Name);
            };
            VirtualDesktop.VirtualDesktop.Removed+=(desktop) => {
                Logger.Debug("移除桌面" + desktop.Name);
            };
            Application.Run(new OptionView());

        }
        static void ChangeDesktop(string desktopName, int _threadID)
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
                string oldDesktopName = Path.GetFileName(path);
                string? desktopPath = Path.GetDirectoryName(path);
                if (desktopName.Equals(oldDesktopName))
                {
                    Logger.Debug($"线程{_threadID}: 运行中断,因为当前桌面已经是目标桌面...");
                    Logger.Debug($"线程{_threadID}: 解锁...");
                    return;
                }
                if (config.Desktops != null && desktopPath != null)
                    foreach (var item in config.Desktops)
                    {
                        if (item.Equals(desktopName))
                        {
                            var fullNewDesktopPath = Path.Combine(desktopPath, desktopName);
                            if (!Directory.Exists(fullNewDesktopPath))
                            {
                                Directory.CreateDirectory(fullNewDesktopPath);
                            }
                            if (config.RestoreIcon)
                            {
                                SaveIcon(oldDesktopName);
                                IconsRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                Thread.Sleep(80 + (int)config.Delay / 10);
                                SetIcon(desktopName);
                            }
                            else
                            {
                                IconsRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                            }

                            Logger.Debug($"线程{_threadID}: 运行完毕，解锁...");
                            // threadID = 0;
                            return;
                        }
                    }
                Logger.Debug($"线程{_threadID}: 运行结束,因为目标桌面没有在配置中...:{desktopName}");
                return;
            }


        }
        static void SaveIcon(string desktopName)
        {
            var desktop = new IconsRestorer.Desktop();
            IconsRestorer.Desktop.Refresh();
            var iconPositions = desktop.GetIconsPositions();
            // var registryValues = _registry.GetRegistryValues();
            Storage.SaveIconPositions(iconPositions, desktopName);
        }
        static void SetIcon(string desktopName)
        {
            var desktop = new IconsRestorer.Desktop();

            //var registryValues = _storage.GetRegistryValues(desktopName);
            //_registry.SetRegistryValues(registryValues);
            var iconPositions = (NamedDesktopPoint[])Storage.GetIconPositions(desktopName);
            Program.Logger.Debug("开始恢复桌面图标位置: " + desktopName);

            desktop.SetIconPositions(iconPositions);

            IconsRestorer.Desktop.Refresh();
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
            var RKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (RKey == null)
            {
                MessageBox.Show("错误", "设置自启动失败, 无法打开注册表。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var appName = Environment.ProcessPath;
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