using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeviceCountMgr
{
    public partial class Form1 : Form
    {

        private DbUtils dbUitls = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dbUitls = new DbUtils(System.Configuration.ConfigurationSettings.AppSettings["ConnStr"].ToString());



            String sqlStr = "SELECT TOP 1 * FROM PARAMCONFIG";



            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {



                    tbAuditMax.Text = EncryptUtils.DesDecrypt(dt.Rows[0]["MaxClientsCountForVideo"].ToString());

                    tbVideoMax.Text = EncryptUtils.DesDecrypt(dt.Rows[0]["MaxClientsCountForAudio"].ToString());

                    tbRemoteMax.Text = EncryptUtils.DesDecrypt(dt.Rows[0]["MaxClientsCountForRemoteControl"].ToString());
                }


            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
         

            int a=0;

            if (!int.TryParse(tbAuditMax.Text,out a))
            {
                MessageBox.Show("请输入正确数字");
                return;
            }

            if (!int.TryParse(tbVideoMax.Text, out a))
            {
                MessageBox.Show("请输入正确数字");
                return;
            }

            if (!int.TryParse(this.tbRemoteMax.Text, out a))
            {
                MessageBox.Show("请输入正确数字");
                return;
            }

            String sqlStr = "update PARAMCONFIG set MaxClientsCountForVideo='{0}',MaxClientsCountForAudio='{1}',MaxClientsCountForRemoteControl='{2}'";

            dbUitls.ExecuteNonQuery(String.Format(sqlStr, EncryptUtils.DesEncrypt(tbAuditMax.Text), EncryptUtils.DesEncrypt(tbVideoMax.Text), EncryptUtils.DesEncrypt(tbRemoteMax.Text)));
            MessageBox.Show("保存成功");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
