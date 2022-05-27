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
                MessageBox.Show("Ӧ�ó����Ѿ��������С���", "��ʾ", MessageBoxButtons.OK);
                Environment.Exit(0);
            }

            ApplicationConfiguration.Initialize();
            InitConf(config);
            /*            VirtualDesktop.CurrentChanged += (_, args) =>
                        {
                            Logger.Debug($"�߳�{threadID}: �л�����: {args.OldDesktop.Name} -> {args.NewDesktop.Name}");
                            ThreadPool.QueueUserWorkItem((state) => { ChangeDesktop(args.NewDesktop.Name, threadID); });
                            ++threadID;
                        };*/

            VirtualDesktop.VirtualDesktop.CurrentChanged += (oldDesktop,newDesktop) =>
            {
                Logger.Debug("��������"+VirtualDesktop.VirtualDesktop.NowDesktop.Name);
            };
            VirtualDesktop.VirtualDesktop.Created += (desktop) =>
            {
                Logger.Debug("��������" + desktop.Name);
            };
            VirtualDesktop.VirtualDesktop.Removed+=(desktop) => {
                Logger.Debug("�Ƴ�����" + desktop.Name);
            };
            Application.Run(new OptionView());

        }
        static void ChangeDesktop(string desktopName, int _threadID)
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
                                IconsRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                Thread.Sleep(80 + (int)config.Delay / 10);
                                SetIcon(desktopName);
                            }
                            else
                            {
                                IconsRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
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
            Program.Logger.Debug("��ʼ�ָ�����ͼ��λ��: " + desktopName);

            desktop.SetIconPositions(iconPositions);

            IconsRestorer.Desktop.Refresh();
        }
        //��ʼ��,��������ļ�
        static void InitConf(Config config)
        {
            config.LoadConfig();
            if (config.DebugMode)
            {
                Win32.AllocConsole();
                Logger.Debug("������Debug����,�����������ļ��ｫ[DebugMode]���Ը�Ϊfalse�رոô��ڵ���ʾ.");
            }
            //��������
            var RKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (RKey == null)
            {
                MessageBox.Show("����", "����������ʧ��, �޷���ע���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var appName = Environment.ProcessPath;
            if (appName == null)
            {
                MessageBox.Show("����", "����������ʧ��, ������Ϊ null ��", MessageBoxButtons.OK, MessageBoxIcon.Error);
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