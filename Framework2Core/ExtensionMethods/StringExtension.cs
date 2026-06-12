using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Framework2Core {

    /// <summary>
    /// 静态类：string、string[] 的扩展方法
    /// </summary>
    public static class StringExtension {

        #region 4. 主要功能

        /// <summary>
        /// string 类型的扩展方法：将字符串解析为目标类型的对象。
        /// 支持的类型包括：string、值类型（枚举类型，int，double 等）、数组、泛型（List，Dictionary）、AbstractOptions 的子类
        /// </summary>
        /// <param name="str">被解析的字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>解析得到的对象，由 object 类型变量引用</returns>
        public static object ParseStringToTargetType(this string str, Type targetType) {

            object parsedValue = null; //解析得到的属性值

            //1. 基础类型：String、枚举类型、其他值类型
            if (targetType.Name == "String") {
                parsedValue = str;
            }
            else if (targetType.IsEnum) {
                try {
                    parsedValue = Enum.Parse(targetType, str); //解析字符串为枚举值（此处会抛异常：解析失败）
                }
                catch {
                    parsedValue = Activator.CreateInstance(targetType);//如果发生异常：使用枚举类型的默认值
                }                
            }
            else if (targetType.IsValueType) {
                parsedValue = str.ParseStringToBasicType(targetType); //解析字符串为基础类型
            }

            //2. 数组：Array[] 字符串格式为 a,b,c,...
            else if (targetType.IsArray) {
                var arrItems = str.Replace(" ", "").Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries); //去除空格，然后以逗号分隔为字符串数组
                parsedValue = arrItems.ParseStringArrayToArray(targetType); //解析字符串数组为 Array[]
            }

            //3. 泛型，包括 List<>，Dictionary<,>
            else if (targetType.IsGenericType) {

                //3.1 List<> 字符串格式为 a,b,c,...
                if (targetType.Name.StartsWith("List")) {
                    var arrItems = str.Replace(" ", "").Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries); //去除空格，然后以逗号分隔为字符串数组
                    parsedValue = arrItems.ParseStringArrayToList(targetType); //解析字符串数组为 List<T>
                }

                //3.2 Dictionary<,> 字符串格式为 <key1,value1>, <key2,value2>...
                else if (targetType.Name.StartsWith("Dictionary")) {
                    var arrItems = str.Replace(" ", "").Split(new char[] { ',', '，', '<', '>' }, StringSplitOptions.RemoveEmptyEntries); //去除空格，然后以 逗号、<、> 分隔
                    parsedValue = arrItems.ParseStringArrayToDic(targetType); //解析字符串数组为 Dictionary<,>
                }
            }

            //4. AbstractOptions 的子类
            else if (targetType.IsSubclassOf(typeof(AbstractOptions))) {
                var optionObj = Activator.CreateInstance(targetType) as AbstractOptions; //新建对象
                optionObj.Text = str; //字符串赋值给 AbstractOptions 类的 Text 属性
                parsedValue = optionObj; //将新对象交给 parsedValue
            }

            return parsedValue;
        }


        /// <summary>
        /// string 类型的扩展方法：解析字符串为基础类型的对象
        /// </summary>
        /// <param name="str">被解析的字符串</param>
        /// <param name="basicType">目标类型，限定为值类型或 string</param>
        /// <returns>解析得到的对象，由 object 类型变量引用</returns>
        public static object ParseStringToBasicType(this string str, Type basicType) {

            if (basicType.Name == "String") { //字符串：直接返回
                return str;
            }

            if (basicType.IsEnum) {
                return Enum.Parse(basicType, str); //解析字符串为枚举值
            }

            if (basicType.IsValueType) { //目标类型为值类型
                try {
                    //调用扩展方法：根据类型，获取值类型的 TryParse 方法
                    var method = basicType.GetTryParseMethod();
                    var parameters = new object[] { str, Activator.CreateInstance(basicType) };
                    method.Invoke(null, parameters); //调用 TryParse 方法
                    return parameters[1]; //返回out参数，索引为 1
                }
                catch {
                }
            }

            return null; //发生错误，转换失败
        }


        /// <summary>
        /// string[] 类型的扩展方法：将字符串数组解析为数组 Array[] 的对象
        /// </summary>
        /// <param name="arrStr">被解析的字符串数组，每个字符串对应一个数组元素</param>
        /// <param name="arrayType">数组 Array[] 的类型</param>
        /// <returns>解析得到的对象，由 object 类型变量引用</returns>
        public static object ParseStringArrayToArray(this string[] arrStr, Type arrayType) {

            if (arrayType.IsArray) { //目标类型限定为 Array
                try {
                    //获得数组元素的类型，例如 int[] 的 int
                    string eleTypeName = arrayType.FullName.Replace("[]", string.Empty); //去除数组类型名的 "[]"
                    Type eleType = arrayType.Assembly.GetType(eleTypeName);//从程序集中，根据名称找到元素类型

                    //根据数组元素类型，创建对应的 List<T> 类型及对象
                    var listType = typeof(List<>).MakeGenericType(eleType); //创建泛型类 List<T>
                    var obj = Activator.CreateInstance(listType); //创建新的泛型对象
                    var list = obj as IList; //转换为接口：列表

                    foreach (var str in arrStr) { //遍历字符串数组
                        var item = ParseStringToBasicType(str, eleType); //解析字符串为基础类型
                        list.Add(item); //添加元素到列表中
                    }

                    //将 List<T> 转换为 T[] 数组
                    var toArrayMethod = listType.GetMethod("ToArray");
                    var result = toArrayMethod.Invoke(list, null);
                    return result;
                }
                catch {
                }
            }

            return null; //发生错误，转换失败
        }


        /// <summary>
        /// string[] 类型的扩展方法：将字符串数组解析为泛型 List《T》的对象
        /// </summary>
        /// <param name="arrStr">被解析的字符串数组，每个字符串对应一个列表元素</param>
        /// <param name="listType">泛型 List《T》的类型</param>
        /// <returns>解析得到的对象，由 object 类型变量引用</returns>
        public static object ParseStringArrayToList(this string[] arrStr, Type listType) {

            if (listType.IsGenericType && listType.Name.StartsWith("List")) { //目标类型限定为 List<T>
                try {
                    var typeT = listType.GetGenericArguments()[0]; //获得泛型的参数化类型，即 List<T> 的 T
                    var obj = Activator.CreateInstance(listType); //创建新的泛型对象
                    var list = obj as IList; //转换为接口：列表

                    foreach (var str in arrStr) { //遍历字符串数组
                        var item = ParseStringToBasicType(str, typeT); //解析字符串为基础类型
                        list.Add(item); //添加元素到列表中
                    }
                    return list;
                }
                catch {
                }
            }

            return null; //发生错误，转换失败
        }


        /// <summary>
        /// string[] 类型的扩展方法：将字符串数组解析为泛型 Dictionary《TKey,TValue》的对象 
        /// </summary>
        /// <param name="arrStr">被解析的字符串数组，每两个字符串对应一个键值对</param>
        /// <param name="dicType">泛型 Dictionary《TKey,TValue》的类型</param>
        /// <returns>解析得到的对象，由 object 类型变量引用</returns>
        public static object ParseStringArrayToDic(this string[] arrStr, Type dicType) {

            if (dicType.IsGenericType && dicType.Name.StartsWith("Dictionary")) { //目标类型限定为 Dictionary<TKey,TValue>
                try {
                    var paramTypes = dicType.GetGenericArguments(); //获得泛型的参数化类型集合，即 TKey 和 TValue 
                    var typeKey = paramTypes[0];
                    var typeValue = paramTypes[1];
                    var obj = Activator.CreateInstance(dicType); //创建新的泛型对象
                    var dic = obj as IDictionary; //转换为接口：字典

                    for (int i = 0; i < arrStr.Length / 2; i++) { //遍历字符串数组，每两个一组
                        var key = arrStr[i * 2].ParseStringToBasicType(typeKey); //解析字符串为基础类型
                        var value = arrStr[i * 2 + 1].ParseStringToBasicType(typeValue);
                        dic.Add(key, value); //添加键值对到字典中
                    }
                    return dic;
                }
                catch {
                }
            }

            return null; //发生错误，转换失败
        }

        #endregion

    }// class StringExtensionMethods


    #region 辅助类

    /// <summary>
    /// 辅助类：TryParse 方法集合。包含字典：《类型名，TryParse 方法》。可通过扩展方法，找到值类型对应的 TryParse 方法
    /// </summary>
    public static class ParseStringHelper {

        #region 1. 字段

        // 私有字段：方法字典 <类型名，TryParse方法>
        private static Dictionary<string, MethodInfo> _dic_TypeName_Method = new Dictionary<string, MethodInfo>();

        #endregion


        #region 4. 主要功能：根据输入的值类型信息，找到该类型的 TryParse 方法

        /// <summary>
        /// Type 类型的扩展方法：根据输入的值类型信息，找到该类型的 TryParse 方法
        /// </summary>
        /// <param name="t">类型信息，限定为值类型</param>
        /// <returns>类型的 TryParse 方法</returns>
        public static MethodInfo GetTryParseMethod(this Type t) {

            //方法字典未添加该类型，先注册
            if (!_dic_TypeName_Method.ContainsKey(t.FullName)) {

                //反射获取基本类型的 TryParse 方法：public static TryParse(string, out T)
                var method = t.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                    new Type[] { typeof(string), t.MakeByRefType() },
                    new ParameterModifier[] { new ParameterModifier(2) });

                //在字典中注册属性列表，下次使用时直接通过索引找到
                _dic_TypeName_Method.Add(t.FullName, method);
            }

            return _dic_TypeName_Method[t.FullName]; //从字典中，找到类型的 TryParse 方法
        }
        #endregion

    }// class MethodTryParseSet

    #endregion

}// namespace
