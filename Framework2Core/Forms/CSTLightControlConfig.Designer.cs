namespace Framework2Core
{
    partial class CSTLightControlConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSTLightControlConfig));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.TopTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.btnLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus0 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTypeName = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblObjectName = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblModelName = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblVisionName = new System.Windows.Forms.ToolStripStatusLabel();
            this.CenterPanel = new System.Windows.Forms.Panel();
            this.txtLightPower = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLightChannel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.TopTablePanel.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.BottomPanel.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.CenterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(26, 3);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(65, 19);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "参数设置";
            // 
            // TopTablePanel
            // 
            this.TopTablePanel.ColumnCount = 3;
            this.TopTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.TopTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.TopTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.TopTablePanel.Controls.Add(this.flowLayoutPanel2, 2, 0);
            this.TopTablePanel.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.TopTablePanel.Controls.Add(this.menuStrip1, 1, 0);
            this.TopTablePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopTablePanel.Location = new System.Drawing.Point(0, 0);
            this.TopTablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.TopTablePanel.Name = "TopTablePanel";
            this.TopTablePanel.RowCount = 1;
            this.TopTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TopTablePanel.Size = new System.Drawing.Size(500, 27);
            this.TopTablePanel.TabIndex = 4;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(166)))), ((int)(((byte)(0)))));
            this.flowLayoutPanel2.Controls.Add(this.btnClose);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(350, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(150, 27);
            this.flowLayoutPanel2.TabIndex = 7;
            this.flowLayoutPanel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.windowMove_MouseDown);
            this.flowLayoutPanel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.windowMove_MouseMove);
            this.flowLayoutPanel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.windowMove_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Location = new System.Drawing.Point(122, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(27, 27);
            this.btnClose.TabIndex = 4;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(166)))), ((int)(((byte)(0)))));
            this.flowLayoutPanel1.Controls.Add(this.pictureBox1);
            this.flowLayoutPanel1.Controls.Add(this.lblTitle);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(150, 27);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.windowMove_MouseDown);
            this.flowLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.windowMove_MouseMove);
            this.flowLayoutPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.windowMove_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad,
            this.btnSave});
            this.menuStrip1.Location = new System.Drawing.Point(150, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.menuStrip1.Size = new System.Drawing.Size(200, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // btnLoad
            // 
            this.btnLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(60)))), ((int)(((byte)(85)))));
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Padding = new System.Windows.Forms.Padding(0);
            this.btnLoad.Size = new System.Drawing.Size(60, 27);
            this.btnLoad.Text = "载入";
            this.btnLoad.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(60)))), ((int)(((byte)(85)))));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(0);
            this.btnSave.Size = new System.Drawing.Size(60, 27);
            this.btnSave.Text = "设定";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // BottomPanel
            // 
            this.BottomPanel.Controls.Add(this.statusStrip1);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(0, 310);
            this.BottomPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(500, 23);
            this.BottomPanel.TabIndex = 7;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(117)))), ((int)(((byte)(100)))));
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus0,
            this.lblTypeName,
            this.lblStatus1,
            this.lblObjectName,
            this.lblStatus2,
            this.lblModelName,
            this.lblStatus3,
            this.lblVisionName});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 9, 0);
            this.statusStrip1.Size = new System.Drawing.Size(500, 23);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus0
            // 
            this.lblStatus0.ForeColor = System.Drawing.Color.White;
            this.lblStatus0.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.lblStatus0.Name = "lblStatus0";
            this.lblStatus0.Size = new System.Drawing.Size(35, 19);
            this.lblStatus0.Text = "类型:";
            // 
            // lblTypeName
            // 
            this.lblTypeName.ForeColor = System.Drawing.Color.White;
            this.lblTypeName.Margin = new System.Windows.Forms.Padding(0, 2, 5, 2);
            this.lblTypeName.Name = "lblTypeName";
            this.lblTypeName.Size = new System.Drawing.Size(71, 19);
            this.lblTypeName.Text = "TypeName";
            // 
            // lblStatus1
            // 
            this.lblStatus1.ForeColor = System.Drawing.Color.White;
            this.lblStatus1.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.lblStatus1.Name = "lblStatus1";
            this.lblStatus1.Size = new System.Drawing.Size(47, 19);
            this.lblStatus1.Text = "对象名:";
            // 
            // lblObjectName
            // 
            this.lblObjectName.ForeColor = System.Drawing.Color.White;
            this.lblObjectName.Margin = new System.Windows.Forms.Padding(0, 2, 5, 2);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(81, 19);
            this.lblObjectName.Text = "ObjectName";
            // 
            // lblStatus2
            // 
            this.lblStatus2.ForeColor = System.Drawing.Color.White;
            this.lblStatus2.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(35, 19);
            this.lblStatus2.Text = "型号:";
            // 
            // lblModelName
            // 
            this.lblModelName.ForeColor = System.Drawing.Color.White;
            this.lblModelName.Margin = new System.Windows.Forms.Padding(0, 2, 5, 2);
            this.lblModelName.Name = "lblModelName";
            this.lblModelName.Size = new System.Drawing.Size(49, 19);
            this.lblModelName.Text = "Default";
            // 
            // lblStatus3
            // 
            this.lblStatus3.ForeColor = System.Drawing.Color.White;
            this.lblStatus3.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.lblStatus3.Name = "lblStatus3";
            this.lblStatus3.Size = new System.Drawing.Size(35, 19);
            this.lblStatus3.Text = "视觉:";
            // 
            // lblVisionName
            // 
            this.lblVisionName.ForeColor = System.Drawing.Color.White;
            this.lblVisionName.Margin = new System.Windows.Forms.Padding(0, 2, 5, 2);
            this.lblVisionName.Name = "lblVisionName";
            this.lblVisionName.Size = new System.Drawing.Size(56, 19);
            this.lblVisionName.Text = "上料定位";
            // 
            // CenterPanel
            // 
            this.CenterPanel.AutoScroll = true;
            this.CenterPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CenterPanel.Controls.Add(this.txtLightPower);
            this.CenterPanel.Controls.Add(this.label2);
            this.CenterPanel.Controls.Add(this.txtLightChannel);
            this.CenterPanel.Controls.Add(this.label1);
            this.CenterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CenterPanel.Location = new System.Drawing.Point(0, 27);
            this.CenterPanel.Margin = new System.Windows.Forms.Padding(2);
            this.CenterPanel.Name = "CenterPanel";
            this.CenterPanel.Size = new System.Drawing.Size(500, 283);
            this.CenterPanel.TabIndex = 8;
            // 
            // txtLightPower
            // 
            this.txtLightPower.Location = new System.Drawing.Point(105, 106);
            this.txtLightPower.Name = "txtLightPower";
            this.txtLightPower.Size = new System.Drawing.Size(100, 21);
            this.txtLightPower.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "亮度：";
            // 
            // txtLightChannel
            // 
            this.txtLightChannel.Location = new System.Drawing.Point(105, 53);
            this.txtLightChannel.Name = "txtLightChannel";
            this.txtLightChannel.Size = new System.Drawing.Size(100, 21);
            this.txtLightChannel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "通道号：";
            // 
            // CSTLightControlConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 333);
            this.Controls.Add(this.CenterPanel);
            this.Controls.Add(this.BottomPanel);
            this.Controls.Add(this.TopTablePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CSTLightControlConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormSetConfig";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.TopTablePanel.ResumeLayout(false);
            this.TopTablePanel.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.BottomPanel.ResumeLayout(false);
            this.BottomPanel.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.CenterPanel.ResumeLayout(false);
            this.CenterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel TopTablePanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem btnLoad;
        private System.Windows.Forms.ToolStripMenuItem btnSave;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel BottomPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus0;
        private System.Windows.Forms.ToolStripStatusLabel lblTypeName;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus1;
        private System.Windows.Forms.ToolStripStatusLabel lblObjectName;
        private System.Windows.Forms.Panel CenterPanel;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus2;
        private System.Windows.Forms.ToolStripStatusLabel lblModelName;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus3;
        private System.Windows.Forms.ToolStripStatusLabel lblVisionName;
        private System.Windows.Forms.TextBox txtLightPower;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLightChannel;
        private System.Windows.Forms.Label label1;
    }
}