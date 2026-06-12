using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace Framework2Core
{

    /// <summary>
    /// 属性设置窗口：根据对象可配置的属性，自动生成相应的控件；可载入对象的属性值到控件，也可以将控件的值设置给属性，并同步写入本地文件中
    /// </summary>
    public partial class FormSetConfig : Form
    {

        #region 1. 字段、普通属性

        // 字段：被设置的对象、类型，及其对应的 section
        private object _configObj;
        private Type _configObjType;
        private string _configSectionName;
        private string[] _configArrLimits;
        private List<PropertyInfo> _configListProps; //被配置的属性列表，由 strLimits 限定
        public bool _bbtnSave = false; //判断是否点击保存更新

        //定义属性值改变事件委托
        public delegate void PropChangeDelegate(object sender);
        public static event PropChangeDelegate PropChangeEventHandler;
        private static Dictionary<string, string> _comboBoxAttributeToVal;
        /// <summary>
        /// 只读属性：设置属性的控件字典：《属性元数据, 控件》
        /// </summary>        
        public Dictionary<PropertyInfo, Control> DicPropControls
        {
            get
            {
                //在第一次访问时初始化包装的字段
                if (_dicPropControls == null)
                {
                    _dicPropControls = new Dictionary<PropertyInfo, Control>();

                    //遍历属性列表，依次在窗口中添加相应的控件
                    int countAddedControl = 0; //计数，用来计算控件的Y坐标
                    foreach (var prop in _configListProps)
                    {
                        int locationY = countAddedControl * 30 + 15;

                        //添加Label：属性名                    
                        Label lblPropName = new Label
                        {
                            Text = IniConfigFormManager.GetPropInfoName(prop),
                            Location = new Point(10, locationY + 3),
                            Size = new Size(130, 15)
                        };
                        this.CenterPanel.Controls.Add(lblPropName); //添加控件到面板显示


                        //添加Label：属性类型
                        // 对于字典，由于比较长，所以分为三行显示：Dictionary、TKey、TValue
                        var propTypeName = prop.PropertyType.GetTypeName();  //使用扩展方法，获得属性的类型名
                        var lblSize = new Size(100, 15);
                        if (propTypeName.StartsWith("Dictionary"))
                        {
                            string[] strs = propTypeName.Split(new char[] { ',', '<', '>', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (strs.Length > 2)
                            {
                                propTypeName = $"{strs[0]}\r\n<{strs[1]},\r\n{strs[2]}>";
                                lblSize = new Size(120, 40); //需要显示3行，因此这里高度设置为40
                            }
                        }
                        Label lblPropType = new Label
                        {
                            Text = propTypeName,
                            Location = new Point(380, locationY + 3),
                            Size = lblSize
                        };
                        this.CenterPanel.Controls.Add(lblPropType); //添加控件到面板显示


                        //根据属性类型，添加不同的配置控件：cmb、chk、txt、nud
                        Control ctrl = IniConfigFormManager.CreateConfigControl(prop, ref _comboBoxAttributeToVal);
                        if (ctrl != null)
                        {
                            if (ctrl is NumericUpDown)
                            {
                                ctrl.MouseWheel += (s, e) =>
                                 {
                                     HandledMouseEventArgs h = e as HandledMouseEventArgs;
                                     if (h != null)
                                     {
                                         h.Handled = true;
                                     }
                                 };
                            }
                            ctrl.Location = new Point(150, locationY);
                            if (ctrl.Size.Height == 50)
                            { //只有Dictionary的高度是50
                                countAddedControl++; //占位相当于两个控件
                            }
                            else if (ctrl is CheckBox)
                            {
                                ctrl.Size = new Size(15, 20);
                            }
                            else
                            {
                                ctrl.Size = new Size(200, 20); //其他控件高度都是20
                            }

                            //注册控件
                            this.CenterPanel.Controls.Add(ctrl); //注册控件到中央面板显示
                            _dicPropControls.Add(prop, ctrl); //注册到字典中：《属性元数据, 控件》
                        }
                        countAddedControl++;

                    }//foreach

                    //更新窗口的大小，不超过450×500
                    int newSizeY = countAddedControl * 30 + 80;
                    if (newSizeY > 500)
                    {
                        this.Size = new Size(500, 500);
                    }
                    else
                    {
                        this.Size = new Size(500, newSizeY);
                    }

                }//if

                return _dicPropControls;
            }
        }
        private Dictionary<PropertyInfo, Control> _dicPropControls;

        #endregion


        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：根据被设置的对象及类型，创建参数设置窗口
        /// </summary>
        /// <param name="obj">被设置的对象。对于静态类或静态属性，传入 null</param>
        /// <param name="objType">对象的类型，或静态类</param>
        /// <param name="sectionName">对象在 Ini 文件中对应的 section。对于静态类或静态属性，传入 "static"</param>
        /// <param name="formTitle">打开窗口后要显示的标题。默认为空字符串，不指定</param>
        /// <param name="arrLimits">限定属性范围。默认值 null，表示不限制</param>
        public FormSetConfig(object obj, Type objType, string sectionName, string formTitle = "", string[] arrLimits = null)
        {

            InitializeComponent(); //界面初始化：VS自动生成的代码

            _configObj = obj; //传入被设置的对象
            _configObjType = objType; //传入对象的类型
            _configSectionName = sectionName; //对象在ini文件中对应的 SectionName

            // 获取类的可配置属性列表，并限制在范围内
            _configArrLimits = arrLimits;
            _configListProps = IniConfigPropManager.GetListProps(objType, arrLimits, obj == null);

            _comboBoxAttributeToVal = new Dictionary<string, string>();

            // 设置窗体标题
            if (!string.IsNullOrEmpty(formTitle))
            {
                lblTitle.Text = formTitle;
            }
            this.Text = lblTitle.Text; // 更新窗口的 Text 属性

            // 如果该类型是可换型的，显示当前型号     
            if (_configObjType.IsModelChangeable() || _configObj is IVisionChange)
            {
                lblModelName.Text = ModelChangeVariables.当前型号.Text;
                lblStatus2.Visible = true;
                lblModelName.Visible = true;
            }
            else
            {
                lblStatus2.Visible = false;
                lblModelName.Visible = false;
            }

            // 如果该对象是视觉变量（继承自接口 IVisionChange），显示当前视觉名称
            if (_configObj is IVisionChange)
            {
                lblVisionName.Text = (_configObj as IVisionChange).VisionName;
                lblStatus3.Visible = true;
                lblVisionName.Visible = true;
            }
            else
            {
                lblStatus3.Visible = false;
                lblVisionName.Visible = false;
            }


            // 在窗口底部状态栏，显示被配置的类名和对象名
            lblTypeName.Text = _configObjType.Name; //类型名
            lblObjectName.Text = _configSectionName; //对象名
            _bbtnSave = false; //初次加载置false，点击保存事件后置true才发送
            LanguageChangeHandle();
            LanguageMenagement.LanguageChangeDeleg += LanguageChangeHandle;
        }

        #endregion


        #region 4. 主要功能：对象 → 界面：载入对象的参数到界面

        // 事件：对象 → 界面：窗体加载
        private void FormSetConfig_Load(object sender, EventArgs e)
        {
            SetPropValuesToControls(); //将对象的属性赋值给控件显示
        }


        // 事件：对象 → 界面：载入对象的参数到界面
        private void btnLoad_Click(object sender, EventArgs e)
        {
            SetPropValuesToControls(); //将对象的属性赋值给控件显示
            HZH_Controls.Forms.FrmTips.ShowTipsInfo(this, "已载入对象的当前属性值");
        }


        // 私有方法：读取对象的属性值，设置到界面的控件中
        private void SetPropValuesToControls()
        {

            lblTitle.Focus(); //让标题获得焦点

            // 遍历属性列表，依次将属性值设置到界面控件中
            foreach (var prop in _configListProps)
            {
                if (DicPropControls.ContainsKey(prop))
                {
                    Control ctrl = DicPropControls[prop]; //在控件字典中，根据 属性元数据 找到对应的 控件
                    var propType = prop.PropertyType;//属性类型

                    // 属性值 → 字符串：将被配置对象的 prop 属性值，转换为字符串
                    var strValue = IniConfigPropManager.ConvertPropToString(_configObj, prop);
                    if (ctrl is ComboBox)
                    {
                        if (propType.IsEnum)
                        {
                            var actualValue = prop.GetValue(_configObj); //获取该属性的实际值
                            Enum en = actualValue as Enum;
                            var filed = (en).GetType().GetField(en.ToString());
                            //var customAttribute = Attribute.GetCustomAttribute(filed, typeof(IniConfigAttribute));
                            // strValue = customAttribute == null ? filed.Name : (customAttribute as IniConfigAttribute).EnglishName;
                            strValue = filed.GetFieldInfoName();
                        }
                    }
                    // 字符串 → 控件：依次输入参数：控件，属性类型，属性值
                    IniConfigFormManager.SetStringValueToControl(ctrl, propType, strValue);
                }
            }
        }

        #endregion


        #region 4. 主要功能：界面 → 对象 → 本地文件：保存界面的设置，到对象，再到本地文件

        // 事件：界面 → 对象 → 本地文件：保存界面的设置，到对象，再到本地文件
        private void btnSave_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("请在专业的指导下，确认参数是否保存！", "保存确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            WriteControlValuesToIni();
            _bbtnSave = true; //初次加载置false，点击保存事件后置true才发送
        }


        // 私有方法：将界面控件的值读取到对象中，并同步保存到本地文件
        private bool WriteControlValuesToIni()
        {

            System.Text.StringBuilder SBLog = new System.Text.StringBuilder();
            string strLog = null;

            // 1. 遍历属性列表，依次将将界面控件中的值，设置给对象
            foreach (var prop in _configListProps)
            {
                if (DicPropControls.ContainsKey(prop))
                {

                    Control ctrl = DicPropControls[prop]; //在控件字典中，根据 属性元数据 找到对应的 控件

                    // 控件 → 字符串：获取控件的值到 string
                    var strValue = IniConfigFormManager.GetStringValueFromControl(ctrl);
                    //if (ctrl is ComboBox)
                    //{
                    //    strValue = _comboBoxAttributeToVal[strValue];
                    //}
                    // 字符串 → 属性：解析字符串，赋值给对象的属性
                    IniConfigPropManager.SetStringValueToProp(_configObj, prop, strValue, out strLog);
                    if (strLog != null)
                    {
                        SBLog.Append(strLog);
                        if (PropChangeEventHandler!= null &&  _configObjType.Name == "CalcResult")
                        {
                            PropChangeEventHandler(_configObj);
                        }
                    }
                }
            }
            if(SBLog.Length > 0)
            {
                string filePath = @".\配置操作日志\";
                if (!System.IO.Directory.Exists(filePath))
                    System.IO.Directory.CreateDirectory(filePath);  //如果文件不存在则新建

            filePath += DateTime.Now.ToString("yyyy-MM") + ".txt";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(DateTime.Now.ToString("G"));
                if (_configObj is IVisionChange)
                {
                    sw.WriteLine((_configObj as IVisionChange).VisionName);
                }
                sw.WriteLine(_configSectionName);
                sw.Write(SBLog.ToString());
                sw.WriteLine("-^8^--^*^--^-^--^.^--^8^--^*^--^-^--^.^-");
                sw.WriteLine();
            }
            }

            // 2. 重新将对象的属性赋值给控件显示
            try
            {
                SetPropValuesToControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取窗口控件参数失败！异常：\r\n" + ex.ToString());
            }

            // 3. 将对象的属性值写入到本地文件
            try
            {
                IniConfigPropManager.SaveConfigsToIni(_configObj, _configObjType, _configSectionName, _configArrLimits);
                HZH_Controls.Forms.FrmTips.ShowTipsSuccess(this, "保存参数到配置文件成功");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存参数到配置文件失败！异常：\r\n" + ex.ToString());
                return false;
            }
        }

        #endregion


        #region 5. 事件：窗体移动、关闭

        // 事件：鼠标按下
        private bool isLeftMouseDown = false; //鼠标左键是否按下        
        private Point mPoint; //记录鼠标按下位置
        private void windowMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            { // 判断鼠标按键
                isLeftMouseDown = true;
                mPoint = new Point(e.X, e.Y);  // 鼠标按下位置
            }
        }

        // 事件：鼠标移动
        private void windowMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftMouseDown)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        // 事件：鼠标释放
        private void windowMove_MouseUp(object sender, MouseEventArgs e)
        {
            isLeftMouseDown = false;
        }

        // 事件：鼠标离开窗口
        private void FormSetConfig_MouseLeave(object sender, EventArgs e)
        {
            isLeftMouseDown = false;
        }

        // 事件：关闭窗口，释放资源
        private void btnClose_Click(object sender, EventArgs e)
        {

            this.Close();
            this.Dispose(true); //关闭窗口后，释放此窗口的所有资源
            GC.Collect(); //GC回收
        }

        #endregion

        #region 6.语言切换事件
        private void LanguageChangeHandle()
        {
            if (LanguageMenagement.language == LanguageType.中文)
            {
                btnLoad.Text = "载入";
                btnSave.Text = "保存";
                lblStatus0.Text = "类型";
                lblStatus1.Text = "对象名";
                lblStatus2.Text = "型号";
                lblStatus3.Text = "视觉";
            }
            if (LanguageMenagement.language == LanguageType.English)
            {
                btnLoad.Text = "Reload";
                btnSave.Text = "Save";
                lblStatus0.Text = "Class";
                lblStatus1.Text = "ObjName";
                lblStatus2.Text = "Model";
                lblStatus3.Text = "Vision";
            }
        }
        #endregion

    }// class

}// namespace
