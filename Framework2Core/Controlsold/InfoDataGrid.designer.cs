namespace Framework2Core {
    partial class InfoDataGrid
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.innerDgv = new System.Windows.Forms.DataGridView();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.innerDgv)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Name = "ContextMenuStrip";
            this.menuStrip.Size = new System.Drawing.Size(61, 4);
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
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.innerDgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.innerDgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.innerDgv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
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
            this.序号,
            this.时间});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.innerDgv.DefaultCellStyle = dataGridViewCellStyle3;
            this.innerDgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.innerDgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.innerDgv.GridColor = System.Drawing.SystemColors.Control;
            this.innerDgv.Location = new System.Drawing.Point(0, 0);
            this.innerDgv.Margin = new System.Windows.Forms.Padding(0);
            this.innerDgv.Name = "innerDgv";
            this.innerDgv.RowHeadersVisible = false;
            this.innerDgv.RowHeadersWidth = 62;
            this.innerDgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.innerDgv.Size = new System.Drawing.Size(202, 231);
            this.innerDgv.TabIndex = 1;
            this.innerDgv.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DataGridView_CellFormatting);
            this.innerDgv.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.InnerDataGridView_CellMouseUp);
            // 
            // 序号
            // 
            this.序号.HeaderText = "序号";
            this.序号.MinimumWidth = 8;
            this.序号.Name = "序号";
            this.序号.Width = 57;
            // 
            // 时间
            // 
            this.时间.HeaderText = "时间";
            this.时间.MinimumWidth = 8;
            this.时间.Name = "时间";
            this.时间.Width = 57;
            // 
            // InfoDataGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.innerDgv);
            this.Name = "InfoDataGrid";
            this.Size = new System.Drawing.Size(202, 231);
            ((System.ComponentModel.ISupportInitialize)(this.innerDgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip menuStrip; //右键菜单
        public System.Windows.Forms.DataGridView innerDgv; //内部的dgv
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 时间;
    }
}
