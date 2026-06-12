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
    /// 自定义控件：以饼状图形式显示全机台 NG 分类统计。可支持最多 7 种 NG 类型
    /// </summary>
    public partial class ChartPieNG : UserControl {

        #region 1. 字段

        // 字段：全机台 NG 分类字典： 《NG名称，NG数量》
        public Dictionary<string, int> _dicNGTypes;

        // 字段：控件对应的 NG 分类列表。由 Mainform 传入的列表给定
        public List<string> NG分类列表;

        //字段：控件对应的 NG 分类列表英文名。由 Mainform 传入的列表给定
        public List<string> NG分类列表英文名;

        // 字段：关联的 NG 类型数量
        private int _ngTypeCount;

        #endregion


        #region 2. 构造函数

        // 实例构造函数。在父窗体构造函数中的 InitializeComponent 方法中调用       
        public ChartPieNG() {
            InitializeComponent(); //初始化控件界面，VS自动生成的代码
            LanguageMenagement.LanguageChangeDeleg += LanguageChangeHandle;

        }

        #endregion


        #region 3. 加载配置：从 MainForm 中同步 NG 分类列表的数量，并更新界面

        /// <summary>
        /// 方法：控件添加到窗体后，从 MainForm 中同步 NG 分类列表的数量，并更新界面。
        /// 此方法要在父窗体构造函数的 InitializeComponent 方法之后使用
        /// </summary>
        public void UpdateNGList(List<string> listNG,List <string> listNGEnName = null) {

            NG分类列表 = listNG; //使用传入的NG分类列表
            NG分类列表英文名 = listNGEnName;   //使用传入的NG分类列表英文名

            // 至少有一个NG类型
            if (NG分类列表 == null || NG分类列表.Count < 1) {
                NG分类列表 = new List<string>() { "NG" };
            }

            // 将NG列表各项作为 Key，依次添加到字典 _dicNGTypes 中
            _dicNGTypes = new Dictionary<string, int>();
            foreach (var ngType in NG分类列表) {
                _dicNGTypes.Add(ngType, 0); //各分类初始值：0
                //_dicNGTypes.Add(ngType, new Random(Guid.NewGuid().GetHashCode()).Next(10, 100)); //测试用：各分类初始值给随机数
            }

            // 更新NG饼状图的图例
            List<string> tempListNG = new List<string>();
            if(NG分类列表英文名 != null && LanguageMenagement.language == LanguageType.English && NG分类列表英文名.Count == NG分类列表.Count)
            {
                tempListNG = NG分类列表英文名;
            }
            else
            {
                tempListNG = NG分类列表;
            }
            _ngTypeCount = NG分类列表.Count;  //NG类别数
            int countLegends = chartPie.Series[0].Points.Count; //饼状图的分区数
            for (int i = 0; i < countLegends; i++) {
                if (i < _ngTypeCount) {
                    chartPie.Series[0].Points[i].LegendText = tempListNG[i] + ":#VALY"; //图例格式：名称：值
                }
                else {
                    var point = chartPie.Series[0].Points[_ngTypeCount];
                    chartPie.Series[0].Points.RemoveAt(_ngTypeCount); //删除多余的分区：例如字典里定义了5类，而饼状图里面有6个分区
                    point.Dispose(); //移除后释放此对象
                }
            }

            RefreshChart(); //更新显示
        }

        #endregion


        #region 4. 主要功能：NG 分类统计：加 1、清零

        /// <summary>
        /// 主要功能：方法：将指定类型的 NG 计数加 1
        /// </summary>
        /// <param name="ngType">要增加的 NG 类型名称</param>
        public void AddNGType(string ngType) {
            if (_dicNGTypes.ContainsKey(ngType)) {
                _dicNGTypes[ngType]++;
            }

            RefreshChart(); //更新饼状图显示
        }


        /// <summary>
        /// 主要功能：方法：将所有工位 NG 计数清零
        /// </summary>
        public void ClearAllNGs() {
            //清NG分类统计
            foreach (var key in NG分类列表) {
                _dicNGTypes[key] = 0;
            }

            RefreshChart(); //更新饼状图显示
        }

        #endregion


        #region 4. 主要功能：更新饼状图显示

        /// <summary>
        /// 主要功能：方法：更新饼状图中各 NG 分类的计数
        /// </summary>
        public void RefreshChart() {
            //更新饼状图：NG分类
            for (int i = 0; i < _ngTypeCount; i++) {
                chartPie.Series[0].Points[i].YValues = new double[] { _dicNGTypes[NG分类列表[i]] }; //按顺序为不同NG类型赋值
            }

            chartPie.Update(); //立即重绘
        }

        #endregion


        #region 6语言切换事件处理器
        public void LanguageChangeHandle()
        {
            // 更新NG饼状图的图例
            List<string> tempListNG = new List<string>();
            label2.Text = LanguageMenagement.language == LanguageType.English? "Allstation:NGClassification": "所有工位：不良分类";
            
            if (NG分类列表英文名 != null && LanguageMenagement.language == LanguageType.English && NG分类列表英文名.Count == NG分类列表.Count)
            {
                tempListNG = NG分类列表英文名;
            }
            else
            {
                tempListNG = NG分类列表;
            }
            _ngTypeCount = NG分类列表.Count;  //NG类别数
            int countLegends = chartPie.Series[0].Points.Count; //饼状图的分区数
            for (int i = 0; i < countLegends; i++)
            {
                if (i < _ngTypeCount)
                {
                    chartPie.Series[0].Points[i].LegendText = tempListNG[i] + ":#VALY"; //图例格式：名称：值
                }
                else
                {
                    var point = chartPie.Series[0].Points[_ngTypeCount];
                    chartPie.Series[0].Points.RemoveAt(_ngTypeCount); //删除多余的分区：例如字典里定义了5类，而饼状图里面有6个分区
                    point.Dispose(); //移除后释放此对象
                }
            }
        }
        #endregion

    }// class
}// namespace
