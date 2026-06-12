using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Framework2Core
{
    public partial class Frm_LightControllerCST : Form
    {

       // IniFile iniLight = new IniFile(Application.StartupPath+ @"\ApplicationConfig\Device\LightControllerCSTConfig.ini");
        private int[] 通道亮度 = new int[8];
        public Frm_LightControllerCST()
        {
            InitializeComponent();
            //allLightController = allLightControll;
        }
        #region 窗体拖动
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Set_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left & this.WindowState == FormWindowState.Normal)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion
        //List<LightControllerCST> allLightController = new List<LightControllerCST>();
        private void Frm_LightControllerCST_Load(object sender, EventArgs e)
        {
            if (LightControllerCST._dicLightControllerCST != null && LightControllerCST._dicLightControllerCST.Count > 0)
            {
                foreach (LightControllerCST lightController in LightControllerCST._dicLightControllerCST.Values)
                {
                    cbx_ListLightController.Items.Add(lightController.DeviceName);
                }
                cbx_ListLightController.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 光源控制器s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbx_ListLightController_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (FindLightControllerClass(cbx_ListLightController.Text) == null)
                {
                    MessageBox.Show("无对应的光源控制器，请确认！", "错误提示！");
                    return;
                }
                UpdateStatus();
                Updown_channels.Value = FindLightControllerClass(cbx_ListLightController.Text).通道数;
                UpdateChannels();
            }
            catch (Exception)
            {
            }

        }
        /// <summary>
        /// 更新连接状态
        /// </summary>
        public void UpdateStatus()
        {

            if (FindLightControllerClass(cbx_ListLightController.Text).IsConnected)
            {
                lb_connectStus.Text = "已连接";
                lb_connectStus.BackColor = Color.Green;
                bt_OpenScanner.Enabled = false;

            }
            else
            {
                lb_connectStus.Text = "未连接";
                lb_connectStus.BackColor = Color.Red;
                bt_OpenScanner.Enabled = true;
            }

        }
        /// <summary>
        /// 查扫光源控制器，返回对应的实例化类
        /// </summary>
        /// <returns></returns>
        public LightControllerCST FindLightControllerClass(string deviceName)
        {
            if (LightControllerCST._dicLightControllerCST.ContainsKey(deviceName))
            {
                return LightControllerCST._dicLightControllerCST[deviceName];
            }
            return null;
        }


        /// <summary>
        /// 更新通道亮度
        /// </summary>
        public void UpdateChannels()
        {
            try
            {
                if (FindLightControllerClass(cbx_ListLightController.Text).通道数 > 0 && FindLightControllerClass(cbx_ListLightController.Text).通道数 < 9)
                {
                    for (int j = 0; j < FindLightControllerClass(cbx_ListLightController.Text).通道数; j++)
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control.Name == "brightness" + (j).ToString())
                            {
                                if (control is NumericUpDown)
                                {
                                    NumericUpDown t = (NumericUpDown)control;
                                    t.Enabled = true;
                                    t.ValueChanged -= new System.EventHandler(this.brightness_ValueChanged);
                                    t.Value = FindLightControllerClass(cbx_ListLightController.Text).通道亮度[j];
                                    t.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);
                                }
                            }
                        }
                    }
                    for (int i = FindLightControllerClass(cbx_ListLightController.Text).通道数; i < 8; i++)
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control.Name == "brightness" + (i).ToString())
                            {

                                if (control is NumericUpDown)
                                {
                                    NumericUpDown t = (NumericUpDown)control;
                                    t.Enabled = false;
                                    t.ValueChanged -= new System.EventHandler(this.brightness_ValueChanged);
                                    t.Value = 0;
                                    t.ValueChanged += new System.EventHandler(this.brightness_ValueChanged);

                                }
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
            }


        }

        //连接光源控制器
        private void bt_OpenScanner_Click(object sender, EventArgs e)
        {
            try
            {
                if (lb_connectStus.Text == "未连接")
                {
                    FindLightControllerClass(cbx_ListLightController.Text).Open();
                }
                UpdateStatus();
            }
            catch (Exception ex)
            {

                MessageBox.Show("连接光源控制器异常" + ex.Message, "错误提示");
            }
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_SaveParam_Click(object sender, EventArgs e)
        {
            try
            {
                if (FindLightControllerClass(cbx_ListLightController.Text)==null)
                {
                    MessageBox.Show("无对应的光源控制器，请确认！", "错误提示！");
                    return;
                }
                FindLightControllerClass(cbx_ListLightController.Text).通道数 = Convert.ToInt16(Updown_channels.Value);
                for (int i = 0; i < Convert.ToInt16(Updown_channels.Value); i++)
                {
                    foreach (Control control in this.Controls)
                    {
                        if (control.Name == "brightness" + (i).ToString())
                        {
                            if (control is NumericUpDown)
                            {
                                NumericUpDown t = (NumericUpDown)control;
                                通道亮度[i] = Convert.ToInt16(t.Value);
                                FindLightControllerClass(cbx_ListLightController.Text).通道亮度[i] = 通道亮度[i];
                            }
                        }
                    }
                }

                //FindLightControllerClass(cbx_ListLightController.Text).SaveOtherToConfigs(ModelChangeVariables.当前型号.Text);
                //写入配置
                FindLightControllerClass(cbx_ListLightController.Text).SaveObjConfigsToIni(cbx_ListLightController.Text,new string[] { "通道亮度"});
                // iniLight.WriteValue<int[]>(ModelChangeVariables.当前型号.Text + cbx_ListLightController.Text, "通道亮度", 通道亮度);
                //读取配置
               // FindLightControllerClass(cbx_ListLightController.Text).LoadConfigs();
                MessageBox.Show("保存参数成功！", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存参数异常" + ex.Message, "错误提示");
            }
        }
        /// <summary>
        /// 关闭所有通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (FindLightControllerClass(cbx_ListLightController.Text) != null && FindLightControllerClass(cbx_ListLightController.Text).IsConnected)
                {
                    FindLightControllerClass(cbx_ListLightController.Text).CloseAllLights();
                }
            }
            catch (Exception)
            {
            }
          
        }
        /// <summary>
        /// 打开所有通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (FindLightControllerClass(cbx_ListLightController.Text) != null&& FindLightControllerClass(cbx_ListLightController.Text).IsConnected)
                {
                    FindLightControllerClass(cbx_ListLightController.Text).OpenAllLights();
                }
                UpdateChannels();
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 通道数改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Updown_channels_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (FindLightControllerClass(cbx_ListLightController.Text) != null && FindLightControllerClass(cbx_ListLightController.Text).IsConnected)
                {
                    FindLightControllerClass(cbx_ListLightController.Text).通道数 = Convert.ToInt16(Updown_channels.Value);
                    UpdateChannels();
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 通道亮度改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void brightness_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (FindLightControllerClass(cbx_ListLightController.Text) == null|| !FindLightControllerClass(cbx_ListLightController.Text).IsConnected)
                    return;
                NumericUpDown numericUpDown = sender as NumericUpDown;
                for (int i = 0; i < FindLightControllerClass(cbx_ListLightController.Text).通道数; i++)
                {
                    if (numericUpDown.Name == "brightness" + i.ToString())
                    {
                        FindLightControllerClass(cbx_ListLightController.Text).AdjustChannelSet(i, Convert.ToInt16(numericUpDown.Value));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void bt_Closed_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_minimum_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
