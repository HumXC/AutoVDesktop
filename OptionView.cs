using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AutoVDesktop
{
    public partial class OptionView : Form
    {
        public OptionView()
        {
            InitializeComponent();
        }

        // 初始化窗口
        private void OptionView_Load(object sender, EventArgs e)
        {
            LoadConfig();
            if (Program.config.ShowNotifyIcon == false)
            {
                notifyIcon1.Visible = false;
            }
        }

        // 窗口初始化
        private void OptionView_Shown(object sender, EventArgs e)
        {
            if (Program.config.DebugMode == false)
                this.Hide();
        }

        // 加载配置文件数据到窗口
        private void LoadConfig()
        {
            foreach (var desktopName in Program.config.Desktops)
            {
                desktopList.Items.Add(desktopName);
            }
            startWithWindows.Checked = Program.config.StartWithWindows;
            inputDelay.Text = Program.config.Delay.ToString();
            debugMode.Checked = Program.config.DebugMode;
            restorerIcon.Checked = Program.config.RestoreIcon;
            showNotifyIcon.Checked = Program.config.ShowNotifyIcon;
        }

        private void OpenConfFile_Click(object sender, EventArgs e)
        {
            Process p = new();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = Program.config.confFileName;
            p.Start();
        }

        private void OpenDesktopFolder_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Process p = new();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = Path.GetDirectoryName(path);
            p.Start();

        }

        private void AddDesktop_Click(object sender, EventArgs e)
        {
            if ("".Equals(inputDesktopName.Text))
            {
                return;
            }
            Regex regex = new(@"[\/?*:|\\<>]");
            if (regex.IsMatch(inputDesktopName.Text))
            {
                Program.Logger.Debug("非法的文件夹名称: " + inputDesktopName.Text);
                MessageBox.Show("非法的文件夹名称: " + inputDesktopName.Text + "\n请修改后重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (var desktopName in desktopList.Items)
            {
                if (desktopName.Equals(inputDesktopName.Text))
                {
                    return;
                }
            }
            desktopList.Items.Add(inputDesktopName.Text);
            inputDesktopName.Text = "";
        }

        // 删除桌面按钮
        private void DelDesktop_Click(object sender, EventArgs e)
        {
            desktopList.Items.Remove(desktopList.SelectedItem);
        }

        // 保存配置按钮
        private void SaveConfig_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(inputDelay.Text, @"^[0-9]+$"))
            {
                MessageBox.Show($"响应延迟必须为数字，请修改后重试", "警告", MessageBoxButtons.OK);
                return;
            }
            var delay = int.Parse(inputDelay.Text);
            if (delay < 1)
            {
                MessageBox.Show("响应延迟必须是大于0的整数,配置未保存。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                inputDelay.Text = Program.config.Delay.ToString();
                return;

            }
            if (delay < 100)
            {
                MessageBox.Show("配置已保存\n但是响应延迟太小可能会导致系统不稳定。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

            Program.config.StartWithWindows = startWithWindows.Checked;
            Program.config.Delay = delay;
            Program.config.DebugMode = debugMode.Checked;
            Program.config.ShowNotifyIcon = showNotifyIcon.Checked;
            Program.config.RestoreIcon = restorerIcon.Checked;
            List<string> list = new();
            foreach (string desktopName in desktopList.Items)
            {
                if (!"".Equals(desktopName))
                {
                    list.Add(desktopName);
                }
            }
            Program.config.Desktops = list;
            Program.config.Save();

            //开机自启
            Microsoft.Win32.RegistryKey RKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            var appName = Environment.ProcessPath;
            if (appName != null)
            {
                var k = RKey.GetValue("AutoVDesktop");
                if (k == null)
                {
                    if (Program.config.StartWithWindows)
                        RKey.SetValue("AutoVDesktop", appName);
                }
                else
                {
                    if (Program.config.StartWithWindows)
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

            if (!Program.config.DebugMode)
            {
                this.Visible = false;
            }

        }

        // 手动切换到指定桌面
        private void ChangeToSelect_Click(object sender, EventArgs e)
        {
            var desktopName = changeToSelect.Text;
            if (desktopName.Equals("")) return;
            
        }

        // 以下是通知栏图标的相关内容
        private void 选项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            System.Environment.Exit(0);
        }

        private void OptionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            GC.Collect();
        }

        private void ShowNotifyIcon_CheckedChanged(object sender, EventArgs e)
        {
            if (!showNotifyIcon.Checked)
            {
                MessageBox.Show("如果取消此选项，则不会显示通知栏图标。\n如果想关闭程序需要自己打开任务管理器关闭。\n你可以手动编辑程序目录下的config.json文件，将ShowNotifyIcon属性改成true开启此选项。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Info().Show();
        }

        private void startWithWindows_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void restorerIcon_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void debugMode_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
