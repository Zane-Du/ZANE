namespace Framework2Core {
    partial class ChartPieNG {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 14D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 10D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 23D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 20D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 24D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 13D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint7 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 5D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint8 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint9 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
      System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint10 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
      this.panel1 = new System.Windows.Forms.Panel();
      this.label2 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.chartPie = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.panel1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.chartPie)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.White;
      this.panel1.Controls.Add(this.label2);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Margin = new System.Windows.Forms.Padding(0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(331, 34);
      this.panel1.TabIndex = 9;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(60)))), ((int)(((byte)(85)))));
      this.label2.Location = new System.Drawing.Point(0, 9);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(144, 20);
      this.label2.TabIndex = 2;
      this.label2.Text = "所有工位：不良分类";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.chartPie, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(331, 387);
      this.tableLayoutPanel1.TabIndex = 2;
      // 
      // chartPie
      // 
      this.chartPie.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
      chartArea1.BackColor = System.Drawing.Color.Transparent;
      chartArea1.Name = "ChartArea1";
      chartArea1.Position.Auto = false;
      chartArea1.Position.Height = 80F;
      chartArea1.Position.Width = 80F;
      chartArea1.Position.X = 10F;
      chartArea1.Position.Y = 20F;
      this.chartPie.ChartAreas.Add(chartArea1);
      this.chartPie.Dock = System.Windows.Forms.DockStyle.Fill;
      legend1.AutoFitMinFontSize = 6;
      legend1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
      legend1.Name = "Legend1";
      this.chartPie.Legends.Add(legend1);
      this.chartPie.Location = new System.Drawing.Point(4, 34);
      this.chartPie.Margin = new System.Windows.Forms.Padding(4, 0, 0, 4);
      this.chartPie.Name = "chartPie";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
      series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
      series1.Legend = "Legend1";
      series1.Name = "Series1";
      dataPoint1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(92)))));
      dataPoint1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataPoint1.Label = "#PERCENT{P1}";
      dataPoint1.LabelForeColor = System.Drawing.Color.White;
      dataPoint1.LegendText = "NG分类0:#VALY";
      dataPoint2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(159)))), ((int)(((byte)(127)))));
      dataPoint2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataPoint2.Label = "#PERCENT{P1}";
      dataPoint2.LabelForeColor = System.Drawing.Color.White;
      dataPoint2.LegendText = "NG分类1:#VALY";
      dataPoint3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(162)))), ((int)(((byte)(218)))));
      dataPoint3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataPoint3.Label = "#PERCENT{P1}";
      dataPoint3.LabelForeColor = System.Drawing.Color.White;
      dataPoint3.LegendText = "NG分类2:#VALY";
      dataPoint4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(197)))), ((int)(((byte)(233)))));
      dataPoint4.Font = new System.Drawing.Font("微软雅黑", 9F);
      dataPoint4.Label = "#PERCENT{P1}";
      dataPoint4.LabelForeColor = System.Drawing.Color.White;
      dataPoint4.LegendText = "NG分类3:#VALY";
      dataPoint5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(224)))), ((int)(((byte)(227)))));
      dataPoint5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataPoint5.Label = "#PERCENT{P1}";
      dataPoint5.LabelForeColor = System.Drawing.Color.White;
      dataPoint5.LegendText = "NG分类4:#VALY";
      dataPoint6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(230)))), ((int)(((byte)(184)))));
      dataPoint6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataPoint6.Label = "#PERCENT{P1}";
      dataPoint6.LabelForeColor = System.Drawing.Color.White;
      dataPoint6.LegendText = "NG分类5:#VALY";
      dataPoint7.Color = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(112)))), ((int)(((byte)(198)))));
      dataPoint7.Font = new System.Drawing.Font("微软雅黑", 9F);
      dataPoint7.Label = "#PERCENT{P1}";
      dataPoint7.LabelForeColor = System.Drawing.Color.White;
      dataPoint7.LegendText = "NG分类6:#VALY";
      dataPoint7.MarkerSize = 5;
      series1.Points.Add(dataPoint1);
      series1.Points.Add(dataPoint2);
      series1.Points.Add(dataPoint3);
      series1.Points.Add(dataPoint4);
      series1.Points.Add(dataPoint5);
      series1.Points.Add(dataPoint6);
      series1.Points.Add(dataPoint7);
      series1.Points.Add(dataPoint8);
      series1.Points.Add(dataPoint9);
      series1.Points.Add(dataPoint10);
      this.chartPie.Series.Add(series1);
      this.chartPie.Size = new System.Drawing.Size(327, 349);
      this.chartPie.TabIndex = 10;
      this.chartPie.Text = "chart2";
      // 
      // ChartPieNG
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Font = new System.Drawing.Font("微软雅黑", 9F);
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.Name = "ChartPieNG";
      this.Size = new System.Drawing.Size(331, 387);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.chartPie)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPie;
    }
}
