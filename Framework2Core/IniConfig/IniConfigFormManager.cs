using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Framework2Core {
    /// <summary>
    /// 静态类：根据属性的类型，在 FormSetConfig 窗口中生成相应的控件；可将属性的值赋给控件，也可以将控件的值（以字符串的形式）取回给属性
    /// </summary>
    public static class IniConfigFormManager {

        #region 4. 主要功能

        /// <summary>
        /// 静态方法：属性 → 控件：根据属性的元数据，生成对应的配置控件
        /// </summary>
        /// <param name="prop">属性的元数据</param>
        /// <returns></returns>
        public static Control CreateConfigControl(PropertyInfo prop,ref Dictionary<string,string> dicComboxTextToValue) {
            //dicComboxTextToValue = new Dictionary<string,string>();
            Control ctrl = null;
            Type t = prop.PropertyType; //属性的类型

            //1. 属性为string：Textbox
            if (t.Name == "String" || t.Name == "DateTime") {
                ctrl = new TextBox { Name = "txtSet" + prop.Name };
            }

            //2. 属性为枚举类型，获取其所有枚举值，并添加到 Combobox
            else if (t.IsEnum) {
                ctrl = new ComboBox {
                    Name = "cmbSet" + prop.Name,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                //(ctrl as ComboBox).Items.AddRange(Enum.GetNames(t));

                List<string> listAttributes = new List<string>();
                var fileds = t.GetFields();
                foreach (var item in fileds)
                {
                    Type type = item.FieldType;
                    if (type.Name == "Int32")
                    {
                        continue;
                    }
                    //var customAttribute = Attribute.GetCustomAttribute(item, typeof(IniConfigAttribute));
                    //string strName = customAttribute == null ? item.Name : (customAttribute as IniConfigAttribute).EnglishName;
                    string strName = item.GetFieldInfoName();
                    dicComboxTextToValue[strName] = item.Name;
                    listAttributes.Add(strName);
                }
                (ctrl as ComboBox).Items.AddRange(listAttributes.ToArray());
            }

            //3. 属性为其他值类型：CheckBox 或 NumericUpDown
            else if (t.IsValueType) {
                switch (t.Name) {
                    case "Boolean": {
                            ctrl = new CheckBox() { Name = "chkSet" + prop.Name };
                            break;
                        }
                    case "Single":
                    case "Double":
                    case "Decimal": {
                            ctrl = new NumericUpDown {
                                Name = "nudSet" + prop.Name,
                                Maximum = Int32.MaxValue,
                                Minimum = Int32.MinValue,
                                DecimalPlaces = 3 //小数，精确到小数点后3位
                            };
                            break;
                        }
                    default://整数，包括Int16/32/64，UInt16/32/64
                        ctrl = new NumericUpDown {
                            Name = "nudSet" + prop.Name,
                            Maximum = Int32.MaxValue,
                            Minimum = t.Name.StartsWith("U") ? 0 : Int32.MinValue, //类型名以 U 开头，最小值为0
                            DecimalPlaces = 0 //整数，精确到小数点后0位
                        };
                        break;
                }//switch
            }

            //4. 属性为泛型或数组：Textbox
            else if (t.IsGenericType || t.IsArray) {
                var txt = new TextBox { Name = "txtSet" + prop.Name };
                if (t.Name.StartsWith("Dictionary")) { //字典：控件高度高度为50
                    txt.Multiline = true; //允许多行
                    txt.WordWrap = false; //不自动换行
                    txt.ScrollBars = ScrollBars.Vertical;  //显示滚动条
                    txt.Size = new Size(200, 50);
                }
                ctrl = txt;
            }

            //5. 属性继承自 AbstractOptions 类型，获取其所有选项，并添加到Combobox
            else if (t.IsSubclassOf(typeof(AbstractOptions))) {
                ctrl = new ComboBox {
                    Name = "cmbSet" + prop.Name,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                //获取可选列表
                var newObj = Activator.CreateInstance(t) as AbstractOptions; //新建对象
                List<string> optionList = newObj.ListOptions;

                //将可选列表添加到ComboBox中
                (ctrl as ComboBox).Items.AddRange(optionList.ToArray());
            }

            return ctrl;
        }


        /// <summary>
        /// Type 类型的扩展方法：获得给定类型的名称。如果是泛型类型，则拼接为 List《T》 或 Dictionary《TKey,TValue》
        /// </summary>
        /// <param name="t">类型</param>
        /// <returns>类型的名称</returns>
        public static string GetTypeName(this Type t) {

            string typeName = t.Name;

            if (t.IsGenericType) {
                var paramTypes = t.GetGenericArguments(); //获得泛型的参数化类型集合
                if (typeName.StartsWith("List")) {
                    typeName = $"List<{paramTypes[0].Name}>"; //List<T>
                }
                else if (typeName.StartsWith("Dictionary")) { //Dictionary<TKey,TValue>
                    typeName = $"Dictionary<{paramTypes[0].Name},{paramTypes[1].Name}>";
                }
            }

            return typeName;
        }
        /// <summary>
        /// PorpertyInfo类型的扩展方法：获得给定类型的名称。
        /// </summary>
        /// <param name="t">类型</param>
        /// <returns>属性的名称</returns>
        public static string GetPropInfoName(this PropertyInfo propertyInfo)
        {
            string propName = propertyInfo.Name;
            //if (LanguageMenagement.language == LanguageType.中文)
            //{
            //    return propName;
            //}
            var attrs = propertyInfo.GetCustomAttributes(typeof(IniConfigAttribute),false);
            foreach (var attr in attrs)
            {
                if (!(attr is IniConfigAttribute)) continue;
                var a = attr as IniConfigAttribute;
                if (a.EnglishName != null && LanguageMenagement.language == LanguageType.English)
                {
                    propName =a.EnglishName;
                }else if (a.ChineseName != null && LanguageMenagement.language == LanguageType.中文)
                {
                    propName = a.ChineseName;
                }
            }
            return propName;
        }

        public static string GetFieldInfoName(this FieldInfo fieldInfo)
        {
            string fieldName = fieldInfo.Name;
            var attrs = Attribute.GetCustomAttributes(fieldInfo,typeof(IniConfigAttribute), false);
            foreach (var attr in attrs)
            {
                if (!(attr is IniConfigAttribute)) continue;
                var a = attr as IniConfigAttribute;
                if (a.EnglishName != null && LanguageMenagement.language == LanguageType.English)
                {
                    fieldName = a.EnglishName;
                }
                else if (a.ChineseName != null && LanguageMenagement.language == LanguageType.中文)
                {
                    fieldName = a.ChineseName;
                }
            }
            return fieldName;
            //var customAttribute = Attribute.GetCustomAttribute(item, typeof(IniConfigAttribute));
            //string strName = customAttribute == null ? item.Name : (customAttribute as IniConfigAttribute).EnglishName;
        }

        public static string GetEnumAttribute(this Enum val)
        {
            var filed = val.GetType().GetField(val.ToString());
            var customAttribute = Attribute.GetCustomAttribute(filed, typeof(IniConfigAttribute));
            return customAttribute == null ? val.ToString() : (customAttribute as IniConfigAttribute).EnglishName;
        }

        public static List<string> GetEnumAttributes(this Enum val)
        {
            List<string> listAttributes = new List<string>();
            var fileds = val.GetType().GetFields();
            foreach (var item in fileds)
            {
                var customAttribute = Attribute.GetCustomAttribute(item, typeof(IniConfigAttribute));
                listAttributes.Add(customAttribute == null ? val.ToString() : (customAttribute as IniConfigAttribute).EnglishName);
            }
            return listAttributes;
        }


        /// <summary>
        /// 静态方法：字符串 → 控件：根据属性的类型解析字符串，赋值给对应的配置控件
        /// </summary>
        /// <param name="ctrl">属性对应的控件</param>
        /// <param name="propType">属性的类型</param>
        /// <param name="str">属性对应的值，字符串类型</param>
        public static void SetStringValueToControl(Control ctrl, Type propType, string str) {

            //配置控件为 ComboBox，则属性为可选择类型，包括枚举类型，以及 AbstractOptions 的子类
            //或者属性的类型为 string，直接赋值给控件的 Text
            if (ctrl is ComboBox || propType.Name == "String" || propType.Name == "DateTime") {
                ctrl.Text = str;
            }

            //配置控件为 CheckBox，解析为 Boolean
            else if (ctrl is CheckBox) {
                (ctrl as CheckBox).Checked = Boolean.Parse(str);
            }

            //配置控件为 NumericUpDown，解析为基本类型
            else if (ctrl is NumericUpDown) {
                (ctrl as NumericUpDown).Value = Decimal.Parse(str);
            }

            //属性的类型为泛型或数组
            else if (propType.IsGenericType || propType.IsArray) {
                if (ctrl.Size.Height == 50) { //字典：控件高度高度为50
                    ctrl.Text = str.Replace(", <", ",\r\n<"); //添加换行符
                }
                else { //List<T> 或 Array[]
                    ctrl.Text = str;
                }
            }
        }


        /// <summary>
        /// 静态方法：控件 → 字符串：根据控件的类型，取回对应的值（字符串类型）
        /// </summary>
        /// <param name="ctrl">被取值的控件</param>
        /// <returns>获取的控件值，字符串类型</returns>
        public static string GetStringValueFromControl(Control ctrl) {
            string str = null;
            if (ctrl is CheckBox)
            {
                str = (ctrl as CheckBox).Checked.ToString();
            }
            else if (ctrl is NumericUpDown)
            {
                str = (ctrl as NumericUpDown).Value.ToString();
            }
            else if (ctrl is TextBox /*|| ctrl is ComboBox*/)
            {
                str = ctrl.Text;
                if (ctrl.Size.Height == 50)
                { //字典对应的 TextBox 高度为50
                    str = str.Replace("\r\n", ""); //去除 Text 的换行符
                }
            }
            else if (ctrl is ComboBox) { 
                str = ctrl.Text;
            }
            return str;
        }

        private static void CtrlValueChange(object sender, EventArgs e)
        {
            //获取控件type
            Type type = sender.GetType();   
            //获取控件类型
            string name = type.Name;
           
        }

        #endregion

    }// class

}// namespace
