using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Data;


namespace Framework2Core
{

    public static class LanguageMenagement
    {
        //定义属性值改变事件委托
        public delegate void LanguageChangeEvent();
        public static event LanguageChangeEvent LanguageChangeDeleg;

        static LanguageMenagement()
        {
            Type staticType = typeof(LanguageMenagement); //本静态类的类型

            //加载配置文件
            staticType.LoadStaticConfigsFromIni();

            //保存配置文件
            staticType.SaveStaticConfigsToIni();
        }

        [IniConfig("Language", "语言")]
        public static LanguageType language
        {
            get => _language;
            set
            {
                _language = value;
                LanguageChangeDeleg?.Invoke();
            }
        }
        private static LanguageType _language = LanguageType.中文;
    }
    public enum LanguageType
    {
        [IniConfig("Chinese")]
        中文 = 0,
        [IniConfig("English")]
        English = 1,
    }

    /// <summary>
    /// 多语言切换辅助类：
    /// 思路：使用不同的XML配置文件来映射不同的语言，窗体加载时从默认语言DefultLanguage.xml中读取配置
    /// 中文语言    对应Chinese.xml
    /// 英文语言    对应English.xml
    /// 比如一个Button控件btnLogin，因为控件的变量名是绝对唯一的
    /// 我们可以通过键值对字典来进行处理，键名都是控件的变量名称，值为控件的文本内容
    /// 中文语言Chinese.xml     设置Name="btnLogin" Text="登录"
    /// 英文语言English.xml     设置Name="btnLogin",Text="Login"
    /// </summary>
    public class MultiLanguageUtil
    {
        private static string _languageFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}Language\\";
        public static string GetDefultLanguage()
        {
            string defualtLanguage = "Chinese";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(_languageFilePath + "DefaultLanguage.xml", settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            XmlNode root = doc.DocumentElement;
            //默认语言节点
            XmlNode node = root.SelectSingleNode("DefaultLanguage");
            if (node != null)
            {
                defualtLanguage = node.InnerText;
            }
            reader.Close();
            reader.Dispose();
            return defualtLanguage;
        }

        /// <summary>
        /// 设置默认语言
        /// </summary>
        /// <param name="defaultLanguage"></param>
        public static void SetDefaultLanguage(string defaultLanguage)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(_languageFilePath + "DefaultLanguage.xml");
            DataTable dt = ds.Tables["Root"];
            dt.Rows[0]["DefaultLanguage"] = defaultLanguage;
            ds.AcceptChanges();
            ds.WriteXml(_languageFilePath + "DefaultLanguage.xml");
        }

        public static void SetFormLanguage(Form form)
        {
            WriteFormCtrlsToXml(form);
            SetControlsTextFromXml(form, (LanguageMenagement.language == LanguageType.中文 ? "Chinese" : "English"));
        }



