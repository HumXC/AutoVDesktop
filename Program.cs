namespace AutoVDesktop
{
    using System.Threading;
    using System;
    using AutoVDesktop.DesktopRestorer;
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

        private static string dataPath = Path.Combine(Environment.CurrentDirectory, "Desktops");
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
            VirtualDesktop.VirtualDesktop.CurrentChanged += (oldDesktop, newDesktop) =>
            {
                Logger.Debug($"�߳�{threadID}: �л�����: {oldDesktop.Name} -> {newDesktop.Name}");
                ThreadPool.QueueUserWorkItem((state) => { ChangeDesktop(newDesktop.Name, threadID); });
                ++threadID;
            };
            Application.Run(new OptionView());
        }
        static void ChangeDesktop(string newDesktopName, int _threadID)
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
                string nowDesktopName = Path.GetFileName(path);
                string? desktopPath = Path.GetDirectoryName(path);
                if (newDesktopName.Equals(nowDesktopName))
                {
                    Logger.Debug($"�߳�{_threadID}: �����ж�,��Ϊ��ǰ�����Ѿ���Ŀ������...");
                    Logger.Debug($"�߳�{_threadID}: ����...");
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
                                SaveDesktop(nowDesktopName);
                                DesktopRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                DesktopRestorer.Desktop.Refresh();
                                SetDesktop(newDesktopName);
                            }
                            else
                            {
                                DesktopRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                            }

                            Logger.Debug($"�߳�{_threadID}: ������ϣ�����...");
                            return;
                        }
                    }
                Logger.Debug($"�߳�{_threadID}: ���н���,��ΪĿ������û����������...:{newDesktopName}");
                return;
            }
        }
        static DesktopRestorer.Desktop SaveDesktop(string desktopName)
        {
            var desktop = new DesktopRestorer.Desktop();
            var iconPositions = desktop.GetIconsPositions();
            Program.Logger.Debug("��ʼ��������ͼ��λ��: " + desktopName);
            Storage.SaveIconPositions(iconPositions, desktopName);
            return desktop;
        }
        static DesktopRestorer.Desktop SetDesktop(string desktopName)
        {
            var desktop = new DesktopRestorer.Desktop();
            var iconPositions = Storage.GetIconPositions(Path.Combine(dataPath, desktopName + ".xml"));
            Program.Logger.Debug("��ʼ�ָ�����ͼ��λ��: " + desktopName);
            desktop.SetIconPositions(iconPositions);
            return desktop;
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