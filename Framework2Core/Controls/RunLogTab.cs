using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace Framework2Core
{
    /// <summary>
    /// 自定义控件：以文本框形式显示程序运行的日志，并可按照日期保存在指定目录下
    /// </summary>
    public partial class RunLogTab : UserControl
    {

        #region 1. 字段

        // 字段：是否完成配置加载。
        // 在控件事件中判断此值：如果还未完成配置加载，即使控件发生了改变，也不会保存配置到本地文件。
        // 防止初始化控件时，多次写入本地文件
        private bool _isConfigLoaded = false;

        //字段：线程Txt列表。通过线程编号，找到对应的 TextBox
        public List<TextBox> _listThreadTxts = new List<TextBox>();

        // 字段：关联的工位数量
        private int _threadCount;

        // 字段：内置 TabControl 控件的 Tabpage 对应的线程名称集合。由 Mainform 传入的列表给定
        public List<string> 线程名称列表;

        // 字段：内置 TabControl 控件的 Tabpage 对应的线程英文名称集合。由 Mainform 传入的列表给定
        public List<string> 线程英文名称列表;

        #endregion


        #region 2. 构造函数

        // 实例构造函数。在父窗体构造函数中的 InitializeComponent 方法中调用        
        public RunLogTab()
        {
            InitializeComponent(); //初始化控件界面，VS自动生成的代码      
            LanguageMenagement.LanguageChangeDeleg += LanguageChangeHandle;

        }

        #endregion


        #region 3. 可配置的属性、加载配置

        /// <summary>
        /// 可配置的属性：是否启用日志。默认值为 true
        /// </summary>
        [IniConfig]
        public bool 启用日志 { get; set; } = true;


        /// <summary>
        /// 可配置的属性：是否本地保存。默认值为 true
        /// </summary>
        [IniConfig]
        public bool 本地保存 { get; set; } = true;


        /// <summary>
        /// 可配置的属性：自动清空的行数。默认值为 1000。
        /// 如果清空行数 = 0，不自动清空
        /// </summary>
        [IniConfig]
        public int 清空行数 { get; set; } = 1000;


        /// <summary>
        /// 方法：控件添加到窗体后，从本地加载配置文件，并同步配置到界面。
        /// 此方法要在父窗体构造函数的 InitializeComponent 方法之后使用
        /// </summary>
        public void LoadConfigs()
        {

            // 加载本地配置：启用、保存等
            this.LoadObjConfigsFromIni(this.Name);

            // 同步配置到界面的 switch 控件
            swEnableLog.Checked = 启用日志;
            swSaveLog.Checked = 本地保存;

            this.SaveObjConfigsToIni(this.Name); //保存所有配置到本地

            _isConfigLoaded = true; //完成配置加载
        }


        /// <summary>
        /// 方法：控件添加到窗体后，从 MainForm 中同步线程名称的数量，并更新界面。
        /// 此方法要在父窗体构造函数的 InitializeComponent 方法之后使用
        /// </summary>
        public void UpdateThreadList(List<string> listThread,List<string> listEnThread = null)
        {

            线程名称列表 = listThread; //使用传入的线程列表
            线程英文名称列表 = listEnThread; //使用传入的线程英文名称列表

            // 至少有一个线程
            if (线程名称列表 == null || 线程名称列表.Count < 1)
            {
                线程名称列表 = new List<string>() { "线程" };
            }

            List<string> tempListNG = new List<string>();
            if (线程英文名称列表 != null && LanguageMenagement.language == LanguageType.English && 线程英文名称列表.Count == 线程名称列表.Count)
            {
                tempListNG = 线程英文名称列表;
            }
            else
            {
                tempListNG = 线程名称列表;
            }

            // 同步 TabPage 数量，更新线程名称，并将 TextBox 添加到列表
            _threadCount = 线程名称列表.Count(); //关联的线程数量
            int pageCount = tabThreads.TabCount; //当前 TabPage 数量
            for (int i = 0; i < pageCount; i++)
            {
                if (i < _threadCount)
                {
                    var page = tabThreads.TabPages[i];
                    string strTh = LanguageMenagement.language == LanguageType.中文 ? "线程" : "Thread";
                    page.Text = $"{strTh}{i}-{tempListNG[i]}";
                    //取 TabPage 的第一个子控件，并转换为 TextBox，并添加到列表
                    _listThreadTxts.Add(page.Controls[0] as TextBox);
                }
                else
                {
                    TabPage page = tabThreads.TabPages[_threadCount];
                    tabThreads.TabPages.RemoveAt(_threadCount); //TabPage 数量超过 threadCount，移除该 TabPage
                    page.Dispose(); //移除后释放此控件
                }
            }
        }

        #endregion


        #region 4. 主要功能：新增记录、写入本地

        /// <summary>
        /// 主要功能：方法：如果启用了日志，新增一行记录到控件中显示，并同步保存此记录到本地日志文件。
        /// 如果控件中的文本行数超过了设定值，清空控件中的文本
        /// </summary>
        /// <param name="strLog">新增的记录</param>
        /// <param name="threadNum">关联的线程编号</param>
        public void AppendLog(int threadNum, string strLog)
        {

            if (!启用日志) return; //如果禁用了日志，直接退出

            if (threadNum >= _threadCount)
            {
                return; //索引超出关联的线程数量，直接返回
            }

            TextBox txtLog = _listThreadTxts[threadNum]; //找到线程编号对应的 Textbox

            // 超过指定行数自动清空，默认500行
            // 如果清空行数 = 0，不清空
            if (清空行数 > 0 && txtLog.Lines.Count() > 清空行数)
            {
                txtLog.Clear();
            }

            // 添加到textbox中
            txtLog.Text += "> " + strLog + "\r\n"; //添加换行符
            txtLog.Select(txtLog.TextLength, 0);//光标定位到文本最后
            txtLog.ScrollToCaret();//滚动到光标处

            // 如果启用了本地保存，写日志到本地文件
            if (本地保存)
            {
                LocalLogSave.WriteRunLog(strLog);
            }
        }



        #endregion


        #region 5. 事件：点击开关，切换启用及保存

        // 事件：点击开关，启用或禁用日志
        private void swEnableLog_CheckedChanged(object sender, EventArgs e)
        {
            启用日志 = swEnableLog.Checked;

            // 完成配置加载后，如有更新，保存配置到本地
            if (_isConfigLoaded)
            {
                this.SaveObjConfigsToIni(this.Name, new string[] { "启用日志" });
            }
        }

        // 事件：点击开关，开启或关闭保存
        private void swSaveLog_CheckedChanged(object sender, EventArgs e)
        {
            本地保存 = swSaveLog.Checked;

            // 完成配置加载后，如有更新，保存配置到本地
            if (_isConfigLoaded)
            {
                this.SaveObjConfigsToIni(this.Name, new string[] { "本地保存" });
            }
        }

        #endregion


        #region 5. 事件：控件尺寸修改、或者切换 TabPage 后，更新该 TextBox 的大小

        // 事件：被选中 TabPage 中的 TextBox，大小根据 tabThreads 大小变化
        private void tabThreads_SizeChanged(object sender, EventArgs e)
        {

            var ctrl = tabThreads.SelectedTab.Controls[0];
            //ctrl.Location = new Point(0, 0); //位置  
            //大小：根据 tabThreads 大小变化：[246, 414] - [238, 384] = [8,30]
            ctrl.Size = new Size(tabThreads.Size.Width - 8, tabThreads.Size.Height - 30); //大小 
        }

        #endregion

        private void CmsOPtion_Click(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, tsmClear))
            {
                TextBox txtLog = _listThreadTxts[tabThreads.SelectedIndex];
                txtLog.Clear();
            }

        }


        #region 6语言切换事件处理器
        public void LanguageChangeHandle()
        {
            // 更新NG饼状图的图例
            List<string> tempListNG = new List<string>();
            if (线程英文名称列表 != null && LanguageMenagement.language == LanguageType.English && 线程英文名称列表.Count == 线程名称列表.Count)
            {
                tempListNG = 线程英文名称列表;
            }
            else
            {
                tempListNG = 线程名称列表;
            }
            // 同步 TabPage 数量，更新线程名称，并将 TextBox 添加到列表
            _threadCount = 线程名称列表.Count(); //关联的线程数量
            int pageCount = tabThreads.TabCount; //当前 TabPage 数量
            for (int i = 0; i < pageCount; i++)
            {
                if (i < _threadCount)
                {
                    var page = tabThreads.TabPages[i];
                    string strHead = LanguageMenagement.language == LanguageType.English ? "Thread" : "线程";
                    page.Text = $"{strHead}{i}-{tempListNG[i]}";
                    //取 TabPage 的第一个子控件，并转换为 TextBox，并添加到列表
                    _listThreadTxts.Add(page.Controls[0] as TextBox);
                }
                else
                {
                    TabPage page = tabThreads.TabPages[_threadCount];
                    tabThreads.TabPages.RemoveAt(_threadCount); //TabPage 数量超过 threadCount，移除该 TabPage
                    page.Dispose(); //移除后释放此控件
                }
            }
        }
        #endregion
    }// class

}// namespace
