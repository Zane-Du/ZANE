using System;

namespace Framework2Core
{
    /// <summary>
    /// 特性：标记于属性上，表示此属性可以被Ini文件配置。
    /// 相关的类包括：IniFile（文件读写）；IniConfigPropManager（属性管理）；IniConfigFormManager（设置窗口管理）
    /// </summary>
    public class IniConfigAttribute : Attribute
    {
        private string _EnglishName;
        /// <summary>
        /// 获取属性需要在属性编辑器上显示的名称
        /// </summary>
        public string EnglishName
        {
            get { return _EnglishName; }
        }
        private string _ChineseName;

        public string ChineseName
        {
            get { return _ChineseName; }
            set { _ChineseName = value; }
        }

        public IniConfigAttribute(string strEnglishName = null, string strChineseName = null)
        {
            this._EnglishName = strEnglishName;
            this._ChineseName = strChineseName;
        }
    }// class

    /// <summary>
    /// 特性：标记于属性上，不显示某些属性参数
    /// </summary>
    public class NotDisplayAttribute : Attribute
    {

    }// class


    /// <summary>
    /// 特性：标记于属性上，用于类型参数设置。
    /// </summary>
    public class ModelAttribute : Attribute
    {

    }// class

    /// <summary>
    /// 特性：标记于属性上，用于管控参数设置。
    /// </summary>
    public class ParameterAttribute : Attribute
    {

    }// class

    /// <summary>
    /// 特性：标记于属性上，用于Mes参数设置。
    /// </summary>
    public class MesSettingAttribute : Attribute
    {

    }// class

    /// <summary>
    /// 特性：标记于属性上，用于岗位参数设置。
    /// </summary>
    public class StationSettingAttribute : Attribute
    {

    }// class
     // 

    ///// <summary>
    ///// 特性：标记于属性上，用于换型配置参数设置。
    ///// </summary>
    public class Model2Attribute : Attribute
    {

    }// class
}// namespace
