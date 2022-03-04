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
                System.Console.WriteLine("�л�������: " + args.NewDesktop.Name);
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string oldDesktopName = Path.GetFileName(path);
                while (locked)
                {
                    System.Console.WriteLine($"�߳�{threadID}: ���ȴ�...");
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
            System.Console.WriteLine($"�߳�{_threadID}: ��ʼ����...");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string oldDesktopName = Path.GetFileName(path);
            string? desktopPath = Path.GetDirectoryName(path);
            Thread.Sleep(config.Delay);
            if (threadID != _threadID)
            {
                System.Console.WriteLine($"�߳�{_threadID}: �����ж�,��Ϊ�ڵȴ�ʱ���µĽ���...");
                return;
            }
            if (args.NewDesktop.Name.Equals(oldDesktopName))
            {
                System.Console.WriteLine($"�߳�{_threadID}: �����ж�,��Ϊ��ǰ�����Ѿ���Ŀ������...");
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
                        System.Console.WriteLine($"�߳�{_threadID}: �������...");
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
                MessageBox.Show("�����ļ�������,����������.���ȱ༭��������.", "�����ļ�������", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                if (MessageBox.Show("�����ļ�����ʧ��,���޸������ļ�������������:\n�Ƿ��������������ļ�?", "�����ļ�����",
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

        //�������������ļ�
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
            if (config.ShowInTaskbar)
            {
                 new NotifyIcon
                {
                    Icon = Properties.Resources.Icon,
                    Visible = true,
                    Text = "AutoVDesktop\n����û�õ�,��صĻ��Լ�ȥ������������Ұ�"
                };
            }
            if (config.DebugMode)
            {
                AllocConsole();
                System.Console.WriteLine("������Debug����,�����������ļ��ｫ[DebugMode]���Ը�Ϊfalse�رոô��ڵ���ʾ.");
            }
        }
    }
}