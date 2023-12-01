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
    public partial class Form1 : Form
    {
        public void get_book_info()
        {
            if (textBox1.Text == "001")
            {
                books_name.Text = "測試書本一";
                books_price.Text = "200";
            }
            else if (textBox1.Text == "002")
            {
                books_name.Text = "測試書本二";
                books_price.Text = "400";
            }
            else
            {
                books_name.Text = "unknow";
                books_price.Text = "0";
            }
        }

        public void sales_subtotal() {
            double num = 0;
            if (sale_num.Text != "")
            {
                num = Convert.ToDouble(sale_num.Text);
            }
            double price = Convert.ToDouble(books_price.Text);
            books_sum.Text = (price * num).ToString();
        }

        public Form1()
        {
            InitializeComponent();
            get_book_info();
            sales_subtotal();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            get_book_info();
            sales_subtotal();
        }
        private void sale_num_TextChanged(object sender, EventArgs e)
        {
            get_book_info();
            sales_subtotal();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double price = Convert.ToDouble((string)books_price.Text);
            double num = Convert.ToDouble((string)sale_num.Text);
            double subtotal = Convert.ToDouble((string)books_sum.Text);

            DataGridViewRowCollection rows = dataGridView1.Rows;
            rows.Add("", books_name.Text, price, num, subtotal);
        }

        private void contactUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Student's ID is 092214133.");
        }
    }
}
