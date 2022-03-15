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
            public bool StartWithWindows { get; set; } = false;

            public override string ToString()
            {
                return $"{Desktops} {Delay} {RestoreIcon} {ShowNotifyIcon} {DebugMode}";

            }

        }

        public static Config? config;
        private static readonly object lockObj = new();
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
                MessageBox.Show("Ӧ�ó����Ѿ��������С���", "��ʾ", MessageBoxButtons.OK);
                Thread.Sleep(1000);
                System.Environment.Exit(1);
            }
            ApplicationConfiguration.Initialize();
            VirtualDesktop.Configure();
            config = GetConfig();
            Init(config);


            //�����µ�����������������������û���������棬��������������
            VirtualDesktop.Created += (_, newVDesktop) =>
            {
                var vDesktops = VirtualDesktop.GetDesktops();
                var vDesktopNames = new List<string>();
                foreach (var vDesk in vDesktops)
                {
                    vDesktopNames.Add(vDesk.Name);
                }
                foreach (var desktopName in config.Desktops)
                {
                    if (vDesktopNames.IndexOf(desktopName) == -1)
                    {
                        newVDesktop.Name = desktopName;
                        return;
                    }
                }

            };
            VirtualDesktop.CurrentChanged += (_, args) =>
            {
                Logger.Debug($"�߳�{threadID}: �л�����: {args.OldDesktop.Name} -> {args.NewDesktop.Name}");
                ThreadPool.QueueUserWorkItem((state) => { ChangeDesktopThread(args, threadID); });
                /*                new Thread()
                                {
                                    IsBackground = true,
                                    Name ="�л������߳�"+ threadID.ToString()
                                }.Start();*/
                ++threadID;

            };
            Application.Run(new OptionView());

        }

        public static void ChangeDesktopThread(VirtualDesktopChangedEventArgs args, int _threadID)
        {
            Thread.Sleep(config.Delay);
            if (threadID != _threadID)
            {
                Logger.Debug($"�߳�{_threadID}: �����ж�,��Ϊ�ڵȴ�ʱ���µĽ���...");
                return;
            }
            lock (lockObj)
            {
                Logger.Debug($"�߳�{_threadID}: �����,��ʼ����...");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string oldDesktopName = Path.GetFileName(path);
                string? desktopPath = Path.GetDirectoryName(path);
                if (args.NewDesktop.Name.Equals(oldDesktopName))
                {
                    Logger.Debug($"�߳�{_threadID}: �����ж�,��Ϊ��ǰ�����Ѿ���Ŀ������...");
                    Logger.Debug($"�߳�{_threadID}: ����...");
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
                            if (config.RestoreIcon)
                            {
                                SaveIcon(oldDesktopName);
                                Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                Thread.Sleep(80 + (int)config.Delay / 10);
                                SetIcon(args.NewDesktop.Name);
                            }
                            else
                            {
                                Win32.ChangeDesktopFolder(fullNewDesktopPath);
                            }

                            Logger.Debug($"�߳�{_threadID}: ������ϣ�����...");
                            // threadID = 0;
                            return;
                        }
                    }
                Logger.Debug($"�߳�{_threadID}: ���н���,��ΪĿ������û����������...:{args.NewDesktop.Name}");
                return;
            }


        }
        static void SaveIcon(string desktopName)
        {
            var desktop = new Desktop();
            desktop.Refresh();
            var iconPositions = desktop.GetIconsPositions();
            // var registryValues = _registry.GetRegistryValues();
            _storage.SaveIconPositions(iconPositions, desktopName);
        }
        static void SetIcon(string desktopName)
        {
            var desktop = new Desktop();

            //var registryValues = _storage.GetRegistryValues(desktopName);
            //_registry.SetRegistryValues(registryValues);
            var iconPositions = (NamedDesktopPoint[])_storage.GetIconPositions(desktopName);
            Program.Logger.Debug("��ʼ�ָ�����ͼ��λ��: " + desktopName);

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
                var vDesktops = VirtualDesktop.GetDesktops();
                var vDesktopNames = new List<string>();
                foreach (var v in vDesktops)
                {
                    vDesktopNames.Add(v.Name);
                }
                new Info().Show();
                var c = ReConfig();
                if (c.Desktops[0] != null)
                {
                    if (vDesktopNames.IndexOf(c.Desktops[0]) == -1)
                    {
                        if (VirtualDesktop.Current.Name.Equals(""))
                        {
                            VirtualDesktop.Current.Name = c.Desktops[0];
                        }
                        else
                        {
                            VirtualDesktop.Create().Name = c.Desktops[0];
                        }
                    }
                }
                return c;
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
                    return ReConfig();
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
                c.Desktops = new String[] { Path.GetFileName(path) };
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
                    Program.Logger.Debug("�Ƿ����ļ�������: " + desktopName);
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

            //��������
            Microsoft.Win32.RegistryKey RKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            var appName = Environment.ProcessPath;
            if (appName != null)
            {
                var k = RKey.GetValue("AutoVDesktop");
                if (k == null)
                {
                    if (config.StartWithWindows)
                        RKey.SetValue("AutoVDesktop", appName);
                }
                else
                {
                    if (config.StartWithWindows)
                    {
                        if (!k.Equals(appName))
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