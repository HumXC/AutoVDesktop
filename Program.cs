namespace AutoVDesktop
{
    using System.Threading;
    using System;
    using AutoVDesktop.IconsRestorer;
    using System.Diagnostics;

    internal static class Program
    {

        public static Config config =new();
        private static readonly object lockObj = new();
        private static int threadID = 0;

        // �������̨
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
                Environment.Exit(0);
            }
            InitConf(config);
/*            VirtualDesktop.CurrentChanged += (_, args) =>
            {
                Logger.Debug($"�߳�{threadID}: �л�����: {args.OldDesktop.Name} -> {args.NewDesktop.Name}");
                ThreadPool.QueueUserWorkItem((state) => { ChangeDesktop(args.NewDesktop.Name, threadID); });
                ++threadID;
            };*/
            Application.Run(new OptionView());

        }
        public static void ChangeDesktop(string desktopName , int _threadID)
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
                if (desktopName.Equals(oldDesktopName))
                {
                    Logger.Debug($"�߳�{_threadID}: �����ж�,��Ϊ��ǰ�����Ѿ���Ŀ������...");
                    Logger.Debug($"�߳�{_threadID}: ����...");
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
                                Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                Thread.Sleep(80 + (int)config.Delay / 10);
                                SetIcon(desktopName);
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
                Logger.Debug($"�߳�{_threadID}: ���н���,��ΪĿ������û����������...:{desktopName}");
                return;
            }


        }
        static void SaveIcon(string desktopName)
        {
            var desktop = new Desktop();
            Desktop.Refresh();
            var iconPositions = desktop.GetIconsPositions();
            // var registryValues = _registry.GetRegistryValues();
            Storage.SaveIconPositions(iconPositions, desktopName);
        }
        static void SetIcon(string desktopName)
        {
            var desktop = new Desktop();

            //var registryValues = _storage.GetRegistryValues(desktopName);
            //_registry.SetRegistryValues(registryValues);
            var iconPositions = (NamedDesktopPoint[])Storage.GetIconPositions(desktopName);
            Program.Logger.Debug("��ʼ�ָ�����ͼ��λ��: " + desktopName);

            desktop.SetIconPositions(iconPositions);

            Desktop.Refresh();
        }
        //��ʼ��,��������ļ�
        static void InitConf(Config config)
        {
            config.LoadConfig();
            if (config.DebugMode)
            {
                AllocConsole();
                Logger.Debug("������Debug����,�����������ļ��ｫ[DebugMode]���Ը�Ϊfalse�رոô��ڵ���ʾ.");
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
                if (config.DebugMode) { System.Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {msg}"); }
            }
        }
    }
}