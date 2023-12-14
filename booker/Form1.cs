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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace booker
{
    public partial class Form1 : Form
    {
        int index = 1;

        public class DBConfig
        {
            //DB位置設定
            //log.db要放在【bin\Debug底下】
            //由於無法正常載入因此位置改為booker底下
            public static string dbFile = Application.StartupPath + @"\..\..\booker.db";

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
            string member = input_member.Text;
            string date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string sql_sale_insert = $"INSERT INTO  sale  (total,member_id,employ_id,date) VALUES (0, '{member}',  '123', '{date}' ); SELECT last_insert_rowid();";//改
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
                    string book_name = row.Cells["Id"].Value.ToString();
                    int num = Convert.ToInt32(row.Cells["num"].Value);
                    int sub_total = Convert.ToInt32(row.Cells["sum"].Value);
                    total += sub_total;

                    string sql_salebook_insert = $"INSERT INTO salebook (books_id,sales_id,num) VALUES ('{book_name}','{sales_id}',{num});";//改
                    //建立sqlite指令
                    DBConfig.sqlite_cmd = new SQLiteCommand(sql_salebook_insert, DBConfig.sqlite_connect);
                    //執行sql
                    DBConfig.sqlite_cmd.ExecuteNonQuery();
                }
            }

            //======更新銷售紀錄======
            //撰寫要執行的sql指令；先設定total=0
            string sql_sale_update = $"UPDATE sale SET total = {total} WHERE id = {sales_id} ; ";
            //建立sqlite指令
            DBConfig.sqlite_cmd = new SQLiteCommand(sql_sale_update, DBConfig.sqlite_connect);
            //執行sql
            DBConfig.sqlite_cmd.ExecuteNonQuery();

            //刪除購物車
            this.dataGridView1.Rows.Clear();
            input_type.Text = "";
            sale_num.Text = "0";
        }
        public void get_book_info()
        {
            int book_id = 0;
            if (input_bookId.Text!="")
            {
                book_id = Convert.ToInt32(input_bookId.Text);
            }
            //撰寫要執行的sql指令
            string sql = $"SELECT * FROM book WHERE id = {book_id};";
            //建立sqlite指令
            DBConfig.sqlite_cmd = new SQLiteCommand(sql, DBConfig.sqlite_connect);
            //執行sql
            DBConfig.sqlite_datareader = DBConfig.sqlite_cmd.ExecuteReader();

            if (DBConfig.sqlite_datareader.HasRows)
            {
                while (DBConfig.sqlite_datareader.Read()) //read every data
                {
                    books_name.Text = Convert.ToString(DBConfig.sqlite_datareader["name"]);
                    books_price.Text = Convert.ToString(DBConfig.sqlite_datareader["price"]);

                    break;
                }
                DBConfig.sqlite_datareader.Close();
            }
            else
            {
                books_name.Text = "unknow";
                books_price.Text = "1";
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
            //get_book_info();
            sales_subtotal();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double price = Convert.ToDouble((string)books_price.Text);
            double num = Convert.ToDouble((string)sale_num.Text);
            double subtotal = Convert.ToDouble((string)books_sum.Text);

            DataGridViewRowCollection rows = dataGridView1.Rows;
            rows.Add(input_bookId.Text, books_name.Text, price, num, subtotal);

            //恢復預設輸入狀態
            input_type.Text = "";
            sale_num.Text = "1";
        }

        private void contactUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Student's ID is 092214133.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Insert_DB_Sales();
        }

        private void input_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type = input_type.Text;

            //撰寫要執行的sql指令
            string sql = $"SELECT * FROM book WHERE type = '{type}';";
            //建立sqlite指令
            DBConfig.sqlite_cmd = new SQLiteCommand(sql, DBConfig.sqlite_connect);
            //執行sql
            DBConfig.sqlite_datareader = DBConfig.sqlite_cmd.ExecuteReader();

            input_bookId.Items.Clear(); // 清空原有選項

            if (DBConfig.sqlite_datareader.HasRows)
            {
                while (DBConfig.sqlite_datareader.Read()) //read every data
                {
                    input_bookId.Items.Add(Convert.ToString(DBConfig.sqlite_datareader["id"]));
                }
                DBConfig.sqlite_datareader.Close();
            }
            else
            {
                input_bookId.Text = "";
                sale_num.Text = "1";
            }
        }

        //==================== SEARCH ====================
        private void button3_Click(object sender, EventArgs e)
        {
            // 賦值
            string book_id = search_id.Text;
            string book_name = search_name.Text;
            string book_type = search_type.Text;
            int book_price_min = -1;
            if (search_min.Text != "") {
                book_price_min = Convert.ToInt32(search_min.Text);
            }
            int book_price_max = -1;
            if (search_max.Text != "")
            {
                book_price_max = Convert.ToInt32(search_max.Text);
            }
            
            //撰寫sql
            string sql = @"SELECT book.* , COALESCE(SUM(salebook.num),0) AS salebook_num, COALESCE(SUM(restockbook.num),0) AS restockbook_num
    FROM book 
    LEFT JOIN salebook ON book.id = salebook.books_id
    LEFT JOIN restockbook ON book.id = restockbook.books_id
    WHERE book.id NOT NULL ";
            if (book_id!="")
            {
                sql += $"AND id LIKE '%{book_id}%' ";
            }
            if (book_name!="")
            {
                sql += $"AND name LIKE '%{book_name}%' ";
            }
            if (book_type != "")
            {
                sql += $"AND type = '{book_type}' ";
            }
            if (book_price_min > 0)
            {
                sql += $"AND price > '{book_price_min}' ";
            }
            if (book_price_max > 0)
            {
                sql += $"AND price < '{book_price_max}' ";
            }
            sql += @"GROUP BY book.id, book.name, book.price, book.type;";

            //建立sqlite指令
            DBConfig.sqlite_cmd = new SQLiteCommand(sql, DBConfig.sqlite_connect);
            //執行sql
            DBConfig.sqlite_datareader = DBConfig.sqlite_cmd.ExecuteReader();

            //顯示
            this.search_dataGridView.Rows.Clear();//清框顯示器
            DataGridViewRowCollection rows = search_dataGridView.Rows;
            if (DBConfig.sqlite_datareader.HasRows)
            {
                while (DBConfig.sqlite_datareader.Read()) //read every data
                {
                    string ele_id = Convert.ToString(DBConfig.sqlite_datareader["id"]);
                    string ele_name = Convert.ToString(DBConfig.sqlite_datareader["name"]);
                    string ele_price = Convert.ToString(DBConfig.sqlite_datareader["price"]);
                    int ele_sale_num = Convert.ToInt32(DBConfig.sqlite_datareader["salebook_num"]);
                    int ele_restock_num = Convert.ToInt32(DBConfig.sqlite_datareader["restockbook_num"]);
                    string ele_num = Convert.ToString(ele_restock_num- ele_sale_num);


                    rows.Add(ele_id, ele_name, ele_price, ele_num); 
                }
                DBConfig.sqlite_datareader.Close();
            }
        }
    }
}
