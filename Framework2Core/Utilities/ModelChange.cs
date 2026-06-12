using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Framework2Core {

    /// <summary>
    /// 特性：标记于类型上，表示此类型是可换型的。
    /// 可换型的类型，其配置文件、图像数据保存路径前缀为：.\ApplicationConfig\Models\型号名
    /// </summary>
    public class ModelChangeAttribute : Attribute {

    }//class ModelChangeAttribute


    /// <summary>
    /// 辅助类：包含字典：《类型名，是否可换型》。可通过扩展方法，判断类型是否可换型
    /// </summary>
    public static class ModelChangeHelper{

        #region 1. 字段

        // 私有字段：字典 <类型名，是否可换型>
        private static Dictionary<string, bool> _dic_Type_ModelChangeable = new Dictionary<string, bool>();

        #endregion


        #region 4. 功能：判断类型是否可换型

        /// <summary>
        /// Type 类型的扩展方法：根据输入的类型信息，判断该类型是否可换型
        /// </summary>
        /// <param name="t">类型信息</param>
        /// <returns>类型是否可换型</returns>
        public static bool IsModelChangeable(this Type t) {

            var typeName = t.FullName;

            // 字典中未添加该类型，先注册
            if (!_dic_Type_ModelChangeable.ContainsKey(typeName)) {
                //获得指定类型的自定义特性，如果不为空，说明可换型
                var attr = t.GetCustomAttribute<ModelChangeAttribute>();
                _dic_Type_ModelChangeable.Add(typeName, attr != null); //在字典中注册该类型
            }

            return _dic_Type_ModelChangeable[typeName]; //从字典中，判断该类型是否可换型
        }

        #endregion

    }// class ModelChangeHelper


    /// <summary>
    /// 静态类型：包含换型的相关变量：可选型号列表、当前型号
    /// </summary>
    public static class ModelChangeVariables {

        #region 1. 字段

        // 字段：默认型号的名称
        private static string _defaultModel = "Default"; 

        #endregion


        #region 2. 静态构造函数

        // 静态构造函数：首次被调用静态类时，自动调用此方法
        static ModelChangeVariables() {
            Type staticType = typeof(ModelChangeVariables); //本静态类的类型

            //加载配置文件
            staticType.LoadStaticConfigsFromIni();

            //保存配置文件
            staticType.SaveStaticConfigsToIni();
        }

        #endregion


        #region 3. 可配置的属性

        /// <summary>
        /// 可配置的属性：可选型号的列表，必须包含默认型号 "Default"
        /// </summary>
        [IniConfig]
        public static List<string> 可选型号列表 {
            get { return _listAvaliableModels; }
            set {
                // 如果传入的列表为空，或者不包含默认型号，则添加默认型号
                if (value == null) {
                    value = new List<string>() { _defaultModel };
                } 
                else if(!value.Contains(_defaultModel)) {
                    value.Add(_defaultModel);
                }

                // 如果传入的列表不包含当前型号，则添加当前型号
                string strCurrentModel = _currentModel.Text;
                if (!value.Contains(strCurrentModel)) {
                    value.Add(strCurrentModel);
                }

                _listAvaliableModels = value;

                //更新 _currentModel（当前型号）的 ListOptions（可选列表）：创建一个新的 ModelOptions 对象
                _currentModel = new ModelOptions() { Text = strCurrentModel };
            }
        }
        private static List<string> _listAvaliableModels = new List<string> { _defaultModel };


        /// <summary>
        /// 可配置的属性：当前型号，使用 ModelOptions 类型。
        /// 可选值的列表，跟随 ModelChangeVariables 类的 [可选型号列表] 属性 “动态” 变化
        /// </summary>
        [IniConfig]
        public static ModelOptions 当前型号 {
            get { return _currentModel; }
            set {
                var valueText = value.Text; //传入的型号

                // 如果传入的型号为空，或者不在 [可选型号列表] 中，则使用默认型号
                if (string.IsNullOrEmpty(valueText) || !_listAvaliableModels.Contains(valueText)) {
                    value = new ModelOptions() { Text = _defaultModel };
                }
                _currentModel = value;
            }
        }
        private static ModelOptions _currentModel = new ModelOptions() { Text = _defaultModel };

        #endregion

    } // class ModelChangeVariables

}// namespace
