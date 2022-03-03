namespace AutoVDesktop
{
    using System.Threading;
    using WindowsDesktop;
    using System;
    using System.Runtime.InteropServices;
    using System.Text.Json;

    internal static class Program
    {

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>


        static string[]? desktops;
        private static bool locked = false;
        public static int threadID = 0;



        //����̨��

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [STAThread]
        static void Main()
        {

            AllocConsole();
            Init();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            if (desktops == null || desktops.Length == 0)
            {
                MessageBox.Show("�յ�����");
                System.Environment.Exit(0);
            }
            VirtualDesktop.Configure();
            VirtualDesktop.CurrentChanged += (_, args) =>
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string oldDesktopName = Path.GetFileName(path);
                log("�����л������¼�: " + oldDesktopName + " -> " + args.NewDesktop.Name);
                while (locked)
                {
                    log("���ȴ�");
                    Thread.Sleep(10);
                }
                log("�������߳�: " + threadID.ToString());
                new Thread(() =>
                 {
                     ChangeDesktopThread(args, threadID);
                 }).Start();
                threadID++;
            };

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        public static void ChangeDesktopThread(VirtualDesktopChangedEventArgs args, int _threadID)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string oldDesktopName = Path.GetFileName(path);
            log("�߳�: " + _threadID + " ���ڵȴ�...");
            Thread.Sleep(100);
            log("�߳�: " + _threadID + " �����ȴ�,��ʼ����");
            if (threadID != _threadID)
            {
                log("�߳�: " + _threadID + " �µ��߳�[" + threadID + "]���ڴ���,return");
                return;
            }
            if (args.NewDesktop.Name.Equals(oldDesktopName))
            {
                log("�߳�: " + _threadID + " ��ǰ����[" + oldDesktopName + "]��Ŀ��������ͬ,return");
                return;
            }


            if (desktops != null)

                foreach (var item in desktops)
                {
                    if (item.Equals(args.NewDesktop.Name))
                    {
                        log("�߳�: " + _threadID + " ��ʼִ���л���������[" + args.NewDesktop.Name + "],����,����threadID");
                        locked = true;
                        Work.Start(oldDesktopName, args.NewDesktop.Name, true);
                        threadID = 0;
                        locked = false;
                    }

                }
            log("�߳�: " + _threadID + " �߳��������,return");
        }
        static void Init()
        {
            log("��ʼ��");
            string? appFolder = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (string.IsNullOrEmpty(appFolder))
            {
                MessageBox.Show("����Ŀ¼���ִ���");
                System.Environment.Exit(0);
            }
            string? configPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (string.IsNullOrEmpty(configPath))
            {
                MessageBox.Show("��������ʱ���ִ���");
                System.Environment.Exit(0);
            }
            try
            {
                string jsonString = File.ReadAllText(Path.Combine(configPath, "config.json"));
                desktops = JsonSerializer.Deserialize<string[]>(jsonString)!;
            }
            catch (Exception)
            {
                MessageBox.Show("��������ʱ���ִ���");
                System.Environment.Exit(0);
            }
            foreach (var item in desktops)
            {
                log(item);
            }

        }

        public static void log(string str)
        {
            str = "[" + DateTime.Now.ToLongTimeString().ToString() + "] " + str;
            System.Console.WriteLine(str);
        }


    }
}