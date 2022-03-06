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
            this.label1 = new System.Windows.Forms.Label();
            this.inputDesktopName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.delDesktop = new System.Windows.Forms.Button();
            this.addDesktop = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.debugMode = new System.Windows.Forms.CheckBox();
            this.saveConfig = new System.Windows.Forms.Button();
            this.showNotifyIcon = new System.Windows.Forms.CheckBox();
            this.restorerIcon = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.inputDelay = new System.Windows.Forms.TextBox();
            this.openDesktopFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.startWithWindows = new System.Windows.Forms.CheckBox();
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(109, 76);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.关于ToolStripMenuItem.Text = "关于";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // 选项ToolStripMenuItem
            // 
            this.选项ToolStripMenuItem.Name = "选项ToolStripMenuItem";
            this.选项ToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.选项ToolStripMenuItem.Text = "选项";
            this.选项ToolStripMenuItem.Click += new System.EventHandler(this.选项ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // desktopList
            // 
            this.desktopList.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.desktopList.FormattingEnabled = true;
            this.desktopList.ItemHeight = 23;
            this.desktopList.Location = new System.Drawing.Point(12, 65);
            this.desktopList.Name = "desktopList";
            this.desktopList.Size = new System.Drawing.Size(159, 280);
            this.desktopList.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12.10084F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 27);
            this.label1.TabIndex = 2;
            this.label1.Text = "已添加的桌面";
            // 
            // inputDesktopName
            // 
            this.inputDesktopName.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.inputDesktopName.Location = new System.Drawing.Point(199, 65);
            this.inputDesktopName.Name = "inputDesktopName";
            this.inputDesktopName.Size = new System.Drawing.Size(157, 29);
            this.inputDesktopName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 12.10084F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(199, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 27);
            this.label2.TabIndex = 4;
            this.label2.Text = "添加桌面";
            // 
            // delDesktop
            // 
            this.delDesktop.Location = new System.Drawing.Point(12, 361);
            this.delDesktop.Name = "delDesktop";
            this.delDesktop.Size = new System.Drawing.Size(159, 49);
            this.delDesktop.TabIndex = 5;
            this.delDesktop.Text = "删除选中";
            this.delDesktop.UseVisualStyleBackColor = true;
            this.delDesktop.Click += new System.EventHandler(this.delDesktop_Click);
            // 
            // addDesktop
            // 
            this.addDesktop.Location = new System.Drawing.Point(199, 100);
            this.addDesktop.Name = "addDesktop";
            this.addDesktop.Size = new System.Drawing.Size(159, 49);
            this.addDesktop.TabIndex = 6;
            this.addDesktop.Text = "添加";
            this.addDesktop.UseVisualStyleBackColor = true;
            this.addDesktop.Click += new System.EventHandler(this.addDesktop_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 493);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(339, 40);
            this.label3.TabIndex = 7;
            this.label3.Text = "从这里删除已经添加的桌面不会删除你的任何文件\r\n只是让此程序不再切换到那个桌面";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 12.10084F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(199, 173);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 27);
            this.label4.TabIndex = 8;
            this.label4.Text = "其他配置";
            // 
            // debugMode
            // 
            this.debugMode.AutoSize = true;
            this.debugMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.debugMode.Location = new System.Drawing.Point(199, 248);
            this.debugMode.Name = "debugMode";
            this.debugMode.Size = new System.Drawing.Size(97, 27);
            this.debugMode.TabIndex = 9;
            this.debugMode.Text = "调试模式";
            this.debugMode.UseVisualStyleBackColor = true;
            // 
            // saveConfig
            // 
            this.saveConfig.Location = new System.Drawing.Point(199, 425);
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.Size = new System.Drawing.Size(157, 49);
            this.saveConfig.TabIndex = 10;
            this.saveConfig.Text = "保存配置";
            this.saveConfig.UseVisualStyleBackColor = true;
            this.saveConfig.Click += new System.EventHandler(this.saveConfig_Click);
            // 
            // showNotifyIcon
            // 
            this.showNotifyIcon.AutoSize = true;
            this.showNotifyIcon.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.showNotifyIcon.Location = new System.Drawing.Point(199, 316);
            this.showNotifyIcon.Name = "showNotifyIcon";
            this.showNotifyIcon.Size = new System.Drawing.Size(148, 27);
            this.showNotifyIcon.TabIndex = 11;
            this.showNotifyIcon.Text = "显示通知栏图标";
            this.showNotifyIcon.UseVisualStyleBackColor = true;
            this.showNotifyIcon.CheckedChanged += new System.EventHandler(this.showNotifyIcon_CheckedChanged);
            // 
            // restorerIcon
            // 
            this.restorerIcon.AutoSize = true;
            this.restorerIcon.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.restorerIcon.Location = new System.Drawing.Point(199, 281);
            this.restorerIcon.Name = "restorerIcon";
            this.restorerIcon.Size = new System.Drawing.Size(131, 27);
            this.restorerIcon.TabIndex = 12;
            this.restorerIcon.Text = "保持图标位置";
            this.restorerIcon.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(199, 352);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 23);
            this.label5.TabIndex = 13;
            this.label5.Text = "响应延迟(ms)";
            // 
            // inputDelay
            // 
            this.inputDelay.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.inputDelay.Location = new System.Drawing.Point(199, 381);
            this.inputDelay.Name = "inputDelay";
            this.inputDelay.Size = new System.Drawing.Size(114, 29);
            this.inputDelay.TabIndex = 14;
            // 
            // openDesktopFolder
            // 
            this.openDesktopFolder.Location = new System.Drawing.Point(12, 425);
            this.openDesktopFolder.Name = "openDesktopFolder";
            this.openDesktopFolder.Size = new System.Drawing.Size(159, 49);
            this.openDesktopFolder.TabIndex = 15;
            this.openDesktopFolder.Text = "打开桌面文件夹";
            this.openDesktopFolder.UseVisualStyleBackColor = true;
            this.openDesktopFolder.Click += new System.EventHandler(this.openDesktopFolder_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(287, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 20);
            this.label6.TabIndex = 16;
            this.label6.Text = "打开文件";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // startWithWindows
            // 
            this.startWithWindows.AutoSize = true;
            this.startWithWindows.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.startWithWindows.Location = new System.Drawing.Point(199, 215);
            this.startWithWindows.Name = "startWithWindows";
            this.startWithWindows.Size = new System.Drawing.Size(97, 27);
            this.startWithWindows.TabIndex = 17;
            this.startWithWindows.Text = "开机启动";
            this.startWithWindows.UseVisualStyleBackColor = true;
            // 
            // OptionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(119F, 119F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(372, 543);
            this.Controls.Add(this.startWithWindows);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.openDesktopFolder);
            this.Controls.Add(this.inputDelay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.restorerIcon);
            this.Controls.Add(this.showNotifyIcon);
            this.Controls.Add(this.saveConfig);
            this.Controls.Add(this.debugMode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.addDesktop);
            this.Controls.Add(this.delDesktop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.inputDesktopName);
            this.Controls.Add(this.label1);
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
        private Label label1;
        private TextBox inputDesktopName;
        private Label label2;
        private Button delDesktop;
        private Button addDesktop;
        private Label label3;
        private Label label4;
        private CheckBox debugMode;
        private Button saveConfig;
        private CheckBox showNotifyIcon;
        private CheckBox restorerIcon;
        private Label label5;
        private TextBox inputDelay;
        private Button openDesktopFolder;
        private Label label6;
        private CheckBox startWithWindows;
    }
}