using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core
{
    /// <summary>
    /// 自定义控件：以柱状图形式显示单工位的 OK、NG、总数统计
    /// </summary>
    public partial class ChartHistCounting : UserControl
    {

        #region 1. 字段

        // 字段：各工位的 OK、NG 计数
        private int[] _countOKs, _countNGs;

        // 字段：控件对应的工位名称列表。由 Mainform 传入的列表给定
        public List<string> 工位名称列表 { get; set; }
        // 字段：控件对应的工位英文名称列表。由 Mainform 传入的列表给定
        public List<string> 工位英文名称列表 { get; set; }

        // 字段：关联的工位数量
        private int _stationCount;

        // 字段：当前显示的工位索引
        private int _currentIndex;

        #endregion


        #region 2. 构造函数

        // 实例构造函数。在父窗体构造函数中的 InitializeComponent 方法中调用
        public ChartHistCounting()
        {
            InitializeComponent(); //初始化控件界面，VS自动生成的代码 
            LanguageMenagement.LanguageChangeDeleg += LanguageChangeHandle;
        }

        #endregion


        #region 3. 加载配置，从 MainForm 中同步工位名称的数量

        /// <summary>
        /// 方法：控件添加到窗体后，从 MainForm 中同步工位名称的数量。
        /// 此方法要在父窗体构造函数的 InitializeComponent 方法之后使用
        /// </summary>
        public void UpdateStationList(List<string> listStations,List<string> listEnStations)
        {

            工位名称列表 = listStations; //使用传入的工位列表
            工位英文名称列表 = listEnStations;

            // 至少有一个工位
            if (工位名称列表 == null || 工位名称列表.Count < 1)
            {
                工位名称列表 = new List<string>() { "工位" };
            }

            // 根据工位数量初始化数组
            _stationCount = 工位名称列表.Count;
            _countOKs = new int[_stationCount];
            _countNGs = new int[_stationCount];


            for (int i = 0; i < _stationCount; i++)
            {
                //测试用：初始化各数组的值
                //_countOKs[i] = (i + 1) * 40;
                //_countNGs[i] = (i + 1) * 10;

                _countOKs[i] = 0;
                _countNGs[i] = 0;
            }

            RefreshChart(0); //默认显示工位0的数据
            LanguageChangeHandle();
        }

        #endregion


        #region 4. 主要功能：各工位 OK、NG 计数：加 1、清零

        /// <summary>
        /// 主要功能：方法：将索引为 index 的工位 OK 计数加 1
        /// </summary>
        /// <param name="index">工位索引，从 0 开始</param>
        public void AddOK(int index)
        {
            if (index < _stationCount)
            {
                _countOKs[index]++;
            }
            RefreshChart(_currentIndex); //更新显示当前工位
        }


        /// <summary>
        /// 主要功能：方法：将索引为 index 的工位 NG 计数加 1
        /// </summary>
        /// <param name="index">工位索引，从 0 开始</param>
        public void AddNG(int index)
        {
            if (index < _stationCount)
            {
                _countNGs[index]++;
            }
            RefreshChart(_currentIndex); //更新显示当前工位
        }





        /// <summary>
        /// 主要功能：方法：将所有工位 NG 计数清零
        /// </summary>
        public void ClearAllNGs()
        {
            for (int i = 0; i < _stationCount; i++)
            {
                _countNGs[i] = 0;
            }
            RefreshChart(_currentIndex); //更新显示当前工位
        }

        /// <summary>
        /// 主要功能：方法：将所有工位 NG 计数清零
        /// </summary>
        public void ClearAllOKs()
        {
            for (int i = 0; i < _stationCount; i++)
            {
                _countOKs[i] = 0;
            }
            RefreshChart(_currentIndex); //更新显示当前工位
        }

        /// <summary>
        /// 主要功能：方法：将所有工位 OK/NG 计数清零
        /// </summary>
        public void ClearAllOKNGs()
        {
            for (int i = 0; i < _stationCount; i++)
            {
                _countNGs[i] = 0;
                _countOKs[i] = 0;
            }
            RefreshChart(_currentIndex); //更新显示当前工位
        }


        #endregion


        #region 4. 主要功能：切换显示柱状图

        /// <summary>
        /// 主要功能：方法：在柱状图中，切换显示索引为 index 的工位 OK、NG 计数及总数
        /// </summary>
        /// <param name="index">工位索引，从 0 开始</param>
        public void RefreshChart(int index)
        {

            // 防止数组越界
            if (index >= _stationCount)
            {
                return;
            }

            //更新选中的工位名
            _currentIndex = index;// 更新当前工位索引
            if (工位英文名称列表 != null && LanguageMenagement.language == LanguageType.English && 工位英文名称列表.Count > 0)
            {
                lblStationName.Text = 工位英文名称列表[index];
            }
            else
            {
                lblStationName.Text = 工位名称列表[index]; //被选中工位的名称            
            }
            //更新柱状图：单工位生产统计
            int total = _countNGs[index] + _countOKs[index]; //总数
            chartHist.Series[0].Points[0].YValues = new double[] { total };
            chartHist.Series[0].Points[1].YValues = new double[] { _countOKs[index] }; //OK计数
            chartHist.Series[0].Points[2].YValues = new double[] { _countNGs[index] }; //NG计数

            double OKPercent = Math.Round(_countOKs[index] * 100.0 / total, 2);
            chartHist.Series[0].Points[3].YValues = new double[] { OKPercent }; //良率计数

            //良率超过99.5，则显示绿色，否则红色
            //if (OKPercent>99.5)
            //{
            //    chartHist.Series[0].Points[3].Color = Color.Chartreuse;
            //}
            //else
            //{
            //    chartHist.Series[0].Points[3].Color = Color.Red;
            //}

            //更新柱状图Y轴上限 = 总数，向上取整100
            //chartHist.ChartAreas[0].AxisY.Minimum = 0;
            if (total < 100)
            {
                chartHist.ChartAreas[0].AxisY.Maximum = 100;
            }
            else
            {
                chartHist.ChartAreas[0].AxisY.Maximum = Math.Ceiling(((double)total) / 100.0) * 100.0;
            }


            //立即重绘控件
            lblStationName.Update();
            chartHist.Update();
        }

        #endregion

        #region 6语言切换事件处理器
        public void LanguageChangeHandle()
        {
            if (LanguageMenagement.language == LanguageType.中文)
            {
                this.chartHist.Series[0].Points[0].Label = "总数：#VAL";
                this.chartHist.Series[0].Points[3].Label = "良率：#VAL";
                this.label2.Text = "单工位统计：";
                lblStationName.Text = 工位名称列表[_currentIndex] ; //被选中工位的名称            

            }
            else if (LanguageMenagement.language == LanguageType.English)
            {
                this.chartHist.Series[0].Points[0].Label = "Sum：#VAL";
                this.chartHist.Series[0].Points[3].Label = "Yield：#VAL";
                this.label2.Text = "SingleStationStatistics：";
                if(工位英文名称列表 != null && 工位英文名称列表.Count>0)
                    lblStationName.Text = 工位英文名称列表[_currentIndex] ; //被选中工位的英文名称            
            }
        }
        #endregion

    }// class

}// namespace
