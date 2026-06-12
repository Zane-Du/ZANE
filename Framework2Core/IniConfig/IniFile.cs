using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Framework2Core {

    /// <summary>
    /// 类：读写 Ini 配置文件。可按照 section、identity 方式逐个读写；
    /// 也可以一次读写 section 下的所有 identity，对应数据类型为字典：《string, string》
    /// </summary>
    public class IniFile {

        #region 1. 普通属性

        /// <summary>
        /// 只读属性：Ini 文件的全路径名，仅在构造函数中初始化
        /// </summary>
        public string FileFullName { get; } 

        #endregion


        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：根据文件路径名，创建 Ini 文件对象；若路径不存文件，则创建
        /// </summary>
        /// <param name="path">文件路径名，可使用相对路径或绝对路径</param>
        public IniFile(string path) {

            FileInfo fileInfo = new FileInfo(path);
            FileFullName = fileInfo.FullName; //获得配置文件的全路径名

            // 若不存在目录，则新建目录
            var dirInfo = fileInfo.Directory;
            if (!dirInfo.Exists) {
                dirInfo.Create();
            }
            
            //若不存在 ini 文件，则创建文件
            if (!fileInfo.Exists) {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding(0))) { //使用默认编码
                    try {
                        sw.Write("#参数配置文件\r\n");
                        sw.Close();
                    }
                    catch {
                        MessageBox.Show("无法读取文件：\r\n" + FileFullName + "\r\n\r\n错误原因：文件已损坏或移除", "配置文件读取错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }                
            }            
        }

        #endregion


        #region 4. 主要功能：读写单个Identity

        /// <summary>
        /// 主要功能：方法：从 Ini 文件指定 section 的 identity 中，读出 String 类型的值。如读取失败，返回空字符串
        /// </summary>
        public string ReadString(string section, string id) {

            byte[] buffer = new byte[5000];
            //调用非托管dll库中的方法，返回缓存数组 buffer 及有效数据长度 validLength （不包括结尾的 \0）
            int validLength = GetPrivateProfileString(section, id, "", buffer, buffer.Length, FileFullName);

            //从缓存数组 buffer 的索引=0开始，截取长度为 validLength 的数据，并将其编码为字符串
            string str = Encoding.GetEncoding(0).GetString(buffer, 0, validLength);

            //去除 \0 及首尾空格，结果作为此 identity 读取的 value 返回
            return str.Replace("\0","").Trim();
        }


        /// <summary>
        /// 主要功能：泛型方法：从 Ini 文件指定 section 的 identity 中，读出指定类型 T 的值。如读取失败，返回类型 T 的默认值。
        /// 支持的类型包括：string、值类型（枚举类型，int，double 等）、数组、泛型（List，Dictionary）、AbstractOptions 的子类。
        /// </summary>
        public T ReadValue<T>(string section, string id) {

            string str = ReadString(section, id); //从给定的 setction、id 中读取string
            var parsedValue = str.ParseStringToTargetType(typeof(T)); //使用扩展方法，解析字符串为目标类型
            if (parsedValue!=null) {
                return (T)parsedValue; //将 object 转换为 类型 T
            }

            return default(T); //读取失败，返回类型默认值
        }


        /// <summary>
        /// 主要功能：方法：向 Ini 文件指定 section 的 identity，写入 String 类型的值。
        /// </summary>
        public void WriteString(string section, string identity, string value) {

            //调用非托管dll库中的方法，向指定 section 中的 identity，写入值 value
            if (!WritePrivateProfileString(section, identity, value, FileFullName)) {
                MessageBox.Show("无法写入 Ini 文件：\r\n" + FileFullName + "\r\n\r\n错误原因：文件已损坏或移除", "配置文件写入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 主要功能：泛型方法：向 Ini 文件指定 section 的 identity，写入对象 ToString 的字符串。
        /// 支持的类型包括：string、值类型（枚举类型，int，double 等）、数组、泛型（List，Dictionary）、AbstractOptions 的子类。
        /// </summary>
        public void WriteValue<T>(string section, string id, T value) {
            string strValue = IniConfigPropManager.ConvertActualValueToString(typeof(T), value); //将实际值 ToString 为字符串
            WriteString(section, id, strValue); //向给定的 setction、id 中写入string
        }

        #endregion


        #region 4. 主要功能：读写 section下的所有 identity

        /// <summary>
        /// 主要功能：方法：读取 Ini 文件下的指定 section，返回所有 identity 的列表
        /// </summary>
        public List<string> GetIdListFromSection(string section) {

            //调用非托管dll库中的方法，identity=null，表示读取该section下的所有identity
            //返回缓存数组 buffer 及有效数据长度 validLength（包括结尾的 \0）
            byte[] buffer = new byte[50000];
            int validLength = GetPrivateProfileString(section, null, "", buffer, buffer.Length, FileFullName);

            //从缓存中将各字符串依次提取出来，并添加到列表 List<string>
            List<string> listIds= ExtractStringListFromBuffer(buffer, validLength);
            return listIds;
        }


        /// <summary>
        /// 主要功能：方法：读取 Ini 文件下的指定 section，返回字典：《identity, value》
        /// </summary>
        public Dictionary<string, string> GetIdValueDicFromSection(string section) {
            
            var listIds = GetIdListFromSection(section); //获得指定 section 下的 identity 列表

            //遍历列表，依次读取每个 identity 的 value，将其添加到字典中
            Dictionary<string, string> dic = new Dictionary<string, string>(); //创建字典
            foreach (var id in listIds) {
                dic.Add(id, ReadString(section, id)); 
            }
            return dic;
        }


        /// <summary>
        /// 主要功能：方法：给定一个字典《identity, value》，全部写入 Ini 文件下的指定 section 中
        /// </summary>
        public bool WriteIdValueDicToSection(string section, Dictionary<string, string> dic) {
            bool bError = false;

            if (dic != null && dic.Count > 0) {
                foreach (var item in dic) { //遍历字典，依次写入
                    try {
                        WriteString(section, item.Key, item.Value); //section, identity, value
                    }
                    catch {
                        bError = true; //发生错误
                    }                    
                }
            }

            return !bError;
        }

        #endregion


        #region 4. 功能：判断、删除 identity

        /// <summary>
        /// 功能：方法：判断指定 section 下的指定 identity 是否存在
        /// </summary>
        public bool IfExistsId(string section, string id) {
            var listIds = GetIdListFromSection(section); //获得 section 下的 identity 列表
            return listIds.Contains(id);
        }


        /// <summary>
        /// 功能：方法：删除指定 section 下的 identity
        /// </summary>
        public void DeleteId(string section, string id) {
            WritePrivateProfileString(section, id, null, FileFullName); //向 identity 写 null，表示删除
        }

        #endregion


        #region 4. 功能：操作文件中的 section

        /// <summary>
        /// 功能：方法：返回 Ini 文件中的 section 列表，类型为 List《string》
        /// </summary>
        public List<string> GetSectionList() {
            byte[] buffer = new byte[50000];
            //实现方式与方法 GetListIdsFromSection 类似，section、identity=null，表示读取所有 section
            int validLength = GetPrivateProfileString(null, null, "", buffer, buffer.Length, FileFullName);
            List<string> listSections = ExtractStringListFromBuffer(buffer, validLength);
            return listSections;
        }


        /// <summary>
        /// 功能：方法：清除 Ini 文件中的指定 section
        /// </summary>
        public void EraseSection(string section) {
            //identity 和 value 都为 null，表示清除所有 identity
            if (!WritePrivateProfileString(section, null, null, FileFullName)) { 
                MessageBox.Show("无法清除 Ini 文件中的 Section：\r\n" + FileFullName + "\r\n\r\n错误原因：文件已损坏或移除", "配置文件操作错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 功能：方法：清除 Ini 文件中的所有 section
        /// </summary>
        public void EraseAllSections() {
            var listSections = GetSectionList(); //获得 Ini 文件中的 section 列表
            foreach (string section in listSections) { //遍历 section 列表，逐个清除
                EraseSection(section);
            }
        }

        #endregion


        #region 6. 私有方法：内部调用

        /// <summary>
        /// 私有方法：写入 Ini 文件：调用非托管dll库
        /// </summary>
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string id, string value, string filePath);


        /// <summary>
        /// 私有方法：读取 Ini 文件：调用非托管dll库
        /// </summary>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string id, string defaultValue, byte[] retVal, int size, string filePath);


        /// <summary>
        /// 私有方法：解析缓存的字节数组，得到字符串列表
        /// </summary>
        /// <param name="buffer">被解析的字符数组</param>
        /// <param name="validLength">有效数据的长度，包括每个字符串结尾的 '\0'</param>
        /// <returns>解析得到的字符串列表</returns>
        private List<string> ExtractStringListFromBuffer(byte[] buffer, int validLength) {

            if (validLength > 0) {

                // 将 byte[] 编码为 string
                string s = Encoding.GetEncoding(0).GetString(buffer, 0, validLength);

                // 以 '\0' 来拆分字符串，得到 string[]
                string[] strArray = s.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

                // 将 string[] 生成为 List<string>
                var list = new List<string>(strArray);

                return list;
            }
            else {
                return new List<string>();
            }
        } 

        #endregion        

	}// class

}// namespace
