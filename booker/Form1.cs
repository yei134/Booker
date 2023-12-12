using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace booker
{
    public partial class Form1 : Form
    {
        int index = 1;

        public class DBConfig
        {
            //DB位置設定
            //log.db要放在【bin\Debug底下】      
            public static string dbFile = Application.StartupPath + @"\booker.db";

            public static string dbPath = "Data source=" + dbFile;

            public static SQLiteConnection sqlite_connect;
            public static SQLiteCommand sqlite_cmd;
            public static SQLiteDataReader sqlite_datareader;
        }

        private void Load_DB()
        {
            //連線載入DB
            DBConfig.sqlite_connect = new SQLiteConnection(DBConfig.dbPath);
            DBConfig.sqlite_connect.Open();// Open

        }

        private void Show_DB()
        {
            this.dataGridView1.Rows.Clear();

            //撰寫要執行的sql指令
            string sql = @"SELECT * from record;";
            //建立sqlite指令
            DBConfig.sqlite_cmd = new SQLiteCommand(sql, DBConfig.sqlite_connect);
            //執行sql
            DBConfig.sqlite_datareader = DBConfig.sqlite_cmd.ExecuteReader();

            if (DBConfig.sqlite_datareader.HasRows)
            {
                while (DBConfig.sqlite_datareader.Read()) //read every data
                {
                    int _serial = Convert.ToInt32(DBConfig.sqlite_datareader["serial"]);
                    int _date = Convert.ToInt32(DBConfig.sqlite_datareader["date"]);
                    int _type = Convert.ToInt32(DBConfig.sqlite_datareader["type"]);
                    string _name = Convert.ToString(DBConfig.sqlite_datareader["name"]);
                    double _price = Convert.ToDouble(DBConfig.sqlite_datareader["price"]);
                    double _number = Convert.ToDouble(DBConfig.sqlite_datareader["number"]);
                    double _total = _price * _number;

                    string _date_str = DateTimeOffset.FromUnixTimeSeconds(_date).ToString("yy-MM-dd hh:mm:ss");

                    string _type_str = "";
                    if (_type == 0)
                    { _type_str = "進貨"; }
                    else { _type_str = "出貨"; }

                    index = _serial;
                    DataGridViewRowCollection rows = dataGridView1.Rows;
                    rows.Add(new Object[] { index, _date_str, _type_str, _name, _price, _number
                                               , _total });
                }
                DBConfig.sqlite_datareader.Close();
            }
        }
        private void Insert_DB_Sales()
        {
            //======建立銷售紀錄======
            //撰寫要執行的sql指令；先設定total=0
            string sql_sale_insert = @"INSERT INTO 
   sale  (total,members_id,employs_id,date)
   VALUES (500, 2,  'California', 32, 20000.00 );
SELECT last_insert_rowid();";//改
            //建立sqlite指令
            DBConfig.sqlite_cmd = new SQLiteCommand(sql_sale_insert, DBConfig.sqlite_connect);
            //執行sql
            //DBConfig.sqlite_cmd.ExecuteNonQuery();
            string sales_id = DBConfig.sqlite_cmd.ExecuteScalar().ToString();//改

            //======建立銷售明細======
            int total = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow) // 排除新行（通常是用于添加新数据的行）
                {
                    //取出資料表所需欄位
                    string bookName = row.Cells["ColumnName1"].Value.ToString(); // 替换 "ColumnName1" 为你的列名
                    int price = Convert.ToInt32(row.Cells["ColumnName2"].Value); // 替换 "ColumnName2" 为你的列名
                    string category = row.Cells["ColumnName3"].Value.ToString(); // 替换 "ColumnName3" 为你的列名
                    total += price;

                    string sql_salebook_insert = $"INSERT INTO salebook (sales_id,[欄位]) VALUES ({sales_id}[值]);";//改
                    //建立sqlite指令
                    DBConfig.sqlite_cmd = new SQLiteCommand(sql_salebook_insert, DBConfig.sqlite_connect);
                    //執行sql
                    DBConfig.sqlite_cmd.ExecuteNonQuery();
                }
            }

            //======更新銷售紀錄======
            //撰寫要執行的sql指令；先設定total=0
            string sql_sale_update = $"UPDATE sale SET total = {total} WHERE id = {sales_id} ; ";//改
            //建立sqlite指令
            DBConfig.sqlite_cmd = new SQLiteCommand(sql_sale_insert, DBConfig.sqlite_connect);
            //執行sql
            DBConfig.sqlite_cmd.ExecuteNonQuery();

            //刪除購物車
            this.dataGridView1.Rows.Clear();
        }
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
            Load_DB();
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

        private void button2_Click(object sender, EventArgs e)
        {
            Insert_DB_Sales();
        }
    }
}
