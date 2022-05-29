namespace AutoVDesktop
{
    using System.Threading;
    using System;
    using AutoVDesktop.DesktopRestorer;
    using System.Diagnostics;
    using Microsoft.Win32;
    internal static class Program
    {

        public static Config config = new();
        private static readonly object lockObj = new();
        private static int threadID = 0;

        private static readonly string dataPath;
        static Program()
        {
            var path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (path == null)
            {
                MessageBox.Show("����", "�޷���ȡ��������Ŀ¼", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            dataPath = Path.Combine(path, "Desktops");
        }
        [STAThread]
        static void Main()
        {
            try
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
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        try
                        {
                            ChangeDesktop(newDesktop.Name, threadID);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("����", "�л�����ʱ���ִ���: \n" + e.Message + "\n" + e.StackTrace, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(1);
                        }
                    });
                    ++threadID;
                };
                Application.Run(new OptionView());
            }
            catch (Exception e)
            {
                MessageBox.Show("����", "����: \n" + e.Message + "\n" + e.StackTrace, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

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
                Logger.Debug("Desktop_Path=" + path);
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
                                var iconCount = SaveDesktop(nowDesktopName).IconCount;
                                DesktopRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                DesktopRestorer.Desktop.Refresh();
                                if (iconCount < 8) { Thread.Sleep(200); }
                                SetDesktop(newDesktopName);
                            }
                            else
                            {
                                DesktopRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                                DesktopRestorer.Desktop.Refresh();
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
            Storage.SaveIconPositions(iconPositions, Path.Combine(dataPath, desktopName + ".xml"));
            return desktop;
        }
        static DesktopRestorer.Desktop SetDesktop(string desktopName)
        {
            var desktop = new DesktopRestorer.Desktop();
            var iconPositions = Storage.GetIconPositions(Path.Combine(dataPath, desktopName + ".xml"));
            Program.Logger.Debug("��ʼ�ָ�����ͼ��λ��: " + desktopName);
            if (config.EnsureRestore)
            {
                desktop.EnsureSetIconPositions(iconPositions);
                return desktop;
            }
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
            var RKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (RKey == null)
            {
                MessageBox.Show("����", "����������ʧ��, �޷���ע���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var appName = System.Windows.Forms.Application.ExecutablePath;
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