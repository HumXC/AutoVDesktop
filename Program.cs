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
             * ����Ŀǰ�Ѿ����ã���������һЩ������Ҫ�����
             * �� �����Ҽ�->�鿴->��ͼ����뵽���� ѡ���ʱ�������ͼ���λ�����⡣
             * ��Ϊ�ڸ�������·����ʱ�򣬻��Զ���������ͼ�꣬������ѡ���������ܳ���ĳ��ͼ�����λ���Ѿ���ռ�������������λ
             * �����Ѿ��ҵ��˽��������������ͼ�����°���λ��֮ǰ�ر����ѡ���ȡ�
             * �����\HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Bags\1\Desktop
             * FFlags
             * 
             * ��3������ѡ��
             * 1 - �Զ�����ͼ��
             * 2 - ��ͼ�����������
             * 3 - ��ʾ����ͼ��
             * 0x40201220 - ȫ��
             * 0x40200220 - 3
             * 0x40200221 - 3.1
             * 0x40200224 - 3.2
             * 0x40200225 - 3.1.2
             * 
             * ��������Լ�о��ⲻ����ѵĴ���ʽ���п���˼���ˡ�
             */

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
                                SetIcon(desktopName);
                               
                            }
                            else
                            {
                                IconsRestorer.Win32.ChangeDesktopFolder(fullNewDesktopPath);
                            }

                            Logger.Debug($"�߳�{_threadID}: ������ϣ�����...");
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
            var iconPositions = desktop.GetIconsPositions();
            Storage.SaveIconPositions(iconPositions, desktopName);
        }
        static void SetIcon(string desktopName)
        {
            var desktop = new IconsRestorer.Desktop();
            var iconPositions = (NamedDesktopPoint[])Storage.GetIconPositions(desktopName);
            Program.Logger.Debug("��ʼ�ָ�����ͼ��λ��: " + desktopName);
            desktop.SetIconPositions(iconPositions);
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