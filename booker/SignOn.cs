using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace booker
{
    public partial class SignOn : Form
    {
        public SignOn()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string password = psw_input.Text;
            if (password=="123")
            {
                Form1 form1;
                form1 = new Form1();
                // 打開 form1

                form1.ShowDialog();
                psw_input.Text = "";
            }
            else
            {
                MessageBox.Show("密碼錯誤");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 關閉應用
            Application.Exit();
        }
    }
}
