namespace AutoVDesktop
{
    partial class OptionView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionView));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.desktopList = new System.Windows.Forms.ListBox();
            this.label_LoadedDesktop = new System.Windows.Forms.Label();
            this.inputDesktopName = new System.Windows.Forms.TextBox();
            this.label_AddDesktop = new System.Windows.Forms.Label();
            this.delDesktop = new System.Windows.Forms.Button();
            this.addDesktop = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label_OtherOptions = new System.Windows.Forms.Label();
            this.debugMode = new System.Windows.Forms.CheckBox();
            this.saveConfig = new System.Windows.Forms.Button();
            this.showNotifyIcon = new System.Windows.Forms.CheckBox();
            this.restorerIcon = new System.Windows.Forms.CheckBox();
            this.label_OtherOptions_Delay = new System.Windows.Forms.Label();
            this.inputDelay = new System.Windows.Forms.TextBox();
            this.openDesktopFolder = new System.Windows.Forms.Button();
            this.lable_OtherOptions_OpenConfFile = new System.Windows.Forms.Label();
            this.startWithWindows = new System.Windows.Forms.CheckBox();
            this.changeToSelect = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "AutoVDesktop";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem,
            this.选项ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(84, 76);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.关于ToolStripMenuItem.Text = "关于";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // 选项ToolStripMenuItem
            // 
            this.选项ToolStripMenuItem.Name = "选项ToolStripMenuItem";
            this.选项ToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.选项ToolStripMenuItem.Text = "选项";
            this.选项ToolStripMenuItem.Click += new System.EventHandler(this.选项ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // desktopList
            // 
            this.desktopList.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.desktopList.FormattingEnabled = true;
            this.desktopList.ItemHeight = 23;
            this.desktopList.Location = new System.Drawing.Point(12, 66);
            this.desktopList.Name = "desktopList";
            this.desktopList.Size = new System.Drawing.Size(160, 280);
            this.desktopList.TabIndex = 1;
            // 
            // label_LoadedDesktop
            // 
            this.label_LoadedDesktop.AutoSize = true;
            this.label_LoadedDesktop.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label_LoadedDesktop.Font = new System.Drawing.Font("Microsoft YaHei UI", 12.10084F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_LoadedDesktop.Location = new System.Drawing.Point(12, 24);
            this.label_LoadedDesktop.Name = "label_LoadedDesktop";
            this.label_LoadedDesktop.Size = new System.Drawing.Size(138, 28);
            this.label_LoadedDesktop.TabIndex = 2;
            this.label_LoadedDesktop.Text = "已添加的桌面";
            // 
            // inputDesktopName
            // 
            this.inputDesktopName.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.inputDesktopName.Location = new System.Drawing.Point(225, 65);
            this.inputDesktopName.Name = "inputDesktopName";
            this.inputDesktopName.Size = new System.Drawing.Size(161, 29);
            this.inputDesktopName.TabIndex = 3;
            // 
            // label_AddDesktop
            // 
            this.label_AddDesktop.AutoSize = true;
            this.label_AddDesktop.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label_AddDesktop.Font = new System.Drawing.Font("Microsoft YaHei UI", 12.10084F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_AddDesktop.Location = new System.Drawing.Point(225, 24);
            this.label_AddDesktop.Name = "label_AddDesktop";
            this.label_AddDesktop.Size = new System.Drawing.Size(96, 28);
            this.label_AddDesktop.TabIndex = 4;
            this.label_AddDesktop.Text = "添加桌面";
            // 
            // delDesktop
            // 
            this.delDesktop.Location = new System.Drawing.Point(225, 297);
            this.delDesktop.Name = "delDesktop";
            this.delDesktop.Size = new System.Drawing.Size(160, 49);
            this.delDesktop.TabIndex = 5;
            this.delDesktop.Text = "删除选中";
            this.delDesktop.UseVisualStyleBackColor = true;
            this.delDesktop.Click += new System.EventHandler(this.DelDesktop_Click);
            // 
            // addDesktop
            // 
            this.addDesktop.Location = new System.Drawing.Point(225, 102);
            this.addDesktop.Name = "addDesktop";
            this.addDesktop.Size = new System.Drawing.Size(160, 49);
            this.addDesktop.TabIndex = 6;
            this.addDesktop.Text = "添加";
            this.addDesktop.UseVisualStyleBackColor = true;
            this.addDesktop.Click += new System.EventHandler(this.AddDesktop_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 365);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(564, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "从这里删除已经添加的桌面不会删除你的任何文件只是让此程序不再切换到那个桌面";
            // 
            // label_OtherOptions
            // 
            this.label_OtherOptions.AutoSize = true;
            this.label_OtherOptions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label_OtherOptions.Font = new System.Drawing.Font("Microsoft YaHei UI", 12.10084F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_OtherOptions.Location = new System.Drawing.Point(438, 24);
            this.label_OtherOptions.Name = "label_OtherOptions";
            this.label_OtherOptions.Size = new System.Drawing.Size(96, 28);
            this.label_OtherOptions.TabIndex = 8;
            this.label_OtherOptions.Text = "其他配置";
            // 
            // debugMode
            // 
            this.debugMode.AutoSize = true;
            this.debugMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.debugMode.Location = new System.Drawing.Point(438, 161);
            this.debugMode.Name = "debugMode";
            this.debugMode.Size = new System.Drawing.Size(104, 28);
            this.debugMode.TabIndex = 9;
            this.debugMode.Text = "调试模式";
            this.debugMode.UseVisualStyleBackColor = true;
            this.debugMode.CheckedChanged += new System.EventHandler(this.debugMode_CheckedChanged);
            // 
            // saveConfig
            // 
            this.saveConfig.Location = new System.Drawing.Point(438, 297);
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.Size = new System.Drawing.Size(158, 49);
            this.saveConfig.TabIndex = 10;
            this.saveConfig.Text = "保存配置";
            this.saveConfig.UseVisualStyleBackColor = true;
            this.saveConfig.Click += new System.EventHandler(this.SaveConfig_Click);
            // 
            // showNotifyIcon
            // 
            this.showNotifyIcon.AutoSize = true;
            this.showNotifyIcon.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.showNotifyIcon.Location = new System.Drawing.Point(438, 230);
            this.showNotifyIcon.Name = "showNotifyIcon";
            this.showNotifyIcon.Size = new System.Drawing.Size(158, 28);
            this.showNotifyIcon.TabIndex = 11;
            this.showNotifyIcon.Text = "显示通知栏图标";
            this.showNotifyIcon.UseVisualStyleBackColor = true;
            this.showNotifyIcon.CheckedChanged += new System.EventHandler(this.ShowNotifyIcon_CheckedChanged);
            // 
            // restorerIcon
            // 
            this.restorerIcon.AutoSize = true;
            this.restorerIcon.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.restorerIcon.Location = new System.Drawing.Point(438, 194);
            this.restorerIcon.Name = "restorerIcon";
            this.restorerIcon.Size = new System.Drawing.Size(140, 28);
            this.restorerIcon.TabIndex = 12;
            this.restorerIcon.Text = "恢复图标位置";
            this.restorerIcon.UseVisualStyleBackColor = true;
            this.restorerIcon.CheckedChanged += new System.EventHandler(this.restorerIcon_CheckedChanged);
            // 
            // label_OtherOptions_Delay
            // 
            this.label_OtherOptions_Delay.AutoSize = true;
            this.label_OtherOptions_Delay.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_OtherOptions_Delay.Location = new System.Drawing.Point(434, 70);
            this.label_OtherOptions_Delay.Name = "label_OtherOptions_Delay";
            this.label_OtherOptions_Delay.Size = new System.Drawing.Size(119, 24);
            this.label_OtherOptions_Delay.TabIndex = 13;
            this.label_OtherOptions_Delay.Text = "响应延迟(ms)";
            // 
            // inputDelay
            // 
            this.inputDelay.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.inputDelay.Location = new System.Drawing.Point(438, 97);
            this.inputDelay.Name = "inputDelay";
            this.inputDelay.Size = new System.Drawing.Size(115, 29);
            this.inputDelay.TabIndex = 14;
            // 
            // openDesktopFolder
            // 
            this.openDesktopFolder.Location = new System.Drawing.Point(225, 169);
            this.openDesktopFolder.Name = "openDesktopFolder";
            this.openDesktopFolder.Size = new System.Drawing.Size(160, 49);
            this.openDesktopFolder.TabIndex = 15;
            this.openDesktopFolder.Text = "打开桌面文件夹";
            this.openDesktopFolder.UseVisualStyleBackColor = true;
            this.openDesktopFolder.Click += new System.EventHandler(this.OpenDesktopFolder_Click);
            // 
            // lable_OtherOptions_OpenConfFile
            // 
            this.lable_OtherOptions_OpenConfFile.AutoSize = true;
            this.lable_OtherOptions_OpenConfFile.Location = new System.Drawing.Point(527, 32);
            this.lable_OtherOptions_OpenConfFile.Name = "lable_OtherOptions_OpenConfFile";
            this.lable_OtherOptions_OpenConfFile.Size = new System.Drawing.Size(69, 20);
            this.lable_OtherOptions_OpenConfFile.TabIndex = 16;
            this.lable_OtherOptions_OpenConfFile.Text = "打开文件";
            this.lable_OtherOptions_OpenConfFile.Click += new System.EventHandler(this.OpenConfFile_Click);
            // 
            // startWithWindows
            // 
            this.startWithWindows.AutoSize = true;
            this.startWithWindows.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.startWithWindows.Location = new System.Drawing.Point(438, 128);
            this.startWithWindows.Name = "startWithWindows";
            this.startWithWindows.Size = new System.Drawing.Size(104, 28);
            this.startWithWindows.TabIndex = 17;
            this.startWithWindows.Text = "开机启动";
            this.startWithWindows.UseVisualStyleBackColor = true;
            this.startWithWindows.CheckedChanged += new System.EventHandler(this.startWithWindows_CheckedChanged);
            // 
            // changeToSelect
            // 
            this.changeToSelect.Location = new System.Drawing.Point(225, 234);
            this.changeToSelect.Name = "changeToSelect";
            this.changeToSelect.Size = new System.Drawing.Size(160, 49);
            this.changeToSelect.TabIndex = 18;
            this.changeToSelect.Text = "切换到选中桌面";
            this.changeToSelect.UseVisualStyleBackColor = true;
            this.changeToSelect.Click += new System.EventHandler(this.ChangeToSelect_Click);
            // 
            // OptionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(608, 393);
            this.Controls.Add(this.changeToSelect);
            this.Controls.Add(this.startWithWindows);
            this.Controls.Add(this.lable_OtherOptions_OpenConfFile);
            this.Controls.Add(this.openDesktopFolder);
            this.Controls.Add(this.inputDelay);
            this.Controls.Add(this.label_OtherOptions_Delay);
            this.Controls.Add(this.restorerIcon);
            this.Controls.Add(this.showNotifyIcon);
            this.Controls.Add(this.saveConfig);
            this.Controls.Add(this.debugMode);
            this.Controls.Add(this.label_OtherOptions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.addDesktop);
            this.Controls.Add(this.delDesktop);
            this.Controls.Add(this.label_AddDesktop);
            this.Controls.Add(this.inputDesktopName);
            this.Controls.Add(this.label_LoadedDesktop);
            this.Controls.Add(this.desktopList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoVDesktop";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionView_FormClosing);
            this.Load += new System.EventHandler(this.OptionView_Load);
            this.Shown += new System.EventHandler(this.OptionView_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private ToolStripMenuItem 选项ToolStripMenuItem;
        private ToolStripMenuItem 退出ToolStripMenuItem;
        private ListBox desktopList;
        private Label label_LoadedDesktop;
        private TextBox inputDesktopName;
        private Label label_AddDesktop;
        private Button delDesktop;
        private Button addDesktop;
        private Label label3;
        private Label label_OtherOptions;
        private CheckBox debugMode;
        private Button saveConfig;
        private CheckBox showNotifyIcon;
        private CheckBox restorerIcon;
        private Label label_OtherOptions_Delay;
        private TextBox inputDelay;
        private Button openDesktopFolder;
        private Label lable_OtherOptions_OpenConfFile;
        private CheckBox startWithWindows;
        private Button changeToSelect;
    }
}