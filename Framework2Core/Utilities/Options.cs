using System.Collections.Generic;

namespace Framework2Core {

    /// <summary>
    /// 抽象类：用于限制 Ini 配置的属性的可选值。在子类中重写属性 ListOptions，来设置可选值的列表。
    /// 属性 Text 表示被选中的项
    /// </summary>
    public abstract class AbstractOptions {

        #region 1. 属性

        /// <summary>
        /// 抽象只读属性：可选值的列表。必须在子类中重写
        /// </summary>
        public abstract List<string> ListOptions { get; }

        /// <summary>
        /// 属性：可选列表 ListOptions 中被选中的项。默认值为 ListOptions 的第0项
        /// </summary>
        public string Text {
            get {
                if (_text == null) { //未初始化
                    SetTextToDefault();
                }
                return _text;
            }
            set {
                if (ListOptions.Contains(value)) {
                    _text = value;
                }
                //如设置的值不在可选列表中，且还未赋值，则使用默认值
                else {
                    if (_text == null) {
                        SetTextToDefault();
                    }
                }
            }
        }
        private string _text = null;

        #endregion


        #region 4. 功能：重写 ToString

        /// <summary>
        /// 重写基类方法：返回 Text，即被选中的项
        /// </summary>
        public override string ToString() {
            return Text; //返回被选中项
        }

        #endregion


        #region 6. 私有方法

        // 私有方法：设置被选中的项，为默认值
        private void SetTextToDefault() {
            
            // 如果没有可选项列表，则给一个 空字符串 作为可选项
            if (ListOptions==null || ListOptions.Count == 0) {
                _text =  "" ;
            }
            else {
                _text = ListOptions[0]; //默认值为第0个选项
            }            
        }

        #endregion

    }// class AbstractOptions


    /// <summary>
    /// 可选项类型：设置串口波特率。可选的值包括：9600、19200、38400、57600、115200
    /// </summary>
    public class BaudrateOptions : AbstractOptions {

        #region 1. 属性

        /// <summary>
        /// 重写父类的属性：可选值列表
        /// </summary>
        public override List<string> ListOptions { get; }
            = new List<string> { "9600", "19200", "38400", "57600", "115200" };

        #endregion

    } // class BaudrateOptions


    /// <summary>
    /// 可选项类型：设置当前型号。可选的值由 ModelChange 的属性 [可选型号列表] 动态决定
    /// </summary>
    public class ModelOptions : AbstractOptions {

        #region 1. 属性

        /// <summary>
        /// 重写父类的属性：可选值列表，跟随 ModelChangeVariables 类的 [可选型号列表] 属性 “动态” 变化
        /// </summary>
        public override List<string> ListOptions { get; } = ModelChangeVariables.可选型号列表;

        #endregion

    } // class ModelOptions

}// namespace
