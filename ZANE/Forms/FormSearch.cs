using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Framework2Core;
using HZH_Controls.Forms;
using System.Data.SqlClient;

namespace ZANE.ViewModels
{

    /// <summary>
    /// 窗体：查询历史数据。可选择不同工位、不同方式查询，并可设置限定条件精确查找；查询结果可导出为 csv 文件
    /// </summary>
    public partial class FormSearch : Form
    {

        #region 1. 字段

        // 字段：使用控件数组，引用多个控件
        private ComboBox[] cmbColumns; //限定条件：列名
        private ComboBox[] cmbOperators; //限定条件：运算符
        private TextBox[] txtValues; //限定条件：用户输入的值

        // 字段：导出csv文件的默认目录，使用绝对路径
        private string _strInitSaveDir = Application.StartupPath + @"\DatabaseExport";

        // 字段：查询、导出状态
        private bool _isSearching = false; //正在查询中
        private bool _isExporting = false; //正在导出中
        private string _strSearchingStation = ""; //正在查询的工位
        private string _strSearchingTable = ""; //正在查询的数据库表格

        #endregion


        #region 2. 构造函数

        // 实例构造函数
        public FormSearch()
        {
            InitializeComponent(); //初始化窗体界面，VS自动生成的代码
            // 限制窗体最大尺寸，防止遮挡任务栏
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);

            this.Text = lblTitle.Text; // 更新窗口的 Text 属性

            //使用控件数组，引用限定条件的三个控件：列名、运算符、用户输入
            cmbColumns = new ComboBox[] { cmbColumn0, cmbColumn1, cmbColumn2 };
            txtValues = new TextBox[] { txtValue0, txtValue1, txtValue2 };
            cmbOperators = new ComboBox[] { cmbOperator0, cmbOperator1, cmbOperator2 };
            foreach (var cmb in cmbOperators)
            {
                cmb.Text = "="; //运算符默认为等号
            }

            //加载主界面的工位字典：《工位名, DataGrid控件》，作为 cmbStation 的下拉选项


            //cmbStation.Items.AddRange(MainForm.Singleton._dicStationGrids.Keys.ToArray()); //添加工位名









            string[] io = { "称" };


            cmbStation.Items.AddRange(io); //添加工位名


            if (cmbStation.Items.Count > 0)
            {
                cmbStation.SelectedIndex = 0; //默认选择工位1，触发 SeletedIndexChange 事件
            }

