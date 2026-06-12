using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core
{
    //todo:用户登录
    public partial class FormPassword : Form
    {
        public FormPassword()
        {
            InitializeComponent();
            lblErrorReason.Text = "";
            MultiLanguageUtil.SetFormLanguage(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            lblErrorReason.Text = "";
            string strErrorReason = "";
            if (cbmUserGroup.Text=="管理员")
            {
                if (txtPassword.Text=="dbaadmin")
                {
                    PrivilegeLevel = 4;
                    this.Close();
                }
                if (txtPassword.Text == "nvt" + DateTime.Now.ToString("MMddHHmm"))
                {
                    PrivilegeLevel = 5;
                    this.Close();
                }
                else
                {
                    strErrorReason = "密码错误";
                }
            }
            if (cbmUserGroup.Text == "工程师")
            {
                if (txtPassword.Text == "admin")
                {
                    PrivilegeLevel = 3;
                    this.Close();
                }
                else
                {
                    strErrorReason = "密码错误";
                }
            }
            if (cbmUserGroup.Text == "设备技术员")
            {
                if (txtPassword.Text == "3333")
                {
                    PrivilegeLevel = 2;
                    this.Close();
                }
                else
                {
                    strErrorReason = "密码错误";
                }
            }
            if (cbmUserGroup.Text == "操作员")
            {
                if (txtPassword.Text == "123456")
                {
                    PrivilegeLevel = 1;
                    this.Close();
                }
                else
                {
                    strErrorReason = "密码错误";
                }
            }
            lblErrorReason.Text = strErrorReason;
        
        }

        public int PrivilegeLevel = 0;

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            var s = sender.ToString();
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {

        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnOK_Click(null, null);
            }
        }

    }
}
