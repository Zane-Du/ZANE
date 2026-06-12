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
    public partial class Frm_KeyenceScannerSetting : Form
    {
        public Frm_KeyenceScannerSetting(List<ScannerKeyenceTcp> ListScannerKeyence)
        {
            InitializeComponent();
            ListKeyenceScanner = ListScannerKeyence;
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
        List<ScannerKeyenceTcp> ListKeyenceScanner = new List<ScannerKeyenceTcp>();
        private void KeyenceScannerSetting_Load(object sender, EventArgs e)
        {
            if (ListKeyenceScanner != null && ListKeyenceScanner.Count > 0)
            {
                for (int i = 0; i < ListKeyenceScanner.Count; i++)
                {
                    cbx_ListScanner.Items.Add(ListKeyenceScanner[i].DeviceName);
                }
                cbx_ListScanner.SelectedIndex = 0;
            }
        }

        private void cbx_ListScanner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FindKeyenceScannerClass(cbx_ListScanner.Text) == null)
            {
                MessageBox.Show("无对应的扫码枪，请确认！", "错误提示！");
                return;
            }
            if (FindKeyenceScannerClass(cbx_ListScanner.Text).isEnableOrShieldScanning)
            {
                sw_isScan.Checked = true;
            }
            else
            {
                sw_isScan.Checked = false;
            }
            txt_Ip.Text = FindKeyenceScannerClass(cbx_ListScanner.Text).IP地址;
            txt_Port.Text = FindKeyenceScannerClass(cbx_ListScanner.Text).端口号.ToString();
            txb_MaxLength.Text = FindKeyenceScannerClass(cbx_ListScanner.Text).读取条码长度.ToString();
            txb_TimeOut.Text = FindKeyenceScannerClass(cbx_ListScanner.Text).读取超时.ToString();
            UpdateStatus();
        }

        public void UpdateStatus()
        {

            if (FindKeyenceScannerClass(cbx_ListScanner.Text).IsConnected)
            {
                lb_connectStus.Text = "已连接";
                lb_connectStus.BackColor = Color.Green;
                bt_OpenScanner.Text = "断开连接";                
                txt_Ip.Enabled = false;
                txt_Port.Enabled = false;
                txb_MaxLength.Enabled = false;
                txb_TimeOut.Enabled = false;
            }
            else
            {
                lb_connectStus.Text = "未连接";
                lb_connectStus.BackColor = Color.Red;
                bt_OpenScanner.Text = "开启连接";                
                txt_Ip.Enabled = true;
                txt_Port.Enabled = true;
                txb_MaxLength.Enabled = true;
                txb_TimeOut.Enabled = true;
            }

        }
        /// <summary>
        /// 查扫扫码枪，返回对应的实例化类
        /// </summary>
        /// <returns></returns>
        public ScannerKeyenceTcp FindKeyenceScannerClass(string deviceName)
        {
            for (int i = 0; i < ListKeyenceScanner.Count; i++)
            {
                if (ListKeyenceScanner[i].DeviceName == deviceName)
                {
                    return ListKeyenceScanner[i];
                }
            }
            return null;
        }

        private void bt_SaveParam_Click(object sender, EventArgs e)
        {
            if (FindKeyenceScannerClass(cbx_ListScanner.Text) != null)
            {
                FindKeyenceScannerClass(cbx_ListScanner.Text).isEnableOrShieldScanning = sw_isScan.Checked;
                FindKeyenceScannerClass(cbx_ListScanner.Text).IP地址 = txt_Ip.Text.Trim();
                FindKeyenceScannerClass(cbx_ListScanner.Text).端口号 = int.Parse(txt_Port.Text.Trim());
                FindKeyenceScannerClass(cbx_ListScanner.Text).读取条码长度 = int.Parse(txb_MaxLength.Text.Trim());
                FindKeyenceScannerClass(cbx_ListScanner.Text).读取超时 = int.Parse(txb_TimeOut.Text.Trim());
                FindKeyenceScannerClass(cbx_ListScanner.Text).SaveConfigs();
            }
            MessageBox.Show("保存参数成功！","提示");
        }

        private void bt_OpenScanner_Click(object sender, EventArgs e)
        {
            try
            {
                if (FindKeyenceScannerClass(cbx_ListScanner.Text) == null)
                {
                    return;
                }
                if (lb_connectStus.Text == "未连接")
                {
                    FindKeyenceScannerClass(cbx_ListScanner.Text).IP地址 = txt_Ip.Text.Trim();
                    FindKeyenceScannerClass(cbx_ListScanner.Text).端口号 = int.Parse(txt_Port.Text.Trim());
                    FindKeyenceScannerClass(cbx_ListScanner.Text).读取条码长度 = int.Parse(txb_MaxLength.Text.Trim());
                    FindKeyenceScannerClass(cbx_ListScanner.Text).读取超时 = int.Parse(txb_TimeOut.Text.Trim());
                    FindKeyenceScannerClass(cbx_ListScanner.Text).Open();
                }
                else
                {
                    FindKeyenceScannerClass(cbx_ListScanner.Text).Close();
                }

                UpdateStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作异常"+ex.Message,"错误提示");
            }
         
        }

        private void sw_isScan_CheckedChanged(object sender, EventArgs e)
        {
            if (FindKeyenceScannerClass(cbx_ListScanner.Text) == null)
            {
                return;
            }
            FindKeyenceScannerClass(cbx_ListScanner.Text).isEnableOrShieldScanning = sw_isScan.Checked;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (FindKeyenceScannerClass(cbx_ListScanner.Text) == null)
            {
                return;
            }
            textBox1.Text = FindKeyenceScannerClass(cbx_ListScanner.Text).ScanOnce();
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