            //初始化日期控件的时间            
            dtpStart.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString()); //开始日期：默认今天 00:00:00
            dtpEnd.Value = Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString()); //结束日期：默认明天 00:00:00
            dtpShift.Value = dtpStart.Value; //白夜班：默认今天 00:00:00
            cmbShift.Text = "白班";

            //初始化 txtLatestRecord 的数字键盘
            txtLatestRecord.KeyBoardType = HZH_Controls.Controls.KeyBoardType.UCKeyBorderNum;
            txtLatestRecord.IsShowKeyboard = true;

            //创建导出csv文件的默认路径
            if (!Directory.Exists(_strInitSaveDir))
            {
                Directory.CreateDirectory(_strInitSaveDir);
            }
            MultiLanguageUtil.SetFormLanguage(this);

        }

        #endregion


        #region 4. 功能：设置查询条件

        // 功能：事件：第1步：下拉选择不同工位
        private InfoDataGrid selectedGrid = null;
        private void cmbStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果有工位被选中（-1表示无选项被选中）
            if (cmbStation.SelectedIndex > -1)
            {
                string selectedStation = cmbStation.Text;




               // selectedGrid = MainWindowViewModel.infoDataTable0;//通过字典，找到对应的 DataGrid


                selectedGrid = new InfoDataGrid();
                selectedGrid.列字典 = new Dictionary<string, string>
{
    { "时间", "A" },
    { "操作员", "B" }
};
                selectedGrid.Name = "infoDataTable0";




                lblTableName.Text = selectedGrid.Name; //DataGrid 的名字，与数据库中表格的名字一致
            }
            else
            {
                lblTableName.Text = "";
                selectedGrid = null;
            }

            //为限定条件的三个 cmb，依次添加选项
            foreach (var cmb in cmbColumns)
            {
                cmb.Items.Clear(); //清空下拉选项
              
                    cmb.Items.Add("(未启用)"); //Index=0 表示不启用此条件
                     //将控件表的列添加到cmb的下拉框中
                    if (selectedGrid != null)
                    {
                        //string[] columns = selectedGrid.ColumnList.ToArray();
                        string[] columns = selectedGrid.列字典.Keys.ToArray();
                        cmb.Items.AddRange(columns);
                    }
                
             
            }

            ResetConditions(); //重置限定条件
        }


        // 功能：事件：第2步：使用限定条件时，下拉选择列名；当未选择任何列时，旁边的 TextBox 不可用
        private void cmbCol_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = sender as ComboBox; //事件发起者：ComboBox

            int index = int.Parse(cmb.Tag.ToString()); //获取下拉框的Tag：0，1，2
            TextBox txt = txtValues[index];
            // 如果cmb未启用：-1或0
            if (cmb.SelectedIndex <= 0)
            {
                txt.Clear();
                txt.Enabled = false; //清除条件文本，用户不可输入
            }
            else
            {
                txt.Enabled = true; //用户可输入
            }
        }


        // 功能：事件：第2步：使用限定条件时，切换多条件之间的逻辑关系
        private void rdoLogic_CheckedChangeEvent(object sender, EventArgs e)
        {
            string strLogic = rdoAnd.Checked ? "AND" : "OR";
            lblSeparator1.Text = strLogic;
            lblLogic2.Text = strLogic;
        }


        // 功能：事件：第2步：重置限定条件
        private void btnResetCols_Click(object sender, EventArgs e)
        {
            ResetConditions();
        }


        // 私有方法：重置限定条件
        private void ResetConditions()
        {
            foreach (var cmb in cmbColumns)
            {
                cmb.SelectedIndex = 0; //默认值：未启用
            }
            foreach (var cmb in cmbOperators)
            {
                cmb.SelectedIndex = 0; //默认值：等号
            }
            rdoAnd.Checked = true; //默认值：AND
        }

        #endregion        


        #region 4. 主要功能：按条件查询，并统计 OK、NG

        // 主要功能：事件：按条件查询结果，并统计、显示
        private void btnSearch_Click(object sender, EventArgs e)
        {

            //1. 准备
            //判断是否正在查询或导出
            if (_isSearching)
            {
                FrmTips.ShowTipsError(this, "正在查询 [" + _strSearchingStation + "] 中，请勿重复查询！");
                return;
            }
            if (_isExporting)
            {
                FrmTips.ShowTipsError(this, "正在导出 [" + _strSearchingStation + "] 中，请等待导出结束后，再开始查询！！");
                return;
            }

            //获取被查询的工位和数据库表格
            _strSearchingStation = cmbStation.Text;
            _strSearchingTable = lblTableName.Text;
            //清空当前表格显示
            ResultDgv.Columns.Clear();
            var data = ResultDgv.DataSource as DataTable;
            if (data != null)
            {
                data.Dispose(); //释放之前的 DataTable
            }
            ResultDgv.DataSource = null; //扔掉引用的对象


            //2. 根据左侧的选项卡，生成Sql查询语句（前半部分）
            StringBuilder sb = new StringBuilder();
            switch (tabSearchConditions.SelectedIndex)
            {
                case 0:
                    { //日期
                        sb.Append("select * from [");
                        sb.Append(_strSearchingTable);
                        sb.Append("] where [时间]>='");
                        sb.Append(dtpStart.Value.ToShortDateString()); //开始日期
                        sb.Append("' and [时间]<'");
                        sb.Append(dtpEnd.Value.ToShortDateString()); //结束日期
                        sb.Append("'");
                        break;
                    }

                case 1:
                    { //白夜班
                        string startTime, endtime;
                        if (cmbShift.Text == "白班")
                        { //白班：当日8点至20点
                            startTime = dtpShift.Value.ToShortDateString() + " 08:00:00";
                            endtime = dtpShift.Value.ToShortDateString() + " 20:00:00";
                        }
                        else
                        { //夜班：当日20点至次日8点
                            startTime = dtpShift.Value.ToShortDateString() + " 20:00:00";
                            endtime = dtpShift.Value.AddDays(1).ToShortDateString() + " 08:00:00";
                        }
                        sb.Append("select * from [");
                        sb.Append(_strSearchingTable);
                        sb.Append("] where [时间]>='");
                        sb.Append(startTime); //开始日期
                        sb.Append("' and [时间]<'");
                        sb.Append(endtime); //结束日期
                        sb.Append("'");
                        break;
                    }

                case 2:
                    { //最近记录
                        string txt = txtLatestRecord.InputText.Trim();
                        int num;
                        if (int.TryParse(txt, out num))
                        {
                            sb.Append("select top ");
                            sb.Append(num);
                            sb.Append(" * from [");
                            sb.Append(_strSearchingTable);
                            sb.Append("]");
                        }
                        else
                        {
                            sb.Clear();
                            FrmTips.ShowTipsError(this, "记录数 \"" + txt + "\" 无效，请重新设置！");
                        }
                        break;
                    }

                default:
                    sb.Clear();
                    FrmTips.ShowTipsError(this, "查询条件无效，请重新设置！");
                    break;
            }


            //3. 如果前半部分查询语句不为空，根据右侧的限定条件，生成Sql查询语句（后半部分）
            if (sb.Length > 0)
            {
                List<string> conditionList = new List<string>(); //条件列表
                for (int i = 0; i < 3; i++)
                {
                    if (cmbColumns[i].SelectedIndex > 0)
                    { //条件已启用
                        string inputValue = txtValues[i].Text.Trim(); //用户输入的值
                        if (inputValue.Length > 0)
                        {
                            if (LanguageMenagement.language == LanguageType.中文)
                            {
                                // [列名] = N'值'
                                conditionList.Add("[" + cmbColumns[i].Text + "] " + cmbOperators[i].Text + " N'" + inputValue + "'");
                            }
                            else if (LanguageMenagement.language == LanguageType.English)
                            {
                                //将英文的选项转换成对应的中文列名
                                foreach (KeyValuePair<string, string> kvp in selectedGrid.列名英文字典)
                                {
                                    if (kvp.Value.Equals(cmbColumns[i].Text))
                                    {
                                        conditionList.Add("[" + kvp.Key + "] " + cmbOperators[i].Text + " N'" + inputValue + "'");
                                    }
                                }
                            }
                        }
                    }
                }
                if (conditionList.Count > 0)
                { //条件列表不为空
                    if (tabSearchConditions.SelectedIndex == 2)
                    { //查找最近的记录，sql语句后接 where (
                        sb.Append("\r\nwhere (");
                    }
                    else
                    {
                        sb.Append("\r\nand ("); //查找日期或白夜班，已经有 where 了，sql语句后接 and (
                    }
                    string separator = " " + lblSeparator1.Text + " "; //条件分隔符：AND 或 OR
                    sb.Append(string.Join(separator, conditionList)); //使用分隔符拼接各条件
                    sb.Append(")");
                }
            }


            //4. 如果查询语句不为空，查询            
            if (sb.Length > 0)
            {
                string sql = sb.ToString();

                //4.1 查询前：提示正在查询状态中
                _isSearching = true; //更新状态：正在查询
                string strTip = "第3步：查询 [" + _strSearchingStation + "] 中，请稍后...";
                lblStep3.Text = strTip;
                lblStep3.ForeColor = Color.OrangeRed;
                FrmTips.ShowTipsInfo(this, strTip);

                //4.2 查询中，使用Task开新线程                          
                Task.Run(new Action(() =>
                {
                    //查询数据库，得到DataTable
                    DataTable dt = null;
                    string ErrorMessage = "";
                    try
                    {
                        dt = DatabaseManager.ExecuteQuerySql(sql);
                        //Thread.Sleep(5000); //模拟耗时操作
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ex.Message; //提取错误信息
                    }

                    //统计查询结果：总数，OK，NG
                    ulong countTotal = (dt == null) ? 0 : (ulong)dt.Rows.Count; //总数，如果表为null，总数为0
                    ulong countNG = 0;
                    if (countTotal > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {  //遍历每一行，对NG计数
                            foreach (var item in row.ItemArray)
                            {
                                string strItem = item.ToString().ToUpper(); //每个元素，转为字符串，转为大写
                                if (strItem.StartsWith("FAIL") || strItem.StartsWith("ERROR") || strItem.StartsWith("NG"))
                                {
                                    countNG++;
                                    break; //转至下一行
                                }
                            }
                        }
                    }
                    ulong countOK = countTotal - countNG; //OK数 = 总数 - NG数

                    //4.3 查询后，使用 this.BeginInVoke 返回UI线程，更新界面
                    //不要有耗时操作，否则会造成界面假死
                    this.BeginInvoke(new Action(() =>
                    {
                        if (dt != null)
                        {
                            FrmTips.ShowTipsSuccess(this, "查询 [" + _strSearchingStation + "] ：完成");
                            ResultDgv.DataSource = dt; //将搜索结果，绑定到数据源
                            
                            if (ResultDgv.Columns.Contains("时间"))
                            {
                                ResultDgv.Columns["时间"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"; //设置时间显示格式：精确到秒
                            }
                            if (LanguageMenagement.language == LanguageType.English)
                            {
                                for (int i = 0; i <ResultDgv.Columns.Count; i++)
                                {
                                    if (selectedGrid.列名英文字典.ContainsKey(ResultDgv.Columns[i].Name))
                                    {
                                        ResultDgv.Columns[i].HeaderText = selectedGrid.列名英文字典[ResultDgv.Columns[i].Name];
                                    }
                                }
                            }
                        }
                        else
                        {
                            //在UI线程中弹窗，提示SQL查询错误
                            MessageBox.Show("查询数据库表格 [" + _strSearchingTable + "] 失败！\r\n\r\n查询SQL语句：\r\n" + sql + "\r\n\r\n异常：\r\n" + ErrorMessage, "数据库查询失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            FrmTips.ShowTipsError(this, "查询 [" + _strSearchingStation + "] ：失败，请重新设置查询条件！");
                        }

                        //更新显示统计结果
                        lblTotalCount.Text = countTotal.ToString();
                        lblOKCount.Text = countOK.ToString();
                        lblNGCount.Text = countNG.ToString();
                        //格式化字符串为百分比，乘1.0是将int转换为double
                        if (countTotal > 0)
                        {
                            lblNGPercent.Text = (countNG * 1.0 / countTotal).ToString("P2");
                            lblOKPercent.Text = (countOK * 1.0 / countTotal).ToString("P2");
                        }
                        else
                        {
                            lblNGPercent.Text = "N/A";
                            lblOKPercent.Text = "N/A";
                        }

                        //查询和统计结束，恢复                        
                        lblStep3.Text = "第3步：点击“🔍搜索”，查看结果及统计     ";
                        lblStep3.ForeColor = Color.FromArgb(41, 60, 85);
                        _isSearching = false;
                    }
                    ));//this.BeginInvoke                    
                }
                ));//Task.Run

            }//if (sb.Length > 0) 

        }

        #endregion


        #region 4. 主要功能：导出搜索结果


        // 主要功能：事件：导出搜索结果到为 csv 文件
        private void btnExport_Click(object sender, EventArgs e)
        {

            //1. 准备
            //判断是否正在查询或导出
            if (_isSearching)
            {
                FrmTips.ShowTipsError(this, "正在查询 [" + _strSearchingStation + "] 中，请等待查询结束后，再导出结果！");
                return;
            }
            if (_isExporting)
            {
                FrmTips.ShowTipsError(this, "正在导出 [" + _strSearchingStation + "] 中，请勿重复导出！");
                return;
            }
            //判断查询结果是否为空
            if (ResultDgv.Rows.Count == 0)
            {
                FrmTips.ShowTipsError(this, "查询结果为空，无法导出到Excel！");
                return;
            }


            //2. 打开保存文件对话框                    
            string strFileName = "[" + _strSearchingStation + "] 历史数据 " + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".csv"; //默认保存名：工位名+时间.csv
            string strFilePath = "";
            SaveFileDialog ofd = new SaveFileDialog()
            {
                Title = "导出到 Excel 数据表",
                Filter = "数据表 (*.csv)|*.csv",
                FileName = strFileName,
                InitialDirectory = _strInitSaveDir
            };
            DialogResult Result = ofd.ShowDialog();
            if (Result == DialogResult.OK)
            {
                strFileName = ofd.FileName; //更新文件位置
                strFilePath = System.IO.Path.GetDirectoryName(strFileName); //更新文件所在目录
            }
            else
            {
                return;
            }


            //3. 导出
            //3.1 导出前：提示正在导出状态中
            _isExporting = true; //更新状态：正在导出
            string strTip = "第3步：导出 [" + _strSearchingStation + "] 中，请稍后...";
            lblStep3.Text = strTip;
            lblStep3.ForeColor = Color.OrangeRed;
            FrmTips.ShowTipsInfo(this, strTip);

            //3.2 导出中：使用Task开新线程
            Task.Run(new Action(() =>
            {
                using (StreamWriter sw = new StreamWriter(strFileName, false, Encoding.UTF8))
                {
                    StringBuilder sb = new StringBuilder();
                    //Thread.Sleep(5000); //模拟耗时操作
                    //写表头
                    foreach (DataGridViewColumn col in ResultDgv.Columns)
                    {
                        sb.Append(col.HeaderText);
                        sb.Append(","); //分隔符：逗号
                    }
                    sw.WriteLine(sb.ToString());

                    //写每一行                
                    foreach (DataGridViewRow row in ResultDgv.Rows)
                    {
                        sb.Clear();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            sb.Append(cell.Value.ToString().Replace("\t", "").Replace("\r\n", ""));
                            sb.Append(","); //分隔符：逗号
                        }
                        sw.WriteLine(sb.ToString());
                    }
                }//using StreamWriter

                //3.3 导出后：回到UI线程，恢复页面显示
                this.BeginInvoke(new Action(() =>
                {
                    FrmTips.ShowTipsSuccess(this, "导出 [" + _strSearchingStation + "] ：完成");
                    lblStep3.Text = "第3步：点击“🔍搜索”，查看结果及统计     ";
                    lblStep3.ForeColor = Color.FromArgb(41, 60, 85);
                    _isExporting = false;

                    //打开导出文件所在目录
                    System.Diagnostics.Process.Start("explorer.exe", strFilePath);
                }
                )); //this.BeginInvoke                
            }
            ));//Task.Run

        }

        #endregion


        #region 6. 事件：窗体最大化、关闭、移动，单元格显示

        // 事件：最大化、还原窗口
        private Point lastNormalLocation; //上一次正常显示的窗体位置
        private void btnMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Location = lastNormalLocation; //使用上一次正常显示的窗体位置
            }
            else
            {
                lastNormalLocation = this.Location; //记录上一次正常显示的窗体位置
                this.WindowState = FormWindowState.Maximized;
            }
        }

        // 事件：关闭窗口，释放资源
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose(true); //关闭窗口后，释放此窗口的所有资源
            GC.Collect(); //GC回收
        }

        // 事件：DateGridView 单元格显示，PASS 用蓝色，FAIL 用红色
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //前两列是序号和时间，不判断
            if (e.ColumnIndex <= 1)
            {
                return;
            }

            DataGridViewCell CurrentCell = ResultDgv.Rows[e.RowIndex].Cells[e.ColumnIndex]; //当前单元格
            if (CurrentCell.Value != null)
            {
                string strCellValue = CurrentCell.Value.ToString().ToUpper(); //转为大写
                if (strCellValue.StartsWith("PASS"))
                {
                    CurrentCell.Style.ForeColor = Color.DodgerBlue; //成功：蓝色
                    CurrentCell.Style.SelectionForeColor = Color.DodgerBlue;
                }
                else if (strCellValue.StartsWith("FAIL") || strCellValue.StartsWith("ERROR") || strCellValue.StartsWith("NG"))
                {
                    CurrentCell.Style.ForeColor = Color.OrangeRed; //失败：红色
                    CurrentCell.Style.SelectionForeColor = Color.OrangeRed;
                }
                else
                {
                    CurrentCell.Style.ForeColor = Color.DimGray; //普通单元格
                    CurrentCell.Style.SelectionForeColor = Color.White;
                }
            }

        }

        #region 事件：窗体移动
        // 事件：鼠标按下
        private bool isLeftMouseDown = false; // 鼠标左键是否按下        
        private Point mPoint; //记录鼠标按下位置
        private void windowMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            { // 判断鼠标按键
                isLeftMouseDown = true;
                mPoint = new Point(e.X, e.Y); // 鼠标按下位置
            }
        }

        // 事件：鼠标移动
        private void windowMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftMouseDown)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        // 事件：鼠标释放
        private void windowMove_MouseUp(object sender, MouseEventArgs e)
        {
            isLeftMouseDown = false;
        }

        // 事件：鼠标离开窗口
        private void FormSearch_MouseLeave(object sender, EventArgs e)
        {
            isLeftMouseDown = false;
        }

        #endregion

        #endregion

    }// class

}// namespace
