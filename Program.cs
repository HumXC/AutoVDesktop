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



        //控制台绑定

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
                MessageBox.Show("空的配置");
                System.Environment.Exit(0);
            }
            VirtualDesktop.Configure();
            VirtualDesktop.CurrentChanged += (_, args) =>
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string oldDesktopName = Path.GetFileName(path);
                log("触发切换桌面事件: " + oldDesktopName + " -> " + args.NewDesktop.Name);
                while (locked)
                {
                    log("锁等待");
                    Thread.Sleep(10);
                }
                log("创建新线程: " + threadID.ToString());
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
            log("线程: " + _threadID + " 正在等待...");
            Thread.Sleep(100);
            log("线程: " + _threadID + " 结束等待,开始运行");
            if (threadID != _threadID)
            {
                log("线程: " + _threadID + " 新的线程[" + threadID + "]正在待命,return");
                return;
            }
            if (args.NewDesktop.Name.Equals(oldDesktopName))
            {
                log("线程: " + _threadID + " 当前桌面[" + oldDesktopName + "]与目标桌面相同,return");
                return;
            }


            if (desktops != null)

                foreach (var item in desktops)
                {
                    if (item.Equals(args.NewDesktop.Name))
                    {
                        log("线程: " + _threadID + " 开始执行切换桌面任务[" + args.NewDesktop.Name + "],上锁,重置threadID");
                        locked = true;
                        Work.Start(oldDesktopName, args.NewDesktop.Name, true);
                        threadID = 0;
                        locked = false;
                    }

                }
            log("线程: " + _threadID + " 线程任务完成,return");
        }
        static void Init()
        {
            log("初始化");
            string? appFolder = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (string.IsNullOrEmpty(appFolder))
            {
                MessageBox.Show("程序目录出现错误");
                System.Environment.Exit(0);
            }
            string? configPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (string.IsNullOrEmpty(configPath))
            {
                MessageBox.Show("加载配置时出现错误");
                System.Environment.Exit(0);
            }
            try
            {
                string jsonString = File.ReadAllText(Path.Combine(configPath, "config.json"));
                desktops = JsonSerializer.Deserialize<string[]>(jsonString)!;
            }
            catch (Exception)
            {
                MessageBox.Show("加载配置时出现错误");
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