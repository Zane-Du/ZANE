namespace Framework2Core {
    partial class PropDataGrid {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.innerDgv = new System.Windows.Forms.DataGridView();
            this.属性名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.属性值 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.属性类型 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.btnSet = new HZH_Controls.Controls.UCBtnExt();
            this.lblModelName = new System.Windows.Forms.Label();
            this.lblVisionName = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.innerDgv)).BeginInit();
            this.TopPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.innerDgv, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TopPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(334, 247);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // innerDgv
            // 
            this.innerDgv.AllowUserToAddRows = false;
            this.innerDgv.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.innerDgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.innerDgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.innerDgv.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.innerDgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.innerDgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.innerDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.innerDgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.属性名称,
            this.属性值,
            this.属性类型});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微软雅黑", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.innerDgv.DefaultCellStyle = dataGridViewCellStyle3;
            this.innerDgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.innerDgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.innerDgv.GridColor = System.Drawing.SystemColors.Control;
            this.innerDgv.Location = new System.Drawing.Point(4, 28);
            this.innerDgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.innerDgv.Name = "innerDgv";
            this.innerDgv.RowHeadersVisible = false;
            this.innerDgv.RowHeadersWidth = 62;
            this.innerDgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.innerDgv.Size = new System.Drawing.Size(326, 216);
            this.innerDgv.TabIndex = 3;
            // 
            // 属性名称
            // 
            this.属性名称.HeaderText = "属性名称";
            this.属性名称.MinimumWidth = 8;
            this.属性名称.Name = "属性名称";
            this.属性名称.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 属性值
            // 
            this.属性值.HeaderText = "属性值";
            this.属性值.MinimumWidth = 6;
            this.属性值.Name = "属性值";
            this.属性值.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 属性类型
            // 
            this.属性类型.HeaderText = "属性类型";
            this.属性类型.MinimumWidth = 8;
            this.属性类型.Name = "属性类型";
            this.属性类型.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TopPanel
            // 
            this.TopPanel.Controls.Add(this.btnSet);
            this.TopPanel.Controls.Add(this.lblModelName);
            this.TopPanel.Controls.Add(this.lblVisionName);
            this.TopPanel.Controls.Add(this.lblTitle);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Margin = new System.Windows.Forms.Padding(0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(334, 25);
            this.TopPanel.TabIndex = 4;
            // 
            // btnSet
            // 
            this.btnSet.BackColor = System.Drawing.Color.White;
            this.btnSet.BtnBackColor = System.Drawing.Color.White;
            this.btnSet.BtnFont = new System.Drawing.Font("微软雅黑", 9F);
            this.btnSet.BtnForeColor = System.Drawing.Color.White;
            this.btnSet.BtnText = "设置";
            this.btnSet.ConerRadius = 5;
            this.btnSet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSet.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSet.EnabledMouseEffect = false;
            this.btnSet.FillColor = System.Drawing.Color.Gray;
            this.btnSet.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnSet.IsRadius = true;
            this.btnSet.IsShowRect = true;
            this.btnSet.IsShowTips = false;
            this.btnSet.Location = new System.Drawing.Point(-19, 0);
            this.btnSet.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.btnSet.Name = "btnSet";
            this.btnSet.RectColor = System.Drawing.Color.Gray;
            this.btnSet.RectWidth = 1;
            this.btnSet.Size = new System.Drawing.Size(50, 25);
            this.btnSet.TabIndex = 4;
            this.btnSet.TabStop = false;
            this.btnSet.TipsColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(30)))), ((int)(((byte)(99)))));
            this.btnSet.TipsText = "";
            this.btnSet.BtnClick += new System.EventHandler(this.btnSet_BtnClick);
            // 
            // lblModelName
            // 
            this.lblModelName.AutoSize = true;
            this.lblModelName.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblModelName.Location = new System.Drawing.Point(31, 0);
            this.lblModelName.Margin = new System.Windows.Forms.Padding(0);
            this.lblModelName.Name = "lblModelName";
            this.lblModelName.Size = new System.Drawing.Size(169, 31);
            this.lblModelName.TabIndex = 3;
            this.lblModelName.Text = "型号：Default";
            this.lblModelName.Visible = false;
            // 
            // lblVisionName
            // 
            this.lblVisionName.AutoSize = true;
            this.lblVisionName.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblVisionName.Location = new System.Drawing.Point(200, 0);
            this.lblVisionName.Margin = new System.Windows.Forms.Padding(0);
            this.lblVisionName.Name = "lblVisionName";
            this.lblVisionName.Size = new System.Drawing.Size(134, 31);
            this.lblVisionName.TabIndex = 2;
            this.lblVisionName.Text = "视觉：定位";
            this.lblVisionName.Visible = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(62, 31);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "参数";
            // 
            // PropDataGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PropDataGrid";
            this.Size = new System.Drawing.Size(334, 247);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.innerDgv)).EndInit();
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.DataGridView innerDgv;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblModelName;
        private System.Windows.Forms.Label lblVisionName;
        public HZH_Controls.Controls.UCBtnExt btnSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn 属性名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 属性值;
        private System.Windows.Forms.DataGridViewTextBoxColumn 属性类型;
    }
}
