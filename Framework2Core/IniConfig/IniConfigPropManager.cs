using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Framework2Core
{

    /// <summary>
    /// 静态类：读写Ini配置文件，并设置到对应的属性中；
    /// 还可以打开属性设置窗口，载入和保存属性
    /// </summary>
    public static class IniConfigPropManager
    {

        #region 1. 字段

        // 静态字段
        public static Type _ucType = typeof(UserControl); //UserControl 类型，用户自定义控件继承自此类
        public static Type _formType = typeof(Form); //Form类型，用户创建窗体继承自此类

        #endregion


        #region 4. 主要功能：读配置        

        /// <summary>
        /// object 类型的扩展方法：从 Ini 文件中读取对象的配置，并设置到其 对象属性列表 中
        /// </summary>
        /// <param name="obj">被设置的对象</param>
        /// <param name="sectionName">对象在 Ini 文件中对应的 section</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool LoadObjConfigsFromIni(this object obj, string sectionName, string[] arrLimits = null)
        {

            return LoadConfigsFromIni(obj, obj.GetType(), sectionName, arrLimits);//对象：objType=obj.GetType()

        }


        /// <summary>
        /// Type 类型的扩展方法：从 Ini 文件中读取类的配置，并设置到其 静态属性列表 中
        /// </summary>
        /// <param name="type">被设置的类型</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool LoadStaticConfigsFromIni(this Type type, string[] arrLimits = null)
        {

            return LoadConfigsFromIni(null, type, "static", arrLimits);//类型：obj=null，sectionName="static" 

        }

        /// <summary>
        /// Type 类型的扩展方法：从 Ini 文件中读取类的配置，并设置到其 静态属性列表 中
        /// </summary>
        /// <param name="type">被设置的类型</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        /// /// <param name="sectionName">对象在 Ini 文件中对应的 section。</param>
        public static bool LoadStaticConfigsFromIni(this Type type, string sectionName, string[] arrLimits = null)
        {

            return LoadConfigsFromIni(null, type, sectionName, arrLimits);//类型：obj=null，sectionName="static" 

        }


        /// <summary>
        /// 静态方法：从 Ini 文件中读取配置，并设置到对象或者静态类的属性列表中
        /// </summary>
        /// <param name="obj">被设置的对象。对于静态类或静态属性，传入 null</param>
        /// <param name="objType">对象的类型，或静态类</param>
        /// <param name="sectionName">对象在 Ini 文件中对应的 section。对于静态类或静态属性，传入 "static"</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool LoadConfigsFromIni(object obj, Type objType, string sectionName, string[] arrLimits = null)
        {

            try
            {

                IniFile iniFile = GenerateIniFile(objType, obj); //由对象和对象类型，生成相应的配置文件  
                bool isStatic = (obj == null); //如果对象为空，则要读取静态属性
                var listProps = GetListProps(objType, arrLimits, isStatic); //获取类的可配置属性列表，并限制在范围内

                // 遍历列表，从Ini文件中读取字符串并解析，然后赋值给对象/类
                foreach (var prop in listProps)
                {

                    var propName = prop.Name; //属性名
                    string str = iniFile.ReadString(sectionName, propName); //指定section、id，读取string
                    if (str != "")
                    {
                        SetStringValueToProp(obj, prop, str);//依次输入参数：对象，属性，属性值
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 静态方法：字符串 → 对象、属性：将字符串解析为指定类型，然后赋值给对象的属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <param name="strValue"></param>
        internal static void SetStringValueToProp(object obj, PropertyInfo prop, string strValue)
        {

            //使用扩展方法，解析字符串为目标类型
            var propType = prop.PropertyType;
            var parsedValue = strValue.ParseStringToTargetType(propType);

            if (parsedValue != null)
            {

                prop.SetValue(obj, parsedValue); //将解析得到的值，设置给对象的属性，并触发属性的 set 方法
            }

        }
        /// <summary>
        ///  静态方法：字符串 → 对象、属性：将字符串解析为指定类型，然后赋值给对象的属性,返回属性值改变记录
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <param name="strValue"></param>
        /// <param name="strLog">属性改变记录</param>
        internal static void SetStringValueToProp(object obj, PropertyInfo prop, string strValue, out string strLog)
        {
            strLog = null;
            //使用扩展方法，解析字符串为目标类型
            var propType = prop.PropertyType;
            var parsedValue = strValue.ParseStringToTargetType(propType);

            if (parsedValue != null)
            {
                var value = prop.GetValue(obj);     //获取对象的实际属性值
                Type propertyType = prop.PropertyType; //获取该属性的类型
                string strPropValue = ConvertActualValueToString(propType, value);
                if (strValue != strPropValue)
                {
                    string strPropName = prop.Name;   //获取对象属性名
                    strLog = $"[{strPropName}]的值由[{strPropValue}]更改为[{strValue}]\r\n";
                }

                prop.SetValue(obj, parsedValue); //将解析得到的值，设置给对象的属性，并触发属性的 set 方法

            }

        }

        #endregion


        #region 4. 主要功能：写配置        

        /// <summary>
        /// object 类型的扩展方法：将对象的 对象属性列表 的各值，写入到 Ini 文件中
        /// </summary>
        /// <param name="obj">被写入的对象</param>
        /// <param name="sectionName">对象在 Ini 文件中对应的 section</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool SaveObjConfigsToIni(this object obj, string sectionName, string[] arrLimits = null)
        {

            return SaveConfigsToIni(obj, obj.GetType(), sectionName, arrLimits);//对象：objType=obj.GetType()

        }


        /// <summary>
        /// Type 类型的扩展方法：将类型的 静态属性列表 的各值，写入到 Ini 文件中
        /// </summary>
        /// <param name="staticType">被写入的类型</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool SaveStaticConfigsToIni(this Type staticType, string[] arrLimits = null)
        {

            return SaveConfigsToIni(null, staticType, "static", arrLimits);//类型：obj=null，sectionName="static"

        }

        /// <summary>
        /// Type 类型的扩展方法：将类型的 静态属性列表 的各值，写入到 Ini 文件中
        /// </summary>
        /// <param name="staticType">被写入的类型</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool SaveStaticConfigsToIni(this Type staticType, string sectionName, string[] arrLimits = null)
        {

            return SaveConfigsToIni(null, staticType, sectionName, arrLimits);//类型：obj=null，sectionName="static"

        }


        /// <summary>
        /// 静态方法：将对象或类型的属性列表的各值，写入到 Ini 文件中
        /// </summary>
        /// <param name="obj">被写入的对象。对于静态或静态属性，传入 null</param>
        /// <param name="objType">对象的类型，或静态类</param>
        /// <param name="sectionName">对象在 Ini 文件中对应的 section。对于静态类或静态属性，传入 "static"</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool SaveConfigsToIni(object obj, Type objType, string sectionName, string[] arrLimits = null)
        {

            try
            {
                IniFile iniFile = GenerateIniFile(objType, obj); //由对象和对象类型，生成相应的配置文件  
                bool isStatic = (obj == null); //如果对象为空，则要读取静态属性
                var listProps = GetListProps(objType, arrLimits, isStatic); //获取类的可配置属性列表，并限制在范围内

                // 遍历列表，将对象/类的属性转换为 string，然后写入Ini文件
                foreach (var prop in listProps)
                {
                    var strValue = ConvertPropToString(obj, prop); // 将 obj 的 prop 属性的值，转换为字符串
                    iniFile.WriteString(sectionName, prop.Name, strValue); // 指定section、id，写入string
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 静态方法：对象、属性 → 字符串：获取对象指定属性的值，然后转换为字符串
        /// </summary>
        /// <param name="obj">对象，对于静态属性，传入 null</param>
        /// <param name="prop">属性</param>
        /// <returns>属性值转换后得到值，字符串类型</returns>
        internal static string ConvertPropToString(object obj, PropertyInfo prop)
        {

            var actualValue = prop.GetValue(obj); //获取该属性的实际值
            if (actualValue != null)
            {
                var propType = prop.PropertyType; //获取该属性的类型
                string strValue = ConvertActualValueToString(propType, actualValue); //将属性的实际值 ToString 为字符串
                return strValue;
            }

            return "";
        }

        #endregion


        #region 4. 主要功能：设置配置

        /// <summary>
        /// object 类型的扩展方法：打开设置窗口，修改并保存对象的属性
        /// </summary>
        /// <param name="obj">被设置的对象</param>
        /// <param name="sectionName">对象在 Ini 文件中对应的 section</param>
        /// <param name="formTitle">打开窗口后要显示的标题。默认为空字符串，不指定</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool SetObjConfigs(this object obj, string sectionName, string formTitle = "", string[] arrLimits = null)
        {

            return SetConfigs(obj, obj.GetType(), sectionName, formTitle, arrLimits); //对象：objType=obj.GetType()

        }


        /// <summary>
        /// Type 类型的扩展方法：打开设置窗口，修改并保存静态属性
        /// </summary>
        /// <param name="staticType">被设置的类型</param>
        /// <param name="formTitle">打开窗口后要显示的标题。默认为空字符串，不指定</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool SetStaticConfigs(this Type staticType, string formTitle = "", string[] arrLimits = null)
        {

            return SetConfigs(null, staticType, "static", formTitle, arrLimits);//类型：obj=null，sectionName="static"

        }


        /// <summary>
        /// 静态方法：打开设置窗口，修改并保存对象或类型的属性
        /// </summary>
        /// <param name="obj">被写入的对象。对于静态类或静态属性，传入 null</param>
        /// <param name="objType">对象的类型，或静态类</param>
        /// <param name="sectionName">对象在 Ini 文件中对应的 section。对于静态类或静态属性，传入 "static"</param>
        /// <param name="formTitle">打开窗口后要显示的标题。默认为空字符串，不指定</param>
        /// <param name="arrLimits">限定属性范围。默认值为 null，表示不限制</param>
        public static bool SetConfigs(object obj, Type objType, string sectionName, string formTitle = "", string[] arrLimits = null)
        {
            try
            {
                //将待设置的对象、类型，及其在ini文件中的sectionName，传入窗口中
                var form = new FormSetConfig(obj, objType, sectionName, formTitle, arrLimits);
                form.ShowDialog(); //打开属性设置窗口
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region 6. 私有方法：内部调用

        // 私有方法：类型 → 配置文件：由类型自动生成配置文件        
        private static IniFile GenerateIniFile(Type objType, object obj)
        {

            string strPrefix = @".\ApplicationConfig"; //地址前缀

            // 涉及到视觉的配置（实现 IVisionChange 接口），表示此类的对象有两个维度的变化：产品型号、视觉名称
            // 使用 .\ApplicationConfig\Models\型号名\Vision\视觉名\类型名Config.ini
            if (obj != null && obj is IVisionChange)
            {
                var visionName = (obj as IVisionChange).VisionName; //视觉名
                return new IniFile(strPrefix + @"\Models\" + ModelChangeVariables.当前型号 + @"\Vision\" + visionName + "\\" + objType.Name + "Config.ini");
            }

            // 涉及到产品换型的配置，地址前缀变更为：.\ApplicationConfig\Models\型号名
            if (objType.IsModelChangeable())
            {
                strPrefix += @"\Models\" + ModelChangeVariables.当前型号;
            }

            // 静态类：抽象封闭类：地址前缀 + \Static\类型名Config.ini
            if (objType.IsAbstract && objType.IsSealed)
            {
                return new IniFile(strPrefix + @"\Static\" + objType.Name + "Config.ini");
            }

            // 界面配置：地址前缀 + \Interface\类型名Config.ini
            if (objType.IsSubclassOf(_ucType) || objType.IsSubclassOf(_formType))
            {
                return new IniFile(strPrefix + @"\Interface\" + objType.Name + "Config.ini");
            }

            // 其他配置：地址前缀 + \Device\类型名Config.ini
            return new IniFile(strPrefix + @"\Device\" + objType.Name + "Config.ini");

        }


        /// <summary>
        /// 静态方法：类型 → 属性列表：获取类的可配置属性列表，并限制在范围内
        /// </summary>
        /// <param name="objType">被配置的对象类型，或静态类</param>
        /// <param name="strLimits">限制的属性范围</param>
        /// <param name="isStatic">是否为静态属性</param>
        /// <returns>类型的可配置属性列表</returns>
        internal static List<PropertyInfo> GetListProps(Type objType, string[] strLimits, bool isStatic)
        {

            // 调用扩展方法：获取给定类型的属性列表
            // isStatic = true，静态属性；isStatic = false，对象属性
            var listProps = isStatic ? objType.GetListStaticProps() : objType.GetListObjProps();

            // 如果有限制，则仅返回在限制内的属性
            if (strLimits != null)
            {
                var limits = new List<string>(strLimits); //限定范围
                var listLimitedProps = new List<PropertyInfo>();

                // 遍历属性列表，如果某属性在限定范围内，则添加到 listLimitedProps 中
                foreach (var prop in listProps)
                {
                    if (limits.Contains(prop.Name))
                    {
                        listLimitedProps.Add(prop);
                    }
                }

                return listLimitedProps;
            }

            // 无限制，返回整个属性列表
            else
            {
                return listProps;
            }
        }


        /// <summary>
        /// 静态方法：属性值 → 字符串：根据属性的类型，将属性的实际值转换为字符串
        /// </summary>
        /// <param name="propType">属性的类型</param>
        /// <param name="actualValue">属性的实际值</param>
        /// <returns>属性值转换后得到值，字符串类型</returns>
        internal static string ConvertActualValueToString(Type propType, object actualValue)
        {

            // 如果值为空，返回空字符串
            if (actualValue == null)
            {
                return "";
            }
            

            //属性不是泛型或数组，直接ToString
            if (!propType.IsGenericType && !propType.IsArray)
            {
                return actualValue.ToString();
            }

            //属性是泛型或数组，遍历元素ToString，然后将字符串拼接起来
            StringBuilder sb = new StringBuilder();

            //Array[]，字符串格式为 a, b, c,...
            //List<>，字符串格式为 a, b, c,...
            if (propType.Name.StartsWith("List") || propType.IsArray)
            {
                var list = actualValue as IList; // List<T> 和 Array 都继承自 IList 接口，可遍历
                foreach (var item in list)
                {
                    sb.Append(item.ToString());
                    sb.Append(", ");
                }
            }

            //Dicionary<,> 字符串格式为 <key1,value1>, <key2,value2>, ...
            else if (propType.Name.StartsWith("Dictionary"))
            {
                var dic = actualValue as IDictionary; //转换为接口：字典
                foreach (DictionaryEntry item in dic)
                {
                    sb.Append("<");
                    sb.Append(item.Key); //键
                    sb.Append(", ");
                    sb.Append(item.Value); //值
                    sb.Append(">, ");
                }
            }

            if (sb.Length > 2)
            {
                sb.Remove(sb.Length - 2, 2);//去掉最后多余的两个字符：", "
            }

            return sb.ToString();
        }


        #endregion

    }// class IniConfigPropManager


    #region 辅助类

    /// <summary>
    /// 辅助类：属性列表集合。包含两个字典：《类型名，对象属性列表》 以及 《类型名，静态属性列表》。可通过扩展方法，找到类型对应的属性列表
    /// </summary>
    public static class IniConfigPropHelper
    {

        #region 1. 字段

        // 私有字段：字典 <类型名，对象属性列表>
        private static Dictionary<string, List<PropertyInfo>> _dic_Type_ListObjProps = new Dictionary<string, List<PropertyInfo>>();

        // 私有字段：字典 <类型名，静态属性列表>
        private static Dictionary<string, List<PropertyInfo>> _dic_Type_ListStaticProps = new Dictionary<string, List<PropertyInfo>>();

        #endregion


        #region 4. 主要功能：根据输入的类型信息，找到可配置的属性列表

        /// <summary>
        /// Type 类型的扩展方法：根据输入的类型信息，找到可配置的 对象属性列表
        /// </summary>
        /// <param name="t">类型信息</param>
        /// <returns>类型的可配置的 对象属性列表</returns>
        public static List<PropertyInfo> GetListObjProps(this Type t)
        {

            var typeName = t.FullName;

            // 字典中未添加该类型，先注册
            if (!_dic_Type_ListObjProps.ContainsKey(typeName))
            {
                var listObjProps = new List<PropertyInfo>();

                var properties = t.GetProperties();
                foreach (var prop in properties)
                { //通过反射，遍历类型的所有属性
                    var attr = prop.GetCustomAttribute<IniConfigAttribute>(); //获得指定类型的自定义特性，如果没有，返回 null
                    if (attr != null && !prop.GetMethod.IsStatic)
                    { //对象属性，其 GetValue 方法.IsStatic=false
                        listObjProps.Add(prop); //将被标记的属性，添加到列表中
                    }
                }

                // 在字典中注册该类型的 对象属性列表，Key 为类型的全名
                _dic_Type_ListObjProps.Add(typeName, listObjProps);
            }

            return _dic_Type_ListObjProps[typeName]; //从字典中，找到类型的 静态属性列表
        }


        /// <summary>
        /// Type 类型的扩展方法：根据输入的类型信息，找到可配置的 静态属性列表
        /// </summary>
        /// <param name="t">类型信息</param>
        /// <returns>类型的可配置的 静态属性列表</returns>
        public static List<PropertyInfo> GetListStaticProps(this Type t)
        {

            var typeName = t.FullName;

            // 字典中未添加该类型，先注册
            if (!_dic_Type_ListStaticProps.ContainsKey(typeName))
            {
                var listStaticProps = new List<PropertyInfo>();

                var properties = t.GetProperties();
                foreach (var prop in properties)
                { //通过反射，遍历类型的所有属性
                    var attr = prop.GetCustomAttribute<IniConfigAttribute>(); //获得指定类型的自定义特性，如果没有，返回 null
                    if (attr != null && prop.GetMethod.IsStatic)
                    { //静态属性，其 GetValue 方法.IsStatic=true
                        listStaticProps.Add(prop); //将被标记的属性，添加到列表中
                    }
                }

                // 在字典中注册该类型的 静态属性列表，Key 为类型的全名
                _dic_Type_ListStaticProps.Add(typeName, listStaticProps);
            }

            return _dic_Type_ListStaticProps[typeName]; //从字典中，找到类型的 静态属性列表
        }

        #endregion

    }// class ListConfigPropsSet

    #endregion

}// namespace
