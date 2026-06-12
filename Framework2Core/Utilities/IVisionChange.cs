using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework2Core {
    /// <summary>
    /// 接口：视觉相关的参数都实现自此接口，表示此参数有两个维度的变化：产品型号、视觉名称
    /// </summary>
    public interface IVisionChange {

        /// <summary>
        /// 接口 IVisionChange 定义的属性：视觉名称
        /// </summary>
        string VisionName { get; set; } 

    }
}
