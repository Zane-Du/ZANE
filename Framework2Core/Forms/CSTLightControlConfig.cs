using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Framework2Core {

    /// <summary>
    /// 属性设置窗口：根据对象可配置的属性，自动生成相应的控件；可载入对象的属性值到控件，也可以将控件的值设置给属性，并同步写入本地文件中
    /// </summary>
    public partial class CSTLightControlConfig : Form {

        #region 5. 事件：窗体移动、关闭

        // 事件：鼠标按下
        private bool isLeftMouseDown = false; //鼠标左键是否按下        
        private Point mPoint; //记录鼠标按下位置
        private void windowMove_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) { // 判断鼠标按键
                isLeftMouseDown = true;                
                mPoint = new Point(e.X, e.Y);  // 鼠标按下位置
            }
        }

        // 事件：鼠标移动
        private void windowMove_MouseMove(object sender, MouseEventArgs e) {
            if (isLeftMouseDown) {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        // 事件：鼠标释放
        private void windowMove_MouseUp(object sender, MouseEventArgs e) {
            isLeftMouseDown = false;
        }

        // 事件：鼠标离开窗口
        private void FormSetConfig_MouseLeave(object sender, EventArgs e) {
            isLeftMouseDown = false;
        }

        // 事件：关闭窗口，释放资源
        private void btnClose_Click(object sender, EventArgs e) {

            this.Close();
            this.Dispose(true); //关闭窗口后，释放此窗口的所有资源
            GC.Collect(); //GC回收
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
        }
    }// class

}// namespace
