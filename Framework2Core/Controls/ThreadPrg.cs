using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core {
    /// <summary>
    /// 自定义控件：以进度条形式显示各后台线程的运行进度及状态。可支持最多 7 个线程
    /// </summary>
    public partial class ThreadPrg : UserControl {

        #region 1. 字段

        // 字段：控件数组，分别引用线程编号、进度和状态
        private Label[] _nums;
        private ProgressBar[] _prgs;
        private Label[] _states;

        // 字段：关联的工位数量
        private int _threadCount;

        // 属性：控件对应的线程名称集合
        public List<string> 线程名称 { get; set; }

        #endregion


        #region 2. 构造函数

        // 实例构造函数。在父窗体构造函数中的 InitializeComponent 方法中调用 
        public ThreadPrg() {
            InitializeComponent(); //初始化控件界面，VS自动生成的代码 

            // 使用控件数组，引用一系列控件
            _nums = new Label[] { lbl0, lbl1, lbl2, lbl3, lbl4, lbl5, lbl6, lbl7, lbl8, lbl9 };
            _prgs = new ProgressBar[] { prg0, prg1, prg2, prg3, prg4, prg5, prg6, prg7,prg8, prg9 };
            _states = new Label[] { lblState0, lblState1, lblState2, lblState3, lblState4, lblState5, lblState6, lblState7, lblState8, lblState9 };
        }

        #endregion


        #region 3. 加载配置：从 MainForm 中同步线程名称的数量，并更新界面

        /// <summary>
        /// 方法：控件添加到窗体后，从 MainForm 中同步线程名称的数量，并更新界面。
        /// 此方法要在父窗体构造函数的 InitializeComponent 方法之后使用
        /// </summary>
        public void UpdateThreadList(List<string> listThreads) {

            线程名称 = listThreads; //使用传入的线程列表

            // 至少有一个线程
            if (线程名称 == null || 线程名称.Count < 1) {
                线程名称 = new List<string>() { "线程" };
            }

            //同步线程数量：移除多余的 Label 和 Progressbar   
            _threadCount = 线程名称.Count;// 线程数量
            int rowCount = tableThreads.RowCount; //当前 TableLayoutPanel的行数
            for (int i = _threadCount; i < rowCount; i++) {
                tableThreads.Controls.Remove(_nums[i]);
                tableThreads.Controls.Remove(_prgs[i]);
                tableThreads.Controls.Remove(_states[i]);

                _nums[i].Dispose();
                _prgs[i].Dispose();
                _states[i].Dispose();
            }

            //根据线程数量，重新调整各控件在表格中的位置
            switch (_threadCount) {

                // 1个线程：置于最中间一行，即第 3 行
                case 1: {
                        SetControlRow(0, 3); //线程0：第3行
                        break;
                    }

                // 2个线程：分别置于第 1、4 行
                case 2: {
                        SetControlRow(1, 4); //线程1：第4行
                        SetControlRow(0, 1); //线程0：第1行
                        break;
                    }

                // 3个线程：分别置于第 1、3、5 行
                case 3: {
                        SetControlRow(2, 5); //线程2：第5行
                        SetControlRow(1, 3); //线程1：第3行
                        SetControlRow(0, 1); //线程0：第1行
                        break;
                    }

                // 4个线程：分别置于第 0、2、4、6 行
                case 4: {
                        SetControlRow(3, 6); //线程3：第6行
                        SetControlRow(2, 4); //线程2：第4行
                        SetControlRow(1, 2); //线程1：第2行
                        SetControlRow(0, 0); //线程0：第0行
                        break;
                    }

                // 5个线程：分别置于第 1、2、3、4、5 行
                case 5: {
                        SetControlRow(4, 5); //线程4：第5行
                        SetControlRow(3, 4); //线程3：第4行
                        SetControlRow(2, 3); //线程2：第3行
                        SetControlRow(1, 2); //线程1：第2行
                        SetControlRow(0, 1); //线程0：第1行
                        break;
                    }

                // 6、7个线程：保持默认位置不变
                default:
                    break;
            }
        }


        // 私有方法：将索引为 index 的控件，置于表格第 row 行
        private void SetControlRow(int index, int row) {

            tableThreads.SetRow(_nums[index], row);
            tableThreads.SetRow(_prgs[index], row);
            tableThreads.SetRow(_states[index], row);
        } 
        #endregion


        #region 4、主要功能：更新线程的状态和进度

        /// <summary>
        /// 主要功能：方法：更新控件关联的各线程的进度及状态
        /// </summary>
        /// <param name="threadNum">线程编号：0，1，2...</param>
        /// <param name="currentProgress">线程当前的进度 0-100。如处于等待状态，则为 -1</param>
        /// <param name="currentState">线程当前的状态或步骤</param>
        public void UpdateThreadProgress(int threadNum, int currentProgress, string currentState) {

            if (threadNum >= _threadCount) {
                return; //索引超出关联的线程数量，直接返回
            }

            // 更新线程当前状态
            var lblStatus = _states[threadNum]; //通过线程编号，找到对应的 Label
            lblStatus.Text = currentState;

            // 更新线程当前进度
            var prg = _prgs[threadNum]; //通过线程编号，找到对应的 ProgressBar              
            if (currentProgress < 0) {
                prg.Style = ProgressBarStyle.Marquee; //progress < 0 表示跑马灯
            }
            else {
                prg.Style = ProgressBarStyle.Continuous;
                prg.Value = currentProgress; //更新线程进度：0-100
            }

            // 立刻重绘控件
            lblStatus.Update(); 
            prg.Update();
        }
    #endregion

  }// class

}// namespace
