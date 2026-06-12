using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Framework2Core {

    /// <summary>
    /// 自定义控件：给定一个对象/类型，及其属性列表，以 DataGrid 形式显示该对象/类型的指定属性
    /// </summary>
    public partial class PropDataGrid : UserControl {

        #region 1. 字段

        // 字段：被设置的对象、类型，及其对应的 section、限定属性范围
        private object _configObj;
        private Type _configObjType;
        private string _configSectionName;
        private string[] _configArrLimits;
        private string[] _configArrSetLimits;
        
        #endregion


        #region 2. 构造函数

        // 实例构造函数。在父窗体构造函数中的 InitializeComponent 方法中调用
        public PropDataGrid() {
            InitializeComponent(); //初始化控件界面，VS自动生成的代码
            LanguageChangeHandle();
            LanguageMenagement.LanguageChangeDeleg += LanguageChangeHandle;

        }

        #endregion


        #region 4. 主要功能：以 DataGrid 形式显示对象/类型的属性

        /// <summary>
        /// 主要功能：方法：给定一个对象，及其属性列表，以 DataGrid 形式显示该对象的指定属性
        /// </summary>
        /// <param name="obj">要显示的对象</param>
        /// <param name="ctrlTitle">控件要显示的标题。默认为空字符串，不指定</param>
        /// <param name="strLimits">限定属性范围。默认值为 null，表示不限制</param>
        public void ShowObjConfigs(object obj, string sectionName, string ctrlTitle = "", string[] strLimits = null, string[] strSetLimits = null)
        {

            ShowConfigs(obj, obj.GetType(), sectionName, ctrlTitle, strLimits, strSetLimits);//对象传入参数：objType=obj.GetType()

        }

        /// <summary>
        /// 主要功能：方法：给定一个类型，及其静态属性列表，以 DataGrid 形式显示该类型的指定静态属性
        /// </summary>
        /// <param name="ctrlTitle">控件要显示的标题。默认为空字符串，不指定</param>
        /// <param name="strLimits">限定属性范围。默认值为 null，表示不限制</param>
        public void ShowStaticConfigs(Type staticType, string ctrlTitle = "", string[] strLimits = null) {

            ShowConfigs(null, staticType, "static", ctrlTitle, strLimits);//类型传入参数：obj=null，sectionName="static"

        }


        /// <summary>
        /// 主要功能：方法：给定一个对象/类型，及其属性列表，以 DataGrid 形式显示该对象/类型的指定属性
        /// </summary>
        /// <param name="obj">要显示的对象。对于静态类或静态属性，传入 null</param>
        /// <param name="objType">对象的类型，或静态类</param>
        /// <param name="ctrlTitle">控件要显示的标题。默认为空字符串，不指定</param>
        /// <param name="arrLimits">限定属性范围。默认值 null，表示不限制</param>
        public void ShowConfigs(object obj, Type objType, string sectionName, string ctrlTitle = "", string[] arrLimits = null, string[] arrSetLimits = null) {

            // 输入类型不能为空
            if (objType == null) {
                return;
            }

            _configObj = obj; //传入被设置的对象
            _configObjType = objType; //传入对象的类型
            _configSectionName = sectionName; //对象在ini文件中对应的 SectionName
            _configArrLimits = arrLimits; //限定属性范围
            _configArrSetLimits = arrSetLimits; //限定属性范围


            // 获取类的可配置属性列表，并限制在范围内
            var listProps = IniConfigPropManager.GetListProps(objType, arrLimits, obj == null);

            // 设置控件标题
            if (!string.IsNullOrEmpty(ctrlTitle)) {
                lblTitle.Text = ctrlTitle;
            }

            // 如果该类型是可换型的，显示当前型号                        
            if (objType.IsModelChangeable() || obj is IVisionChange) {                
                lblModelName.Text = LanguageMenagement.language == LanguageType.中文? "型号:":"Model"+ModelChangeVariables.当前型号.Text;
                lblModelName.Visible = true;
            }
            else {
                lblModelName.Visible = false;
            }

            // 如果该对象是视觉变量（继承自接口 IVisionChange），显示当前视觉名称
            if (obj is IVisionChange) {
                lblVisionName.Text = LanguageMenagement.language == LanguageType.中文 ? "视觉:":"Vision" +(obj as IVisionChange).VisionName;
                lblVisionName.Visible = true;
            }
            else {
                lblVisionName.Visible = false;
            }


            // 清除表格中原有旧行
            innerDgv.Rows.Clear();

            // 遍历属性列表，将对象/类的属性转换为 string，然后逐行添加到 dgv 中显示
            for (int i = 0; i < listProps.Count; i++) {
                var prop = listProps[i];


                string strValue = IniConfigPropManager.ConvertPropToString(obj, prop); // 将 obj 的 prop 属性的值，转换为字符串
                // 使用扩展方法，获得属性的类型名
                var propTypeName = prop.PropertyType.GetTypeName();
                if (prop.PropertyType.IsEnum)
                {
                    var actualValue = prop.GetValue(obj); //获取该属性的实际值
                    Enum en = actualValue as Enum;
                    var filed = (en).GetType().GetField(en.ToString());
                    //var customAttribute = Attribute.GetCustomAttribute(filed, typeof(IniConfigAttribute));
                    //strValue = customAttribute == null ? filed.Name : (customAttribute as IniConfigAttribute).EnglishName;
                    strValue = filed.GetFieldInfoName();
                }
                // 构造新行
                string[] strPropInfo = new string[] {
                    //prop.Name, //属性名称                    
                    IniConfigFormManager.GetPropInfoName(prop),
                    strValue,
                    propTypeName //属性类型名
                };

                //向 DataGrid 插入新行
                innerDgv.Rows.Insert(i, strPropInfo); //新增一行，现在总行数变为 i+1 
            }
        }

        #endregion

        #region 4. 主要功能：清空显示

        /// <summary>
        /// 清除表格中显示的行
        /// </summary>
        public void Clear() {

            innerDgv.Rows.Clear();

            lblTitle.Text = "参数";
            lblModelName.Visible = false;
            lblVisionName.Visible = false;

            _configObj = null;
            _configObjType = null;
            _configSectionName = null;
            _configArrLimits = null;

        }

        #endregion


        #region 5. 事件：弹窗设置对象/类型的属性

        // 事件：弹窗设置对象/类型的属性。窗口中的控件由 FormConfig 自动生成。设置完成后刷新显示属性值
        private void btnSet_BtnClick(object sender, EventArgs e)
        {

            //设置对象：使用当前控件的 SectionName、标题，以及限定属性范围
            if (_configObj != null)
            {
                if (_configArrSetLimits == null || _configArrSetLimits.Length == 0)
                {
                    _configArrSetLimits = _configArrLimits;
                }
                _configObj.SetObjConfigs(_configSectionName, lblTitle.Text, _configArrSetLimits);
            }

            //设置静态属性：使用当前控件的标题，以及限定属性范围
            else if (_configObjType != null)
            {
                _configObjType.SetStaticConfigs(lblTitle.Text, _configArrLimits);
            }
            else
            {
                return; //如果传入的类型为空，直接退出
            }

            //设置后重新显示
            ShowConfigs(_configObj, _configObjType, _configSectionName, lblTitle.Text, _configArrLimits, _configArrSetLimits);
        }
        #endregion

        #region 6语言切换事件处理器
        public void LanguageChangeHandle()
        {
            if (this.innerDgv.Columns.Count <1)
            {
                return;
            }
            if (LanguageMenagement.language == LanguageType.中文)
            {
                this.innerDgv.Columns["属性名称"].HeaderText = "属性名称";
                this.innerDgv.Columns["属性值"].HeaderText = "属性值";
                this.innerDgv.Columns["属性类型"].HeaderText = "属性类型";
                this.btnSet.BtnText = "设置";
                
            }
            if (LanguageMenagement.language == LanguageType.English)
            {

                this.innerDgv.Columns["属性名称"].HeaderText = "PropName";
                this.innerDgv.Columns["属性值"].HeaderText = "PropValue";
                this.innerDgv.Columns["属性类型"].HeaderText = "PropClass";
                this.btnSet.BtnText = "Set";
            }

            //设置后重新显示
            ShowConfigs(_configObj, _configObjType, _configSectionName, lblTitle.Text, _configArrLimits);
        }

        public void ParentForm_FormClosing(object sender,EventArgs e)
        {
            LanguageMenagement.LanguageChangeDeleg -= LanguageChangeHandle;
        }
        #endregion

        private void PropDataGrid_ParentChanged(object sender, EventArgs e)
        {
            this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
        }
    }// class

}// namespace
