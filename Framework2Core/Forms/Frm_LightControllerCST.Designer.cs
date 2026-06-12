namespace Framework2Core
{
    partial class Frm_LightControllerCST
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.bt_minimum = new System.Windows.Forms.Button();
            this.lbl_title = new System.Windows.Forms.Label();
            this.bt_Closed = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbx_ListLightController = new System.Windows.Forms.ComboBox();
            this.bt_OpenScanner = new Sunny.UI.UIButton();
            this.lb_connectStus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.brightness0 = new System.Windows.Forms.NumericUpDown();
            this.brightness1 = new System.Windows.Forms.NumericUpDown();
            this.brightness2 = new System.Windows.Forms.NumericUpDown();
            this.brightness3 = new System.Windows.Forms.NumericUpDown();
            this.brightness4 = new System.Windows.Forms.NumericUpDown();
            this.brightness5 = new System.Windows.Forms.NumericUpDown();
            this.brightness6 = new System.Windows.Forms.NumericUpDown();
            this.brightness7 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.bt_SaveParam = new Sunny.UI.UIButton();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiButton2 = new Sunny.UI.UIButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.Updown_channels = new System.Windows.Forms.NumericUpDown();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brightness0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Updown_channels)).BeginInit();
            this.SuspendLayout();
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
            this.panel2.Size = new System.Drawing.Size(569, 28);
            this.panel2.TabIndex = 202;
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
            this.bt_minimum.Location = new System.Drawing.Point(520, 5);
            this.bt_minimum.Margin = new System.Windows.Forms.Padding(2);
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
            this.lbl_title.Size = new System.Drawing.Size(90, 17);
            this.lbl_title.TabIndex = 1;
            this.lbl_title.Text = "CST光源控制器";
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
            this.bt_Closed.Location = new System.Drawing.Point(543, 4);
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
            this.panel1.Location = new System.Drawing.Point(2, 79);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(566, 2);
            this.panel1.TabIndex = 230;
            // 
            // cbx_ListLightController
            // 
            this.cbx_ListLightController.Font = new System.Drawing.Font("宋体", 11F);
            this.cbx_ListLightController.FormattingEnabled = true;
            this.cbx_ListLightController.Location = new System.Drawing.Point(151, 44);
            this.cbx_ListLightController.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_ListLightController.Name = "cbx_ListLightController";
            this.cbx_ListLightController.Size = new System.Drawing.Size(133, 23);
            this.cbx_ListLightController.TabIndex = 229;
            this.cbx_ListLightController.SelectedIndexChanged += new System.EventHandler(this.cbx_ListLightController_SelectedIndexChanged);
            // 
            // bt_OpenScanner
            // 
            this.bt_OpenScanner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_OpenScanner.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.bt_OpenScanner.Location = new System.Drawing.Point(312, 40);
            this.bt_OpenScanner.Margin = new System.Windows.Forms.Padding(2);
            this.bt_OpenScanner.MinimumSize = new System.Drawing.Size(1, 1);
            this.bt_OpenScanner.Name = "bt_OpenScanner";
            this.bt_OpenScanner.Size = new System.Drawing.Size(70, 28);
            this.bt_OpenScanner.TabIndex = 228;
            this.bt_OpenScanner.Text = "开启连接";
            this.bt_OpenScanner.Click += new System.EventHandler(this.bt_OpenScanner_Click);
            // 
            // lb_connectStus
            // 
            this.lb_connectStus.AutoSize = true;
            this.lb_connectStus.BackColor = System.Drawing.Color.Red;
            this.lb_connectStus.Font = new System.Drawing.Font("宋体", 11F);
            this.lb_connectStus.Location = new System.Drawing.Point(508, 47);
            this.lb_connectStus.Name = "lb_connectStus";
            this.lb_connectStus.Size = new System.Drawing.Size(52, 15);
            this.lb_connectStus.TabIndex = 227;
            this.lb_connectStus.Text = "未连接";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.YellowGreen;
            this.label1.Location = new System.Drawing.Point(424, 47);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 16);
            this.label1.TabIndex = 226;
            this.label1.Text = "连接状态：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.DarkKhaki;
            this.label2.Location = new System.Drawing.Point(11, 48);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 16);
            this.label2.TabIndex = 225;
            this.label2.Text = "光源控制器选择:";
            // 
            // brightness0
            // 
            this.brightness0.Location = new System.Drawing.Point(103, 166);
            this.brightness0.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness0.Name = "brightness0";
            this.brightness0.Size = new System.Drawing.Size(74, 21);
            this.brightness0.TabIndex = 231;
            this.brightness0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness0.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // brightness1
            // 
            this.brightness1.Location = new System.Drawing.Point(103, 207);
            this.brightness1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness1.Name = "brightness1";
            this.brightness1.Size = new System.Drawing.Size(74, 21);
            this.brightness1.TabIndex = 231;
            this.brightness1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness1.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // brightness2
            // 
            this.brightness2.Location = new System.Drawing.Point(103, 252);
            this.brightness2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness2.Name = "brightness2";
            this.brightness2.Size = new System.Drawing.Size(74, 21);
            this.brightness2.TabIndex = 231;
            this.brightness2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness2.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // brightness3
            // 
            this.brightness3.Location = new System.Drawing.Point(103, 292);
            this.brightness3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness3.Name = "brightness3";
            this.brightness3.Size = new System.Drawing.Size(74, 21);
            this.brightness3.TabIndex = 231;
            this.brightness3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness3.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // brightness4
            // 
            this.brightness4.Location = new System.Drawing.Point(370, 166);
            this.brightness4.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness4.Name = "brightness4";
            this.brightness4.Size = new System.Drawing.Size(74, 21);
            this.brightness4.TabIndex = 231;
            this.brightness4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness4.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // brightness5
            // 
            this.brightness5.Location = new System.Drawing.Point(370, 210);
            this.brightness5.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness5.Name = "brightness5";
            this.brightness5.Size = new System.Drawing.Size(74, 21);
            this.brightness5.TabIndex = 231;
            this.brightness5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness5.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // brightness6
            // 
            this.brightness6.Location = new System.Drawing.Point(370, 251);
            this.brightness6.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness6.Name = "brightness6";
            this.brightness6.Size = new System.Drawing.Size(74, 21);
            this.brightness6.TabIndex = 231;
            this.brightness6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness6.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // brightness7
            // 
            this.brightness7.Location = new System.Drawing.Point(370, 293);
            this.brightness7.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.brightness7.Name = "brightness7";
            this.brightness7.Size = new System.Drawing.Size(74, 21);
            this.brightness7.TabIndex = 231;
            this.brightness7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.brightness7.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(62, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 232;
            this.label3.Text = "通道数:";
            // 
            // bt_SaveParam
            // 
            this.bt_SaveParam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_SaveParam.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bt_SaveParam.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.bt_SaveParam.Location = new System.Drawing.Point(488, 336);
            this.bt_SaveParam.Margin = new System.Windows.Forms.Padding(2);
            this.bt_SaveParam.MinimumSize = new System.Drawing.Size(1, 1);
            this.bt_SaveParam.Name = "bt_SaveParam";
            this.bt_SaveParam.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bt_SaveParam.Size = new System.Drawing.Size(70, 28);
            this.bt_SaveParam.Style = Sunny.UI.UIStyle.Custom;
            this.bt_SaveParam.TabIndex = 234;
            this.bt_SaveParam.Text = "保存参数";
            this.bt_SaveParam.Click += new System.EventHandler(this.bt_SaveParam_Click);
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.FillColor = System.Drawing.Color.Green;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.uiButton1.Location = new System.Drawing.Point(327, 96);
            this.uiButton1.Margin = new System.Windows.Forms.Padding(2);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.uiButton1.Size = new System.Drawing.Size(90, 28);
            this.uiButton1.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton1.TabIndex = 234;
            this.uiButton1.Text = "一键打开";
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uiButton2
            // 
            this.uiButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.uiButton2.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.uiButton2.Location = new System.Drawing.Point(454, 96);
            this.uiButton2.Margin = new System.Windows.Forms.Padding(2);
            this.uiButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton2.Name = "uiButton2";
            this.uiButton2.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.uiButton2.Size = new System.Drawing.Size(75, 28);
            this.uiButton2.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton2.TabIndex = 234;
            this.uiButton2.Text = "一键关闭";
            this.uiButton2.Click += new System.EventHandler(this.uiButton2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(35, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 232;
            this.label4.Text = "通道1亮度:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(32, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 232;
            this.label5.Text = "通道2亮度:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(33, 256);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 232;
            this.label6.Text = "通道3亮度:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(33, 296);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 232;
            this.label7.Text = "通道4亮度:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(297, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 232;
            this.label8.Text = "通道5亮度:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(299, 214);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 232;
            this.label9.Text = "通道6亮度:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(298, 255);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 232;
            this.label10.Text = "通道7亮度:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(299, 297);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 232;
            this.label11.Text = "通道8亮度:";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.panel3.Location = new System.Drawing.Point(2, 143);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(566, 2);
            this.panel3.TabIndex = 230;
            // 
            // Updown_channels
            // 
            this.Updown_channels.Location = new System.Drawing.Point(115, 101);
            this.Updown_channels.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.Updown_channels.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Updown_channels.Name = "Updown_channels";
            this.Updown_channels.Size = new System.Drawing.Size(74, 21);
            this.Updown_channels.TabIndex = 231;
            this.Updown_channels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Updown_channels.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.Updown_channels.ValueChanged += new System.EventHandler(this.Updown_channels_ValueChanged);
            // 
            // Frm_LightControllerCST
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(569, 375);
            this.Controls.Add(this.uiButton2);
            this.Controls.Add(this.uiButton1);
            this.Controls.Add(this.bt_SaveParam);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Updown_channels);
            this.Controls.Add(this.brightness7);
            this.Controls.Add(this.brightness6);
            this.Controls.Add(this.brightness5);
            this.Controls.Add(this.brightness4);
            this.Controls.Add(this.brightness3);
            this.Controls.Add(this.brightness2);
            this.Controls.Add(this.brightness1);
            this.Controls.Add(this.brightness0);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbx_ListLightController);
            this.Controls.Add(this.bt_OpenScanner);
            this.Controls.Add(this.lb_connectStus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Frm_LightControllerCST";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frm_LightControllerCST";
            this.Load += new System.EventHandler(this.Frm_LightControllerCST_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brightness0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightness7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Updown_channels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bt_minimum;
        public System.Windows.Forms.Label lbl_title;
        private System.Windows.Forms.Button bt_Closed;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.ComboBox cbx_ListLightController;
        private Sunny.UI.UIButton bt_OpenScanner;
        private System.Windows.Forms.Label lb_connectStus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown brightness0;
        private System.Windows.Forms.NumericUpDown brightness1;
        private System.Windows.Forms.NumericUpDown brightness2;
        private System.Windows.Forms.NumericUpDown brightness3;
        private System.Windows.Forms.NumericUpDown brightness4;
        private System.Windows.Forms.NumericUpDown brightness5;
        private System.Windows.Forms.NumericUpDown brightness6;
        private System.Windows.Forms.NumericUpDown brightness7;
        private System.Windows.Forms.Label label3;
        private Sunny.UI.UIButton bt_SaveParam;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIButton uiButton2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.NumericUpDown Updown_channels;
    }
}