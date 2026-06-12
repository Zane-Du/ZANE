using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Framework2Core
{
    /// <summary>
    /// 自定义控件：以 DataGrid 形式显示产品信息。可配置 DataGrid 的列名，并将记录写入数据库的同名表格中
    /// </summary>
    public partial class InfoDataGrid : UserControl
    {

        #region 1. 普通属性

        /// <summary>
        /// 只读属性：DataGrid 的列集合。仅可在构造函数中初始化
        /// </summary>
        public List<string> ColumnList { get; } = new List<string>();

        #endregion


        #region 2. 构造函数、子对象初始化

        // 实例构造函数。在父窗体构造函数中的 InitializeComponent 方法中调用  
        public InfoDataGrid()
        {
            InitializeComponent(); //初始化控件界面，VS自动生成的代码
            InitializeMenuItem(); //初始化右键菜单：设置列宽调整方式
        }


        // 私有方法：初始化右键菜单
        private ToolStripMenuItem MenuItemSizeDispMode = new ToolStripMenuItem(); //一级菜单
        private void InitializeMenuItem()
        {
            //一级菜单
            MenuItemSizeDispMode.Text = "自动调整列宽";
            menuStrip.Items.Add(MenuItemSizeDispMode);//右键菜单的第一级

            //二级菜单
            string[] strSizeModes = typeof(DataGridViewAutoSizeColumnsMode).GetEnumNames();//获取枚举的所有可选值            
            string[] strTexts = {
                    "无", "仅列标题", "除列标题外的单元格", "所有单元格",
                    "除列标题外的已显示单元格", "已显示单元格", "全部填充"};
            for (int i = 0; i < strSizeModes.Length; ++i)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem()
                {
                    Checked = (i == 3), //默认按照所有单元格调整列宽
                    Name = strSizeModes[i],
                    Text = strTexts[i]
                };
                menuItem.Click += MenuItemDispStyle_Click; //点击事件处理器，设置列宽调整模式
                MenuItemSizeDispMode.DropDownItems.Add(menuItem);//将此选项添加到一级菜单
            }

            innerDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; //默认按照所有单元格调整列宽
        }

        #endregion


        #region 3. 可配置的属性、加载配置

        /// <summary>
        /// 可配置的属性：自动清空的行数。默认值为 2000。
        /// 如果清空行数 = 0，不自动清空
        /// </summary>
        [IniConfig]
        public int 清空行数 { get; set; } = 2000;


        /// <summary>
        /// 可配置的属性：是否将此 DataGrid 的列与数据库同步，并写入记录到数据库。默认值为 true
        /// </summary>
        [IniConfig]
        public bool 启用数据库 { get; set; } = true;


        /// <summary>
        /// 可配置的属性：列字典：《列名，列类型》，用于同步 DataGrid 及数据库的列
        /// </summary>
        [IniConfig]
        public Dictionary<string, string> 列字典 { get; set; } = new Dictionary<string, string>();

        [IniConfig]
        public Dictionary<string, string> 列名英文字典 { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 可配置的属性：主界面显示时，隐藏列
        /// </summary>
        [IniConfig]
        public List<string> 隐藏列 { get; set; } = new List<string>();


        /// <summary>
        /// 方法：控件添加到窗体后，从本地加载配置文件，并同步界面与数据库的列。
        /// 此方法要在父窗体构造函数的 InitializeComponent 方法之后使用
        /// </summary>
        public void LoadConfigs()
        {

            // 加载配置
            this.LoadObjConfigsFromIni(this.Name);

            // 如果列字典中没有 [序号]、[时间]，则添加
            if (!列字典.ContainsKey("序号"))
            {
                列字典.Add("序号", "int");
            }
            if (!列字典.ContainsKey("时间"))
            {
                列字典.Add("时间", "datetime");
            }
            // 如果列字典中没有 [序号]、[时间]，则添加
            if (!列名英文字典.ContainsKey("序号"))
            {
                列名英文字典.Add("序号", "Serial num");
            }
            if (!列名英文字典.ContainsKey("时间"))
            {
                列名英文字典.Add("时间", "Date");
            }

            // 将配置文件的列，同步到 DataGrid
            innerDgv.Columns.Clear(); //清空之前的列
            if (LanguageMenagement.language == LanguageType.English && 列名英文字典.Count >0)
            {
                foreach (KeyValuePair<string,string> kvp in 列名英文字典)
                {
                    innerDgv.Columns.Add(kvp.Key, kvp.Value); //列名，标题文本
                    ColumnList.Add(kvp.Value); //添加到列集合
                }
            }
            else
            {
                foreach (var col in 列字典.Keys)
                {
                    innerDgv.Columns.Add(col, col); //列名，标题文本
                    ColumnList.Add(col); //添加到列集合
                }
            }
            foreach (var col in 隐藏列)
            {
                innerDgv.Columns.Add(col, col); //列名，标题文本
                ColumnList.Add(col); //添加到列集合
            }

            // 禁止列排序
            foreach (DataGridViewColumn col in innerDgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // 如果指定了隐藏列，则不在界面上显示该列
            foreach (var colName in 隐藏列)
            {
                innerDgv.Columns[colName].Visible = false;
            }

            // 如果启用了数据库，将列字典同步到数据库的同名表格中
            if (启用数据库)
            {
                DatabaseManager.UpdateDatabaseColumns(this.Name, 列字典);
            }
            LanguageMenagement.LanguageChangeDeleg += LanguageChangeHandle;

            this.SaveObjConfigsToIni(this.Name); //将更新后的所有配置，保存到配置文件
        }

        #endregion


        #region 4. 主要功能：新增一行记录；清除数据

        /// <summary>
        /// 主要功能：方法：将产品信息字典添加到 DataGrid 中，并同步写入数据库的同名表格中
        /// </summary>
        /// <param name="productInfo">产品信息字典：《列名, 值》</param>
        public void AddOneRow(Dictionary<string, string> productInfo)
        {

            // DataGrid 中的记录大于指定行数，清空数据，默认2000行
            // 如果清空行数 = 0，不清空
            if (清空行数 > 0 && innerDgv.Rows.Count > 清空行数)
            {
                innerDgv.Rows.Clear();
            }

            // 如果产品信息中未包含序号，默认使用 DataGrid 的行数作为序号
            int rowIndex = innerDgv.Rows.Count; //现有行数
            if (!productInfo.ContainsKey("序号"))
            {
                productInfo.Add("序号", (rowIndex + 1).ToString());
            }

            // 如果产品信息中未包含时间，默认使用当前时间
            if (!productInfo.ContainsKey("时间"))
            {
                productInfo.Add("时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            // 将产品信息字典，按照列的顺序，转换为 List<string>
            List<string> valueList = new List<string>(); //值集合                
            foreach (var col in ColumnList)
            { //遍历列集合
                if (productInfo.ContainsKey(col))
                {
                    valueList.Add(productInfo[col]); //将产品信息字典的值，依次加入 List
                }
                else
                {
                    valueList.Add(""); //产品信息中无该列，给空
                }
            }

            // 向 DataGrid 插入新行
            innerDgv.Rows.Insert(rowIndex, valueList.ToArray()); //新增一行，现在总行数变为 rowIndex+1
            innerDgv.CurrentCell = innerDgv.Rows[rowIndex].Cells[0]; //选中新增行的第一列。行索引从0开始，Rows[n]对应第n+1行

            // 如果启用了数据库，将产品信息加入到 DatabaseManager 缓存队列中，定时往数据库里插入记录
            if (启用数据库)
            {
                DatabaseManager.EnqueueRecord(this.Name, ColumnList, valueList);//数据库的表名，列集合，值集合
            }
        }


        /// <summary>
        /// 主要功能：方法：清除 DataGrid 中的数据，但是保留列结构
        /// </summary>
        public void ClearAllRecords()
        {
            innerDgv.Rows.Clear();
        }

        #endregion


        #region 5. 事件：点击右键菜单、单元格显示

        // 事件：右击单元格后弹出菜单
        private void InnerDataGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button != MouseButtons.Right) return;     //只处理右键

            if (e.RowIndex >= 0)
            {
                innerDgv.ClearSelection();         //清除选中的其他单元格
                innerDgv.Rows[e.RowIndex].Selected = true;  //仅选中右键单击的行
            }
            menuStrip.Show(MousePosition.X, MousePosition.Y); //显示右键菜单
        }


        // 事件：点击二级菜单，设置相应的列宽调整模式
        private void MenuItemDispStyle_Click(object sender, EventArgs e)
        {

            ToolStripMenuItem selectedItem = (ToolStripMenuItem)sender; //被选中的模式
            string strSelectedMode = selectedItem.Name;
            var mode = Enum.Parse(typeof(DataGridViewAutoSizeColumnsMode), strSelectedMode); //将字符串解析为枚举值
            innerDgv.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)mode; //设置到dgv的AutoSizeColumnsMode属性

            //在被选中的选项前，打勾
            foreach (ToolStripMenuItem item in MenuItemSizeDispMode.DropDownItems)
            {
                item.Checked = (item.Name == strSelectedMode);
            }
        }


        // 事件：DataGrid 中的单元格显示：PASS用蓝色，FAIL用红色
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            //前两列是序号和时间，不判断
            if (e.ColumnIndex <= 1)
            {
                return;
            }

            DataGridViewCell CurrentCell = innerDgv.Rows[e.RowIndex].Cells[e.ColumnIndex]; //当前单元格
            if (CurrentCell.Value != null)
            {
                string strCellValue = CurrentCell.Value.ToString().ToUpper(); //转换为大写

                //成功：单元格设置为蓝色字体
                if (strCellValue.StartsWith("PASS"))
                {
                    CurrentCell.Style.ForeColor = Color.DodgerBlue;
                    CurrentCell.Style.SelectionForeColor = Color.DodgerBlue;
                }

                //失败或者超限：单元格设置为红色字体
                else if (strCellValue.StartsWith("FAIL") || strCellValue.StartsWith("ERROR") || strCellValue.StartsWith("NG") || strCellValue.EndsWith("↑") || strCellValue.EndsWith("↓"))
                {
                    CurrentCell.Style.ForeColor = Color.OrangeRed;
                    CurrentCell.Style.SelectionForeColor = Color.OrangeRed;
                }
            }
        }

        #endregion

        #region 6语言切换事件处理器
        public void LanguageChangeHandle()
        {
            if (列字典.Count < 1 || 列名英文字典.Count<1)
            {
                return;
            }
            // 如果列字典中没有 [序号]、[时间]，则添加
            if (!列名英文字典.ContainsKey("序号"))
            {
                列名英文字典.Add("序号", "Serial num");
            }
            if (!列名英文字典.ContainsKey("时间"))
            {
                列名英文字典.Add("时间", "Date");
            }
            if (LanguageMenagement.language == LanguageType.中文)
            {
                for (int i = 0; i < innerDgv.Columns.Count; i++)
                {
                    innerDgv.Columns[i].HeaderText = innerDgv.Columns[i].Name;
                }
                ColumnList.Clear();
                foreach (var col in 列字典.Keys)
                {
                    ColumnList.Add(col); //添加到列集合
                }
                foreach (var col in 隐藏列)
                {
                    ColumnList.Add(col); //添加到列集合
                }
            }
            else if(LanguageMenagement.language == LanguageType.English)
            {
                for(int i = 0;i < innerDgv.Columns.Count; i++)
                {
                    if (列名英文字典.ContainsKey(innerDgv.Columns[i].Name))
                    {
                        innerDgv.Columns[i].HeaderText = 列名英文字典[innerDgv.Columns[i].Name];
                    }
                }
                ColumnList.Clear();
                foreach (var col in 列名英文字典.Values)
                {
                    ColumnList.Add(col); //添加到列集合
                }
                foreach (var col in 隐藏列)
                {
                    ColumnList.Add(col); //添加到列集合
                }
            }
        }
        #endregion

    }//class

}//namespace
