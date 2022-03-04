namespace AutoVDesktop
{
    using System.Threading;
    using WindowsDesktop;
    using System;
    using System.Text.Json;
    using AutoVDesktop.IconsRestorer;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    internal static class Program
    {

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        class Config
        {
            public string[] Desktops { get; set; } = new string[] { "Desktop" };
            public int Delay { get; set; } = 1000;
            public bool RestoreIcon { get; set; } = true;
            public bool ShowInTaskbar { get; set; } = true;
            public bool DebugMode { get; set; } = false;

            public override string ToString()
            {
                return $"{Desktops} {Delay} {RestoreIcon} {ShowInTaskbar} {DebugMode}";

            }

        }

        static Config? config;
        private static bool locked = false;
        public static int threadID = 0;
        

        private static readonly DesktopRegistry _registry = new();
        private static readonly Storage _storage = new();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [STAThread]
        static void Main()
        {
            config = GetConfig();
            Init(config);

            VirtualDesktop.Configure();
            VirtualDesktop.CurrentChanged += (_, args) =>
            {
                System.Console.WriteLine("切换到桌面: " + args.NewDesktop.Name);
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string oldDesktopName = Path.GetFileName(path);
                while (locked)
                {
                    System.Console.WriteLine($"线程{threadID}: 锁等待...");
                    Thread.Sleep(100);
                }

                new Thread(() =>
                 {
                     ChangeDesktopThread(args, threadID);
                 }).Start();
                threadID++;
            };


            while (true)
            {
                /*         SpinWait.SpinUntil(() =>
                         {
                             return false;
                         }, 40960);*/
              System.Console.ReadKey();
            }
        }

        public static void ChangeDesktopThread(VirtualDesktopChangedEventArgs args, int _threadID)
        {
            System.Console.WriteLine($"线程{_threadID}: 开始运行...");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string oldDesktopName = Path.GetFileName(path);
            string? desktopPath = Path.GetDirectoryName(path);
            Thread.Sleep(config.Delay);
            if (threadID != _threadID)
            {
                System.Console.WriteLine($"线程{_threadID}: 运行中断,因为在等待时有新的进程...");
                return;
            }
            if (args.NewDesktop.Name.Equals(oldDesktopName))
            {
                System.Console.WriteLine($"线程{_threadID}: 运行中断,因为当前桌面已经是目标桌面...");
                return;
            }


            if (config.Desktops != null && desktopPath != null)
                foreach (var item in config.Desktops)
                {
                    if (item.Equals(args.NewDesktop.Name))
                    {

                        locked = true;
                        SaveIcon(oldDesktopName);
                        Win32.ChangeDesktopFolder(Path.Combine(desktopPath, args.NewDesktop.Name));
                        SetIcon(args.NewDesktop.Name);
                        threadID = 0;
                        locked = false;
                        System.Console.WriteLine($"线程{_threadID}: 运行完毕...");
                        return;
                    }
                }
        }
        static void SaveIcon(string desktopName)
        {
            var desktop = new Desktop();
            var iconPositions = desktop.GetIconsPositions();
            var registryValues = _registry.GetRegistryValues();
            _storage.SaveIconPositions(iconPositions, registryValues, desktopName);

        }
        static void SetIcon(string desktopName)
        {
            var desktop = new Desktop();

            var registryValues = _storage.GetRegistryValues(desktopName);
            _registry.SetRegistryValues(registryValues);
            var iconPositions = (NamedDesktopPoint[])_storage.GetIconPositions(desktopName);
            Console.WriteLine("开始恢复桌面图标位置: " + desktopName);
            desktop.SetIconPositions(iconPositions);

            desktop.Refresh();
        }
        static Config GetConfig()
        {
            string? configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.json");
            if (string.IsNullOrEmpty(configPath))
            {
                MessageBox.Show("加载配置时出现错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }

            if (File.Exists(configPath) == false)
            {
                MessageBox.Show("配置文件不存在,已重新生成.请先编辑后再运行.", "配置文件不存在", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReConfig();
                System.Environment.Exit(0);
            }
            string jsonString = File.ReadAllText(configPath);
            try
            {
                return JsonSerializer.Deserialize<Config>(jsonString)!;
            }
            catch (Exception)
            {

                if (MessageBox.Show("配置文件解析失败,请修改配置文件或者重新生成:\n是否重新生成配置文件?", "配置文件错误",
        MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    ReConfig();
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }

            return new Config();
        }

        //重新生成配置文件
        static void ReConfig()
        {
            string? configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.json");

            using (Stream s = File.OpenWrite(configPath))
            {

                var c = new Config();
                byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes<Config>(c);
                s.Write(jsonUtf8Bytes);
                Process p = new();
                p.StartInfo.FileName = "explorer.exe";
                p.StartInfo.Arguments = " /select, " + configPath;
                p.Start();
            };

        }

        //初始化,检查配置文件
        static void Init(Config config)
        {
            //检查文件夹名的合法性
            foreach (var desktopName in config.Desktops)
            {
                Regex regex = new(@"[\/?*:|\\<>]");
                if (regex.IsMatch(desktopName))
                {
                    Console.WriteLine("非法的文件夹名称: " + desktopName);
                    MessageBox.Show("非法的文件夹名称: " + desktopName + "\n请修改后重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(0);
                }
            }
            if (config.ShowInTaskbar)
            {
                 new NotifyIcon
                {
                    Icon = Properties.Resources.Icon,
                    Visible = true,
                    Text = "AutoVDesktop\n点我没用的,想关的话自己去任务管理器找我吧"
                };
            }
            if (config.DebugMode)
            {
                AllocConsole();
                System.Console.WriteLine("这里是Debug窗口,可以在配置文件里将[DebugMode]属性改为false关闭该窗口的显示.");
            }
        }
    }
}