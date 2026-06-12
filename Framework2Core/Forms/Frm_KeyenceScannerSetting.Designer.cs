
namespace Framework2Core
{
    partial class Frm_KeyenceScannerSetting
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
            this.lb_connectStus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Port = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_Ip = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bt_OpenScanner = new Sunny.UI.UIButton();
            this.sw_isScan = new HZH_Controls.Controls.UCSwitch();
            this.label5 = new System.Windows.Forms.Label();
            this.txb_MaxLength = new System.Windows.Forms.TextBox();
            this.bt_SaveParam = new Sunny.UI.UIButton();
            this.label6 = new System.Windows.Forms.Label();
            this.txb_TimeOut = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.uiButton3 = new Sunny.UI.UIButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bt_minimum = new System.Windows.Forms.Button();
            this.lbl_title = new System.Windows.Forms.Label();
            this.bt_Closed = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cbx_ListScanner = new System.Windows.Forms.ComboBox();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_connectStus
            // 
            this.lb_connectStus.AutoSize = true;
            this.lb_connectStus.BackColor = System.Drawing.Color.Red;
            this.lb_connectStus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_connectStus.Location = new System.Drawing.Point(418, 127);
            this.lb_connectStus.Name = "lb_connectStus";
            this.lb_connectStus.Size = new System.Drawing.Size(56, 16);
            this.lb_connectStus.TabIndex = 78;
            this.lb_connectStus.Text = "未连接";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(335, 127);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 16);
            this.label1.TabIndex = 77;
            this.label1.Text = "连接状态：";
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(236, 124);
            this.txt_Port.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_Port.Multiline = true;
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.Size = new System.Drawing.Size(63, 22);
            this.txt_Port.TabIndex = 76;
            this.txt_Port.Text = "9000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(189, 126);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 16);
            this.label4.TabIndex = 75;
            this.label4.Text = "端口:";
            // 
            // txt_Ip
            // 
            this.txt_Ip.Location = new System.Drawing.Point(45, 124);
            this.txt_Ip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_Ip.Multiline = true;
            this.txt_Ip.Name = "txt_Ip";
            this.txt_Ip.Size = new System.Drawing.Size(127, 22);
            this.txt_Ip.TabIndex = 74;
            this.txt_Ip.Text = "192.168.100.100";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(10, 126);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 16);
            this.label3.TabIndex = 73;
            this.label3.Text = "IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(11, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 16);
            this.label2.TabIndex = 80;
            this.label2.Text = "扫码枪选择:";
            // 
            // bt_OpenScanner
            // 
            this.bt_OpenScanner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_OpenScanner.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.bt_OpenScanner.Location = new System.Drawing.Point(434, 182);
            this.bt_OpenScanner.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bt_OpenScanner.MinimumSize = new System.Drawing.Size(1, 1);
            this.bt_OpenScanner.Name = "bt_OpenScanner";
            this.bt_OpenScanner.Size = new System.Drawing.Size(70, 28);
            this.bt_OpenScanner.TabIndex = 81;
            this.bt_OpenScanner.Text = "开启连接";
            this.bt_OpenScanner.Click += new System.EventHandler(this.bt_OpenScanner_Click);
            // 
            // sw_isScan
            // 
            this.sw_isScan.BackColor = System.Drawing.Color.Transparent;
            this.sw_isScan.Checked = true;
            this.sw_isScan.FalseColor = System.Drawing.Color.Red;
            this.sw_isScan.FalseTextColr = System.Drawing.Color.White;
            this.sw_isScan.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.sw_isScan.Location = new System.Drawing.Point(410, 53);
            this.sw_isScan.Name = "sw_isScan";
            this.sw_isScan.Size = new System.Drawing.Size(91, 27);
            this.sw_isScan.SwitchType = HZH_Controls.Controls.SwitchType.Ellipse;
            this.sw_isScan.TabIndex = 82;
            this.sw_isScan.Texts = new string[] {
        "开启扫码    ",
        "屏蔽扫码    "};
            this.sw_isScan.TrueColor = System.Drawing.Color.LimeGreen;
            this.sw_isScan.TrueTextColr = System.Drawing.Color.White;
            this.sw_isScan.CheckedChanged += new System.EventHandler(this.sw_isScan_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(22, 179);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 16);
            this.label5.TabIndex = 73;
            this.label5.Text = "条码缓存最大长度:";
            // 
            // txb_MaxLength
            // 
            this.txb_MaxLength.Location = new System.Drawing.Point(172, 176);
            this.txb_MaxLength.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txb_MaxLength.Multiline = true;
            this.txb_MaxLength.Name = "txb_MaxLength";
            this.txb_MaxLength.Size = new System.Drawing.Size(63, 22);
            this.txb_MaxLength.TabIndex = 76;
            this.txb_MaxLength.Text = "100";
            // 
            // bt_SaveParam
            // 
            this.bt_SaveParam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_SaveParam.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bt_SaveParam.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.bt_SaveParam.Location = new System.Drawing.Point(434, 222);
            this.bt_SaveParam.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bt_SaveParam.MinimumSize = new System.Drawing.Size(1, 1);
            this.bt_SaveParam.Name = "bt_SaveParam";
            this.bt_SaveParam.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bt_SaveParam.Size = new System.Drawing.Size(70, 28);
            this.bt_SaveParam.Style = Sunny.UI.UIStyle.Custom;
            this.bt_SaveParam.TabIndex = 81;
            this.bt_SaveParam.Text = "保存参数";
            this.bt_SaveParam.Click += new System.EventHandler(this.bt_SaveParam_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(58, 211);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 16);
            this.label6.TabIndex = 73;
            this.label6.Text = "TimeOut(ms):";
            // 
            // txb_TimeOut
            // 
            this.txb_TimeOut.Location = new System.Drawing.Point(172, 211);
            this.txb_TimeOut.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txb_TimeOut.Multiline = true;
            this.txb_TimeOut.Name = "txb_TimeOut";
            this.txb_TimeOut.Size = new System.Drawing.Size(63, 22);
            this.txb_TimeOut.TabIndex = 76;
            this.txb_TimeOut.Text = "1000";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 271);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(387, 26);
            this.textBox1.TabIndex = 74;
            // 
            // uiButton3
            // 
            this.uiButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton3.FillColor = System.Drawing.SystemColors.ControlDarkDark;
            this.uiButton3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.uiButton3.Location = new System.Drawing.Point(434, 271);
            this.uiButton3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.uiButton3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton3.Name = "uiButton3";
            this.uiButton3.Size = new System.Drawing.Size(58, 25);
            this.uiButton3.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton3.TabIndex = 81;
            this.uiButton3.Text = "触发一次";
            this.uiButton3.Click += new System.EventHandler(this.uiButton3_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.panel2.Controls.Add(this.bt_minimum);
            this.panel2.Controls.Add(this.lbl_title);
            this.panel2.Controls.Add(this.bt_Closed);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(515, 28);
            this.panel2.TabIndex = 201;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Set_MouseDown);
            // 
            // bt_minimum
            // 
            this.bt_minimum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_minimum.BackColor = System.Drawing.Color.Transparent;
            this.bt_minimum.FlatAppearance.BorderSize = 0;
            this.bt_minimum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_minimum.Font = new System.Drawing.Font("新宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_minimum.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.bt_minimum.Location = new System.Drawing.Point(466, 5);
            this.bt_minimum.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bt_minimum.Name = "bt_minimum";
            this.bt_minimum.Size = new System.Drawing.Size(19, 16);
            this.bt_minimum.TabIndex = 14;
            this.bt_minimum.TabStop = false;
            this.bt_minimum.Text = "-";
            this.bt_minimum.UseVisualStyleBackColor = false;
            this.bt_minimum.Click += new System.EventHandler(this.bt_minimum_Click);
            // 
            // lbl_title
            // 
            this.lbl_title.AutoSize = true;
            this.lbl_title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_title.ForeColor = System.Drawing.Color.White;
            this.lbl_title.Location = new System.Drawing.Point(2, 6);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(104, 17);
            this.lbl_title.TabIndex = 1;
            this.lbl_title.Text = "基恩士扫码枪设置";
            // 
            // bt_Closed
            // 
            this.bt_Closed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_Closed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(106)))), ((int)(((byte)(175)))));
            this.bt_Closed.FlatAppearance.BorderSize = 0;
            this.bt_Closed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_Closed.Font = new System.Drawing.Font("新宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_Closed.ForeColor = System.Drawing.Color.White;
            this.bt_Closed.Location = new System.Drawing.Point(489, 4);
            this.bt_Closed.Name = "bt_Closed";
            this.bt_Closed.Size = new System.Drawing.Size(25, 22);
            this.bt_Closed.TabIndex = 3;
            this.bt_Closed.TabStop = false;
            this.bt_Closed.Text = "×";
            this.bt_Closed.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_Closed.UseVisualStyleBackColor = false;
            this.bt_Closed.Click += new System.EventHandler(this.bt_Closed_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.panel1.Location = new System.Drawing.Point(0, 259);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 2);
            this.panel1.TabIndex = 225;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.panel3.Location = new System.Drawing.Point(0, 101);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(512, 2);
            this.panel3.TabIndex = 225;
            // 
            // cbx_ListScanner
            // 
            this.cbx_ListScanner.Font = new System.Drawing.Font("宋体", 11F);
            this.cbx_ListScanner.FormattingEnabled = true;
            this.cbx_ListScanner.Location = new System.Drawing.Point(110, 56);
            this.cbx_ListScanner.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbx_ListScanner.Name = "cbx_ListScanner";
            this.cbx_ListScanner.Size = new System.Drawing.Size(127, 23);
            this.cbx_ListScanner.TabIndex = 226;
            this.cbx_ListScanner.SelectedIndexChanged += new System.EventHandler(this.cbx_ListScanner_SelectedIndexChanged);
            // 
            // Frm_KeyenceScannerSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(515, 304);
            this.Controls.Add(this.cbx_ListScanner);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.sw_isScan);
            this.Controls.Add(this.bt_SaveParam);
            this.Controls.Add(this.uiButton3);
            this.Controls.Add(this.bt_OpenScanner);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lb_connectStus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txb_TimeOut);
            this.Controls.Add(this.txb_MaxLength);
            this.Controls.Add(this.txt_Port);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txt_Ip);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Frm_KeyenceScannerSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "基恩士扫码枪设置";
            this.Load += new System.EventHandler(this.KeyenceScannerSetting_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_connectStus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Port;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_Ip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private Sunny.UI.UIButton bt_OpenScanner;
        private HZH_Controls.Controls.UCSwitch sw_isScan;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txb_MaxLength;
        private Sunny.UI.UIButton bt_SaveParam;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txb_TimeOut;
        private System.Windows.Forms.TextBox textBox1;
        private Sunny.UI.UIButton uiButton3;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label lbl_title;
        private System.Windows.Forms.Button bt_Closed;
        private System.Windows.Forms.Button bt_minimum;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.ComboBox cbx_ListScanner;
    }
}