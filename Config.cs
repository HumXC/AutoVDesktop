using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AutoVDesktop
{
    internal class Config
    {
        public List<string> Desktops { get; set; } = new List<string>();
        public int Delay { get; set; } = 1000;
        public bool RestoreDesktop { get; set; } = true;
        public bool EnsureRestore { get; set; } = false;
        public bool ShowNotifyIcon { get; set; } = true;
        public bool DebugMode { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;
        public readonly string confFileName;
        public Config()
        {
            var path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (path == null)
            {
                throw new Exception("无法读取程序运行目录");
            }
            confFileName = Path.Combine(path, "config.json");

            // 检查文件夹名的合法性
            foreach (var desktopName in Desktops)
            {
                Regex regex = new(@"[\/?*:|\\<>]");
                if (regex.IsMatch(desktopName))
                {
                    Program.Logger.Debug("非法的文件夹名称: " + desktopName);
                    MessageBox.Show("非法的文件夹名称: " + desktopName + "\n请修改后重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(0);
                }
            }
            if (Delay < 1)
            {
                Delay = 1000;
            }
        }

        // 从文件加载配置, 或生成一个初始配置
        public void LoadConfig()
        {
            if (File.Exists(confFileName))
            {
                string jsonString = File.ReadAllText(confFileName);
                try
                {
                    var c = JsonSerializer.Deserialize<Config>(jsonString);
                    if (c != null)
                    {
                        this.DebugMode = c.DebugMode;
                        this.Delay = c.Delay;
                        this.StartWithWindows = c.StartWithWindows;
                        this.Desktops = c.Desktops;
                        this.EnsureRestore = c.EnsureRestore;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("配置文件解析失败, 请修正或删除后重试: \n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
            else
            {
                // 创建新的配置文件
                using (Stream s = File.OpenWrite(confFileName))
                {
                    string nowDesktopDir = Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                    Desktops.Add(nowDesktopDir);
                };
                Save();
            }
        }

        public void Save()
        {
            using (StreamWriter s = new(confFileName, false, Encoding.UTF8))
            {
                byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes<Config>(this);
                try
                {
                    s.Write(Regex.Unescape(Encoding.UTF8.GetString(jsonUtf8Bytes)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("配置文件写入失败: \n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            };
        }
        public override string ToString()
        {
            return $"{Desktops} {Delay} {RestoreDesktop} {EnsureRestore} {ShowNotifyIcon} {DebugMode}";

        }

    }
}
