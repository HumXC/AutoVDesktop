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
            /* TODO:
             * 程序目前已经可用，但仍留有一些问题需要解决。
             * 在 桌面右键->查看->将图标对齐到网格 选项开启时，会存在图标错位的问题。
             * 因为在更换桌面路径的时候，会自动排列桌面图标，如果这个选项开启，则可能出现某个图标的新位置已经被占领的情况，引起错位
             * 好像已经找到了解决方法，就是在图标重新安排位置之前关闭这个选项先。
             * 计算机\HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Bags\1\Desktop
             * FFlags
             * 
             * 有3个设置选项
             * 1 - 自动排列图标
             * 2 - 将图标与网格对齐
             * 3 - 显示桌面图标
             * 0x40201220 - 全关
             * 0x40200220 - 3
             * 0x40200221 - 3.1
             * 0x40200224 - 3.2
             * 0x40200225 - 3.1.2
             * 
             * 不过我隐约感觉这不是最佳的处理方式，有空再思考了。
             */

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
                ThreadPool.QueueUserWorkItem((state) => { ChangeDesktop(newDesktop.Name, threadID); });
                ++threadID;
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
                                SetIcon(desktopName);
                               
                            }
                            else
                            {
                                IconsRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                            }

                            Logger.Debug($"线程{_threadID}: 运行完毕，解锁...");
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
            var iconPositions = desktop.GetIconsPositions();
            Storage.SaveIconPositions(iconPositions, desktopName);
        }
        static void SetIcon(string desktopName)
        {
            var desktop = new IconsRestorer.Desktop();
            var iconPositions = (NamedDesktopPoint[])Storage.GetIconPositions(desktopName);
            Program.Logger.Debug("开始恢复桌面图标位置: " + desktopName);
            desktop.SetIconPositions(iconPositions);
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