        public static void WriteFormCtrlsToXml(Form form)
        {
            string formName = form.GetType().ToString();
            string xmlPath = _languageFilePath + $"{formName}.xml";
            if (System.IO.File.Exists(xmlPath))
            {
                return;
            }
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(xmlPath)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(xmlPath));
            }
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.IndentChars = "\t";

            XmlWriter xmlWriter = XmlWriter.Create(xmlPath, settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Root");
           
            xmlWriter.WriteAttributeString("Language", "Chinese");
            xmlWriter.WriteStartElement("Form");
            xmlWriter.WriteStartElement("FormName");
            xmlWriter.WriteAttributeString("Name", formName);
            xmlWriter.WriteAttributeString("Text", form.Text);
            xmlWriter.WriteEndElement();    //FormName
            xmlWriter.WriteStartElement("Controls");
            WriteCtrlChildsToXml(form, xmlWriter);
            xmlWriter.WriteEndElement();//Controls
            xmlWriter.WriteEndElement();//Form
            xmlWriter.WriteEndElement();//Root
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        private static void WriteCtrlChildsToXml(Control ctrl, XmlWriter xmlWriter)
        {
            if (ctrl == null || ctrl.Controls.Count == 0)
            {
                return;
            }
            if (ctrl is UserControl)
            {
                string controlName = ctrl.GetType().FullName.ToString();
                xmlWriter.WriteStartElement("UserControlName");
                xmlWriter.WriteAttributeString("Name", ctrl.Name);
                xmlWriter.WriteAttributeString("Text", ctrl.Text);
                xmlWriter.WriteStartElement("Controls");
            }
            foreach (var child in ctrl.Controls)
            {
                if (child is ToolStrip)
                {
                    ToolStrip tmpToolStrip = (ToolStrip)child;
                    xmlWriter.WriteStartElement("Control");
                    xmlWriter.WriteAttributeString("Name", tmpToolStrip.Name);
                    xmlWriter.WriteAttributeString("Text", tmpToolStrip.Text);
                    xmlWriter.WriteEndElement();
                    WriteToolStripStatusLabelToXml(tmpToolStrip, xmlWriter);
                }
                else
                {
                    xmlWriter.WriteStartElement("Control");
                    //xmlWriter.WriteAttributeString("ParentName", (child as Control).Parent.Name);
                    xmlWriter.WriteAttributeString("Name", (child as Control).Name);
                    xmlWriter.WriteAttributeString("Text", (child as Control).Text);
                    xmlWriter.WriteEndElement();
                    WriteCtrlChildsToXml(child as Control, xmlWriter);
                }
            }
            if (ctrl is UserControl)
            {
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
        }

        private static void WriteToolStripStatusLabelToXml(ToolStrip toolStrip, XmlWriter xmlWriter)
        {
            if (toolStrip == null || toolStrip.Items.Count == 0)
            {
                return;
            }
            
            foreach (var item in toolStrip.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem temp = (ToolStripMenuItem)item;
                    xmlWriter.WriteStartElement("Control");
                    xmlWriter.WriteAttributeString("Name", temp.Name);
                    xmlWriter.WriteAttributeString("Text", temp.Text);
                    xmlWriter.WriteEndElement();
                    WriteToolStripMenuItemToXml(temp, xmlWriter);
                }
                else
                {
                    xmlWriter.WriteStartElement("Control");
                    xmlWriter.WriteAttributeString("Name", (item as ToolStripItem).Name);
                    xmlWriter.WriteAttributeString("Text", (item as ToolStripItem).Text);
                    xmlWriter.WriteEndElement();
                    WriteToolStripStatusLabelToXml(item as ToolStrip, xmlWriter);
                }
            }
        }

        private static void WriteToolStripMenuItemToXml(ToolStripMenuItem toolStrip, XmlWriter xmlWriter)
        {
            
            if (toolStrip == null || toolStrip.DropDownItems.Count == 0)
            {
                return;
            }
            for (int i = 0; i < toolStrip.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item = toolStrip.DropDownItems[i] as ToolStripMenuItem;
                xmlWriter.WriteStartElement("Control");
                xmlWriter.WriteAttributeString("Name", item.Name);
                xmlWriter.WriteAttributeString("Text", item.Text);
                xmlWriter.WriteEndElement();
                WriteToolStripMenuItemToXml(item, xmlWriter);
            }

        }
       

        public static void SetControlsTextFromXml(Form form,string language)
        {
            string formName = form.GetType().ToString();
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                //判断是否存在该语言的配置文件 
                if (!System.IO.File.Exists(_languageFilePath + $"{language}.xml"))
                {
                    return;
                }
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(_languageFilePath + $"{language}.xml", settings);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XmlNode root = doc.DocumentElement;
                //获取XML文件中对应窗口的内容
                XmlNode nodeFind = root.SelectSingleNode($"Form/FormName[@Name='{formName}']");
                if (nodeFind == null)
                {
                    //如果没有配置该窗体的语言描述，就返回空
                    return;
                }
                form.Text = nodeFind.SelectSingleNode("@Text").InnerText;

                SetCtrlChildsTextFormXml(form, nodeFind.ParentNode);
                reader.Close();
                reader.Dispose();
                return;
            }
            catch (Exception)
            {
                return;
            }
        }
        private static void SetCtrlChildsTextFormXml(Control ctrl,XmlNode nodeFind)
        {
            if (ctrl == null)
            {
                return;
            }
            XmlNode tempNode = nodeFind;
            if(tempNode == null)
            {
                return;
            }
            if (ctrl is UserControl)
            {
                tempNode = nodeFind.SelectSingleNode($"Controls/UserControlName[@Name='{ctrl.Name}']");
                if(tempNode != null)
                {
                    string strText = tempNode.SelectSingleNode("@Text").InnerText;
                    if (!string.IsNullOrEmpty(strText))
                        ctrl.Text = strText;
                }
            }
            else
            {
                tempNode = nodeFind.SelectSingleNode($"Controls/Control[@Name='{ctrl.Name}']");
                if(tempNode != null)
                {
                    string strText = tempNode.SelectSingleNode("@Text").InnerText;
                    if (!string.IsNullOrEmpty(strText))
                        ctrl.Text = strText;
                }
                tempNode = nodeFind;
            }
            if(tempNode == null)
            {
                tempNode = nodeFind;
            }
            if (ctrl is TabPage)
            {
                int a = 1 + 1;
            }
            
            foreach (var child in ctrl.Controls)
            {
                if (tempNode == null)
                {
                    continue;
                }
                if (child is ToolStrip)
                {
                    SetToolStripChildsTextFromXml(child as ToolStrip, tempNode);
                }
                else
                {
                    SetCtrlChildsTextFormXml(child as Control, tempNode);
                }
            }
        }

        private static void SetToolStripChildsTextFromXml(ToolStrip toolStrip, XmlNode nodeFind)
        {
            if (toolStrip != null)
            {
                XmlNode currentNode = nodeFind.SelectSingleNode($"Controls/Control[@Name='{toolStrip.Name}']");
                if (currentNode != null)
                    toolStrip.Text = currentNode.SelectSingleNode("@Text").InnerText;
            }
            if (toolStrip == null || toolStrip.Items.Count == 0)
            {
                return;
            }
            foreach (var item in toolStrip.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    SetMenultemChildsTextFromXml(item as ToolStripMenuItem, nodeFind);
                }
                else
                {
                    XmlNode currentNode = nodeFind.SelectSingleNode($"Controls/Control[@Name='{(item as ToolStripItem).Name}']");
                    if(currentNode != null)
                    {
                        string strText = currentNode.SelectSingleNode("@Text").InnerText;
                        if (!string.IsNullOrEmpty(strText))
                            (item as ToolStripItem).Text = strText;
                    }
                    SetToolStripChildsTextFromXml(item as ToolStrip, nodeFind);
                }
            }
        }

        private static void SetMenultemChildsTextFromXml(ToolStripMenuItem toolStripMenuItem, XmlNode nodeFind)
        {
            if (toolStripMenuItem != null)
            {
                XmlNode currentNode = nodeFind.SelectSingleNode($"Controls/Control[@Name='{toolStripMenuItem.Name}']");
                if (currentNode != null)
                {
                    string strText = currentNode.SelectSingleNode("@Text").InnerText;
                    if (!string.IsNullOrEmpty(strText))
                        toolStripMenuItem.Text = strText;
                }
            }
            if (toolStripMenuItem == null || toolStripMenuItem.DropDownItems.Count == 0)
            {
                return;
            }
            for (int i = 0; i < toolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item = toolStripMenuItem.DropDownItems[i] as ToolStripMenuItem;
                XmlNode currentNode = nodeFind.SelectSingleNode($"Controls/Control[@Name='{(item as ToolStripMenuItem).Name}']");
                if( currentNode != null)
                {
                    string strText = currentNode.SelectSingleNode("@Text").InnerText;
                    if (!string.IsNullOrEmpty(strText))
                        (item as ToolStripMenuItem).Text = strText;
                }

                SetMenultemChildsTextFromXml(item, nodeFind);
            }
        }
    }
}
