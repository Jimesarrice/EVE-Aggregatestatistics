using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 集结统计
{
    public partial class 数据库操作 : Form
    {
        public 数据库操作()
        {
            InitializeComponent();
        }
        SQLiteConnection DBConnection;//默认数据库读写
        int ui = 0;

        #region  SQLite访问类

        public void createNewDatabase(string filename)
        {
            SQLiteConnection.CreateFile(filename);
        }

        //创建一个连接到指定数据库
        public void connectToDatabase(string filename)
        {
            DBConnection = new SQLiteConnection("Data Source=" + filename + ";Version=3;");
            DBConnection.Open();
        }

        //在指定数据库中创建一个table
        public void createTable(string tablename, string data)
        {
            string sql = "create table " + tablename + " (" + data + ");";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            command.ExecuteNonQuery();
        }
        //删除表记录
        public void dropTable(string tablename)
        {
            string sql = "DROP " + tablename + ";";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            command.ExecuteNonQuery();
        }


        //插入一些数据
        public void fillTable(string tablename, string name, string data)
        {
            string sql = "insert into " + tablename + " (" + name + ") values (" + data + ");";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            command.ExecuteNonQuery();
        }

        //使用sql查询语句，并显示结果
        public string printHighscores(string selecting, string tablename, string name, string data)
        {
            string re = null;
            string sql = "select " + selecting + " from '" + tablename + "' where " + name + "='" + data + "';";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    re += reader[i] + "\r\n";
                }

            //re = selecting + reader[selecting];
            //Console.ReadLine();
            return re;
        }

        //执行SQL语句
        public string dosqlcom(string sql)
        {
            string re = null;
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    re += reader[i] + "\r\n";
                }
            //Console.ReadLine();
            return re;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = dosqlcom(textBox1.Text);
            }
            catch
            {
                MessageBox.Show("查询错误");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ui == 0)
            {
                this.Size = new Size(750, 250);
                button2.Text = "收起扩展功能<";
                ui = 1;
            }
            else if (ui == 1)
            {
                this.Size = new Size(445, 250);
                button2.Text = "展开扩展功能>";
                ui = 0;
            }
            else
            {
                MessageBox.Show("求大佬告诉开发者哥哥你是怎么做到的。");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = printHighscores(select.Text, from.Text, where.Text, data.Text);
            }
            catch
            {
                MessageBox.Show("查询错误");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                button5.Enabled = true;
                string FileName = "";
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.DefaultExt = "db";
                openDialog.Filter = "数据库文件|*.db";
                openDialog.ShowDialog();
                FileName = openDialog.FileName;
                DBConnection = new SQLiteConnection("Data Source=" + FileName + ";Version=3;");
                DBConnection.Open();
                richTextBox1.Text += "\r\n连接数据库成功";
                button1.Enabled = true;
                from.Items.Clear();
                using (DataTable mTables = DBConnection.GetSchema("Tables")) // "Tables"包含系统表详细信息；
                {
                    for (int i = 0; i < mTables.Rows.Count; i++)
                    {
                        from.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                        comboBox1.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                    }
                    if (from.Items.Count > 0)
                    {
                        from.SelectedIndex = 0; // 默认选中第一张表.
                        comboBox1.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
                button5.Enabled = false;
                MessageBox.Show("打开数据库错误");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "\r\nUI暂时可能未响应，请耐心等待。";
            string strarrtable = "";
            using (DataTable mTables = DBConnection.GetSchema("Tables")) // "Tables"包含系统表详细信息；
            {
                for (int i = 0; i < mTables.Rows.Count; i++)
                {
                    strarrtable += mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString() + ";";
                }
            }
            string[] sArraytable = strarrtable.Split(';');
            string tablename = DateTime.Now.ToString("yyyyMMddHHmm") + "summarizing";
            createTable("\"" + tablename + "\"", "\"id\"  varchar NOT NULL,\"userid\"  varchar NOT NULL,\"shipclass\"  varchar,PRIMARY KEY(\"id\")");
            string sql, re = "";
            for (int i = 0; i < sArraytable.Length - 1; i++)
            {
                sql = "select userid,shipclass from \'" + sArraytable[i] + "\';";
                SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0].ToString() != string.Empty && reader[1].ToString() != string.Empty)
                    {
                        re += reader[0] + "\",\"" + reader[1] + ";";
                    }
                    if (reader[0].ToString() != string.Empty && reader[1].ToString() == string.Empty)
                    {
                        re += reader[0] + "\",\"Unknown;";
                    }
                }
            }
            //richTextBox1.Text += re;
            string[] impdata = re.Split(';');
            for (int i = 0; i < impdata.Length-1; i++)
            {
                if (impdata[i] != string.Empty)
                {
                    //impdata[i] = impdata[i].Remove(impdata[i].Length, 1);
                    string sqlim = "insert into \"" + tablename + "\" values (\"" + i + "\",\"" + impdata[i] + "\");";
                    SQLiteCommand command = new SQLiteCommand(sqlim, DBConnection);
                    command.ExecuteNonQuery();
                }
            }
            richTextBox1.Text += "\r\n创建整合表成功";
            using (DataTable mTables = DBConnection.GetSchema("Tables")) // "Tables"包含系统表详细信息；
            {
                for (int i = 0; i < mTables.Rows.Count; i++)
                {
                    from.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                    comboBox1.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                }
                if (from.Items.Count > 0)
                {
                    from.SelectedIndex = 0; // 默认选中第一张表.
                    comboBox1.SelectedIndex = 0;
                }
            }
            comboBox1.Text = tablename;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            //SELECT DISTINCT 列名列表 FROM 表名
            string sql = "select distinct userid from \'" + comboBox1.Text + "\';";
            string userinf = "";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                userinf += reader[0] + ";";
            }
            //richTextBox2.Text = userinf;
            string[] userarr = userinf.Split(';');
            //string sql1 = "";
            string usersum = "";
            for (int i = 0; i < userarr.Length - 1; i++)
            {
                int j = 0;
                string sql1 = "select userid from \"" + comboBox1.Text + "\" where userid=\"" + userarr[i] + "\";";
                SQLiteCommand command1 = new SQLiteCommand(sql1, DBConnection);
                SQLiteDataReader reader1 = command1.ExecuteReader();
                while (reader1.Read())
                {
                    j++;
                }
                usersum += j.ToString() + ";";
            }
            string[] usersumarr = usersum.Split(';');
            for (int k = 0; k < userarr.Length - 1; k++)
            {
                if (userarr[k] != string.Empty)
                {
                    richTextBox2.Text += userarr[k] + "\t" + usersumarr[k] + "\r\n";
                }
            }
        }
    }
}
