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
                MessageBox.Show("Ӧ�ó����Ѿ��������С���","��ʾ",MessageBoxButtons.OK);
                Thread.Sleep(1000);
                System.Environment.Exit(1);
            }
            ApplicationConfiguration.Initialize();
            config = GetConfig();
            Init(config);

            VirtualDesktop.Configure();
            VirtualDesktop.CurrentChanged += (_, args) =>
            {
                Logger.Debug("�л�������: " + args.NewDesktop.Name);
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string oldDesktopName = Path.GetFileName(path);

                SpinWait.SpinUntil(() =>
                {
                    Logger.Debug($"�߳�{threadID}: ���ȴ�...");
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
            Logger.Debug($"�߳�{_threadID}: ��ʼ����...");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string oldDesktopName = Path.GetFileName(path);
            string? desktopPath = Path.GetDirectoryName(path);
            Thread.Sleep(config.Delay);
            locked = true;
            if (threadID != _threadID)
            {
                Logger.Debug($"�߳�{_threadID}: �����ж�,��Ϊ�ڵȴ�ʱ���µĽ���...");
                locked = false;
                return;
            }
            if (args.NewDesktop.Name.Equals(oldDesktopName))
            {
                Logger.Debug($"�߳�{_threadID}: �����ж�,��Ϊ��ǰ�����Ѿ���Ŀ������...");
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
                        Logger.Debug($"�߳�{_threadID}: �������...");
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
            Console.WriteLine("��ʼ�ָ�����ͼ��λ��: " + desktopName);
            desktop.SetIconPositions(iconPositions);

            desktop.Refresh();
        }
        static Config GetConfig()
        {
            string? configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.json");
            if (string.IsNullOrEmpty(configPath))
            {
                MessageBox.Show("��������ʱ���ִ���", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }

            if (File.Exists(configPath) == false)
            {
                MessageBox.Show("û�м�⵽�����ļ���������ǵ�һ�����У��������֪ͨ���ҵ�����ͼ�꣬�Ҽ����Դ�ѡ���");
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

                if (MessageBox.Show("�����ļ�����ʧ��,���޸������ļ�������������:\n�Ƿ��������������ļ�?", "�����ļ�����",
        MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                 return     ReConfig();
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }
            MessageBox.Show("����δ֪�����ü��ش��󣬽��˳�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            System.Environment.Exit(0); 
            return null;
            
        }

        //�������������ļ�
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
        //��������
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
        //��ʼ��,��������ļ�
        static void Init(Config config)
        {
            //����ļ������ĺϷ���
            foreach (var desktopName in config.Desktops)
            {
                Regex regex = new(@"[\/?*:|\\<>]");
                if (regex.IsMatch(desktopName))
                {
                    Console.WriteLine("�Ƿ����ļ�������: " + desktopName);
                    MessageBox.Show("�Ƿ����ļ�������: " + desktopName + "\n���޸ĺ�����", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(0);
                }
            }
            if (config.DebugMode)
            {
                AllocConsole();
                Logger.Debug("������Debug����,�����������ļ��ｫ[DebugMode]���Ը�Ϊfalse�رոô��ڵ���ʾ.");
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