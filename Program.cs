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

        public class Config
        {
            public string[] Desktops { get; set; } = Array.Empty<string>();
            public int Delay { get; set; } = 1000;
            public bool RestoreIcon { get; set; } = true;
            public bool ShowNotifyIcon { get; set; } = true;
            public bool DebugMode { get; set; } = false;

            public override string ToString()
            {
                return $"{Desktops} {Delay} {RestoreIcon} {ShowNotifyIcon} {DebugMode}";

            }

        }

        public static Config? config;
        private static bool locked = false;
        private static int threadID = 0;


        private static readonly DesktopRegistry _registry = new();
        private static readonly Storage _storage = new();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [STAThread]
        static void Main()
        {
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.CompanyName);
            if (processes.Length > 1)
            {
                MessageBox.Show("应用程序已经在运行中。。","提示",MessageBoxButtons.OK);
                Thread.Sleep(1000);
                System.Environment.Exit(1);
            }
            ApplicationConfiguration.Initialize();
            config = GetConfig();
            Init(config);

            VirtualDesktop.Configure();
            VirtualDesktop.CurrentChanged += (_, args) =>
            {
                Logger.Debug("切换到桌面: " + args.NewDesktop.Name);
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string oldDesktopName = Path.GetFileName(path);

                SpinWait.SpinUntil(() =>
                {
                    Logger.Debug($"线程{threadID}: 锁等待...");
                    return !locked;
                }, 50);
                new Thread(() =>
                                  {
                                      ChangeDesktopThread(args, threadID);
                                  })
                {
                    IsBackground = true,
                    Name = threadID.ToString()
                }.Start();
                ++threadID;
            };

            Application.Run(new OptionView());

        }

        public static void ChangeDesktopThread(VirtualDesktopChangedEventArgs args, int _threadID)
        {
            Logger.Debug($"线程{_threadID}: 开始运行...");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string oldDesktopName = Path.GetFileName(path);
            string? desktopPath = Path.GetDirectoryName(path);
            Thread.Sleep(config.Delay);
            locked = true;
            if (threadID != _threadID)
            {
                Logger.Debug($"线程{_threadID}: 运行中断,因为在等待时有新的进程...");
                locked = false;
                return;
            }
            if (args.NewDesktop.Name.Equals(oldDesktopName))
            {
                Logger.Debug($"线程{_threadID}: 运行中断,因为当前桌面已经是目标桌面...");
                locked = false;
                return;
            }


            if (config.Desktops != null && desktopPath != null)
                foreach (var item in config.Desktops)
                {
                    if (item.Equals(args.NewDesktop.Name))
                    {
                        var fullNewDesktopPath = Path.Combine(desktopPath, args.NewDesktop.Name);
                        if (!Directory.Exists(fullNewDesktopPath))
                        {
                            Directory.CreateDirectory(fullNewDesktopPath);
                        }
                        SaveIcon(oldDesktopName);
                        Win32.ChangeDesktopFolder(fullNewDesktopPath);
                        SetIcon(args.NewDesktop.Name);
                        threadID = 0;
                        Logger.Debug($"线程{_threadID}: 运行完毕...");
                        locked = false;
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
                MessageBox.Show("没有检测到配置文件，你可能是第一次运行，你可以在通知栏找到程序图标，右键可以打开选项窗口");
                ReConfig();
                new Info().Show();
                return ReConfig();
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
                 return     ReConfig();
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }
            MessageBox.Show("出现未知的配置加载错误，将退出", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            System.Environment.Exit(0); 
            return null;
            
        }

        //重新生成配置文件
        static Config ReConfig()
        {
            string? configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.json");
  File.Delete(configPath);
            using (Stream s = File.OpenWrite(configPath))
            {
                Config c = new();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                c.Desktops = new String[] { Path.GetFileName(path)};
                byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes<Config>(c);
                s.Write(jsonUtf8Bytes);
                return c;

            };

        }
        //保存配置
       public static void SaveConfig()
        {
            string? configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.json");
            File.Delete(configPath);
            using (Stream s = File.OpenWrite(configPath))
            {
                byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes<Config>(config);
                s.Write(jsonUtf8Bytes);
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
            if (config.DebugMode)
            {
                AllocConsole();
                Logger.Debug("这里是Debug窗口,可以在配置文件里将[DebugMode]属性改为false关闭该窗口的显示.");
            }
            if (config.Delay < 1)
            {
                config.Delay = 1000;
            }
        }
        public static class Logger
        {
            public static void Debug(string msg)
            {
                if (config.DebugMode == true) { System.Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {msg}"); }
            }
        }
    }
}