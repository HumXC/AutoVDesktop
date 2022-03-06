using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoVDesktop
{
    public partial class OptionView : Form
    {
        public OptionView()
        {
            InitializeComponent();
        }

        private void OptionView_Load(object sender, EventArgs e)
        {
            LoadConfig();
            if (Program.config.ShowNotifyIcon == false)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void OptionView_Shown(object sender, EventArgs e)
        {
            if (Program.config.DebugMode == false)
                this.Visible = false;
        }
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

        private void label6_Click(object sender, EventArgs e)
        {
            string? configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.json");
            Process p = new();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = configPath;
            p.Start();
        }

        private void openDesktopFolder_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Process p = new();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = Path.GetDirectoryName(path);
            p.Start();

        }

        private void addDesktop_Click(object sender, EventArgs e)
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

        private void delDesktop_Click(object sender, EventArgs e)
        {
            desktopList.Items.Remove(desktopList.SelectedItem);
        }

        private void saveConfig_Click(object sender, EventArgs e)
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
            var li = new List<string>();
            foreach (var desktopName in desktopList.Items)
            {
                if (!"".Equals(desktopName.ToString()))
                {
                    li.Add(desktopName.ToString());
                }

            }
            Program.config.Desktops = li.ToArray();
            //开机自启
            Microsoft.Win32.RegistryKey RKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            var appName = Environment.ProcessPath;
            if (appName != null)
            {
                var k = RKey.GetValue("AutoVDesktop");
                if (k == null)
                {
                    if (Program. config.StartWithWindows)
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

            Program.SaveConfig();
            if (!Program.config.DebugMode)
            {
                this.Visible = false;
            }

        }

        private void 选项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            System.Environment.Exit(0);
        }

        private void OptionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void showNotifyIcon_CheckedChanged(object sender, EventArgs e)
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
    }
}
