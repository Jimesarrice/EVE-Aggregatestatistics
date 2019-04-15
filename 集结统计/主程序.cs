using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 集结统计
{
    public partial class 主程序 : Form
    {
        public class userinf
        {
            public string name { get; set; }
        }
        //private string[] comps;
        public 主程序()
        {
            InitializeComponent();
            this.Resize += Form1_Resize;
        }
        SQLiteConnection mConn;//第三方数据库加载
        SQLiteConnection DBConnection;//默认数据库读写
        SQLiteDataAdapter mAdapter;
        DataTable mTable;
        DataTable dt= null;

        public static string idate = "";

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
            string sql = "DROP TABLE " + tablename + ";";
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
            string sql = "select " + selecting + " from " + tablename + " where " + name + "=\"" + data + "\";";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                re = selecting + reader[selecting];
            Console.ReadLine();
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
                    re += reader[i] + ";";
                }
            Console.ReadLine();
            return re;
        }
        #endregion


        #region 自调节窗口大小
        private float X;//当前窗体的宽度
        private float Y;//当前窗体的高度
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            float newx = (this.Width) / X; //窗体宽度缩放比例
            float newy = (this.Height) / Y;//窗体高度缩放比例
            setControls(newx, newy, this);//随窗体改变控件大小

        }
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            //遍历窗体中的控件，重新设置控件的值
            foreach (Control con in cons.Controls)
            {

                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//获取控件的Tag属性值，并分割后存储字符串数组
                float a = System.Convert.ToSingle(mytag[0]) * newx;//根据窗体缩放比例确定控件的值，宽度
                con.Width = (int)a;//宽度
                a = System.Convert.ToSingle(mytag[1]) * newy;//高度
                con.Height = (int)(a);
                a = System.Convert.ToSingle(mytag[2]) * newx;//左边距离
                con.Left = (int)(a);
                a = System.Convert.ToSingle(mytag[3]) * newy;//上边缘距离
                con.Top = (int)(a);
                Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字体大小
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }
        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            X = this.Width;//获取窗体的宽度
            Y = this.Height;//获取窗体的高度
            setTag(this);//调用方法
            /*
            dataGridView1.Columns.Add("userid", "游戏ID");
            dataGridView1.Columns.Add("userway", "所处位置");
            dataGridView1.Columns.Add("shipid", "舰船名称");
            dataGridView1.Columns.Add("shipclass", "舰船类型");
            dataGridView1.Columns.Add("groupid", "舰队身份");
            dataGridView1.Columns.Add("skilllevel", "技能等级");
            dataGridView1.Columns.Add("groupclass", "编制位置");*/
            //dt.Rows.Clear();   

            connectToDatabase(Path.GetDirectoryName(Application.ExecutablePath) + "\\Database.db");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = dt;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
            }
            catch { }
            dataGridView1.Columns.Add("userid", "游戏ID");
            dataGridView1.Columns.Add("userway", "所处位置");
            dataGridView1.Columns.Add("shipid", "舰船名称");
            dataGridView1.Columns.Add("shipclass", "舰船类型");
            dataGridView1.Columns.Add("groupid", "舰队身份");
            dataGridView1.Columns.Add("skilllevel", "技能等级");
            dataGridView1.Columns.Add("groupclass", "编制位置");
            //dataGridView1.Columns.Add("ChereComp", "所属军团");
            string clipText = "";
            try
            {
                if (Clipboard.ContainsText())
                {
                    clipText = Clipboard.GetText();
                    string[] strArray = clipText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    strArray = strArray.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string[] strArray1 = strArray[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                        dataGridView1.Rows.Add(strArray1);
                    }
                }
                else
                {
                    MessageBox.Show("剪贴板为空！");
                }
            }
            catch
            {
                MessageBox.Show("读取剪贴板失败！");
            }
            Clipboard.SetDataObject(string.Empty, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = dt;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
            }
            catch { }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ExportExcels(dataGridView1);
        }

        /// <summary>
        /// 保存DataGridView到EXCEL
        /// </summary>
        /// <param name="myDGV">控件DataGridView</param>
        private void ExportExcels(DataGridView myDGV)
        {
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xlsx";
            saveDialog.Filter = "2007Excel文件|*.xlsx";
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return; //被点了取消
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("创建Excel对象出错，请检查程序完整性或者安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1
                                                                                                                                  //写入标题
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = myDGV.Columns[i].HeaderText;
            }
            //写入数值
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = myDGV.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出文件时出错,文件可能正被使用！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            GC.Collect();//强行销毁
            MessageBox.Show("文件： " + saveFileName + " 保存成功", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.Clear();
                try
                {
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns.Clear();
                    dataGridView1.Rows.Clear();
                }
                catch { }
                string FileName = "";
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.DefaultExt = "db";
                openDialog.Filter = "数据库文件|*.db";
                openDialog.ShowDialog();
                FileName = openDialog.FileName;
                mConn = new SQLiteConnection("Data Source=" + FileName + ";Version=3;");
                mConn.Open();
                //connectToDatabase(FileName);
                using (DataTable mTables = mConn.GetSchema("Tables")) // "Tables"包含系统表详细信息；
                {
                    for (int i = 0; i < mTables.Rows.Count; i++)
                    {
                        comboBox1.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                    }
                    if (comboBox1.Items.Count > 0)
                    {
                        comboBox1.SelectedIndex = 0; // 默认选中第一张表.
                    }
                }
                button5.Enabled = true;
                button12.Enabled = true;
            }
            catch
            {
                button5.Enabled = false;
                MessageBox.Show("打开数据库错误");
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = dt;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
            }
            catch { }
            try
            {
                if (comboBox1.Text != "")
                {
                    mAdapter = new SQLiteDataAdapter("SELECT * FROM [" + comboBox1.Text + "]", mConn);
                    mTable = new DataTable(); // Don't forget initialize!
                    mAdapter.Fill(mTable);

                    // 绑定数据到DataGridView
                    dataGridView1.DataSource = mTable;
                }
                else
                {
                    MessageBox.Show("然而并没有什么可以读取的东西");
                }
            }
            catch
            {
                MessageBox.Show("读取数据表错误");
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            /*
            string chid, comp, temp,temp1,temp2,json;
            string data = string.Empty;
            try
            {
                //richTextBox1.Text += dataGridView1.RowCount+"\r\n";
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    data = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    temp = HttpGet("https://api.eve-online.com.cn/eve/CharacterID.xml.aspx?names=" + data);
                    string[] sArray = temp.Split(new string[] { "ID=\"", "\" />" }, StringSplitOptions.RemoveEmptyEntries);
                    chid = sArray[1];
                    temp1 = HttpGet("https://api-serenity.eve-online.com.cn/characters/" + chid + "/");
                    string[] ssArray = temp1.Split(new string[] { "corporation", "isNPC" }, StringSplitOptions.RemoveEmptyEntries);
                    temp2 = ssArray[1];
                    string[] sssArray = temp2.Split('"');
                    //textBox1.Text = sssArray[4];
                    //comp = UnicodeToString(sssArray[4]);
                    json = "{\"name\": \"" + sssArray[4] + "\"}";
                    //comp = sssArray[4];
                    //json = "{\"name\": \"" + "\u9ec4\u57d4\u519b\u4e8b\u5b66\u9662" + "\"}";
                    StringReader sr = new StringReader(json);
                    JsonTextReader jsonReader = new JsonTextReader(sr);
                    JsonSerializer serializer = new JsonSerializer();
                    var r = serializer.Deserialize<userinf>(jsonReader);
                    comp = r.name.ToString();
                    dataGridView1.Rows[i].Cells[7].Value = comp;
                    //richTextBox1.Text += sssArray[1] + "\r\n\r\n" + sssArray[2] + "\r\n\r\n\r\n" + sssArray[3] + "\r\n\r\n\r\n";
                }
            }
            catch
            {
                MessageBox.Show("未成功连接服务器或输入数据错误");
            }*/
            //comps = null;
            MessageBox.Show("目前UI可能会不响应数十秒，请耐心等待，真的不好解决开发者要疯了");
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                Thread thread1 = new Thread(new ParameterizedThreadStart(Apiget));
                thread1.Start(i);
            }
        }

        private void Apiget(object ii)
        {
            int i = (int)ii;
            string chid, comp, temp, temp1, temp2, json;
            string data = string.Empty;
            try
            {
                //richTextBox1.Text += dataGridView1.RowCount+"\r\n";
                data = dataGridView1.Rows[i].Cells[0].Value.ToString();
                temp = HttpGet("https://api.eve-online.com.cn/eve/CharacterID.xml.aspx?names=" + data);
                string[] sArray = temp.Split(new string[] { "ID=\"", "\" />" }, StringSplitOptions.RemoveEmptyEntries);
                chid = sArray[1];
                temp1 = HttpGet("https://api-serenity.eve-online.com.cn/characters/" + chid + "/");
                string[] ssArray = temp1.Split(new string[] { "corporation", "isNPC" }, StringSplitOptions.RemoveEmptyEntries);
                temp2 = ssArray[1];
                string[] sssArray = temp2.Split('"');
                //textBox1.Text = sssArray[4];
                //comp = UnicodeToString(sssArray[4]);
                json = "{\"name\": \"" + sssArray[4] + "\"}";
                //comp = sssArray[4];
                //json = "{\"name\": \"" + "\u9ec4\u57d4\u519b\u4e8b\u5b66\u9662" + "\"}";
                StringReader sr = new StringReader(json);
                JsonTextReader jsonReader = new JsonTextReader(sr);
                JsonSerializer serializer = new JsonSerializer();
                var r = serializer.Deserialize<userinf>(jsonReader);
                comp = r.name.ToString();
                //dataGridView1.Rows[i].Cells[7].Value = comp;
                Writedata(comp, i);
                //comps[i] = comp;
                //richTextBox1.Text += sssArray[1] + "\r\n\r\n" + sssArray[2] + "\r\n\r\n\r\n" + sssArray[3] + "\r\n\r\n\r\n";
            }
            catch
            {
                MessageBox.Show("未成功连接服务器或输入数据错误请重试或检查服务器是否在线");
            }

        }

        private void Writedata(string datas,int i)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new Action<object>(Apiget),i);
                return;
            }
            dataGridView1.Rows[i].Cells[7].Value = datas;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// 获取HTTP数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            设置 st = new 设置();
            st.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                string data = string.Empty;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                    {
                        data += "\"" + dataGridView1.Rows[i].Cells[j].Value + "\",";
                        richTextBox1.Text += "\"" + dataGridView1.Rows[i].Cells[j].Value + "\",";
                    }
                    data += ":";
                    richTextBox1.Text += ":";
                }
                string tablename = DateTime.Now.ToString("yyyyMMddHHmm");
                createTable("\"" + tablename + "\"", "\"userid\"  varchar NOT NULL,\"userway\"  varchar,\"shipid\"  varchar,\"shipclass\"  varchar,\"groupid\"  varchar,\"skilllevel\"  varchar,\"groupclass\"  varchar,PRIMARY KEY(\"userid\")");
                string[] impdata = data.Split(':');
                for (int i = 0; i < impdata.Length; i++)
                {
                    if (impdata[i] != string.Empty)
                    {
                        impdata[i] = impdata[i].Remove(impdata[i].Length - 1, 1);
                        string sql = "insert into \"" + tablename + "\" values (" + impdata[i] + ");";
                        SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception er)
            {
                MessageBox.Show("抛出异常" + er.ToString());
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = "";
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "db";
                saveDialog.Filter = "数据库文件|*.db";
                saveDialog.ShowDialog();
                FileName = saveDialog.FileName;
                createNewDatabase(FileName);
            }
            catch
            {
                MessageBox.Show("文件名错误，请输入正确文件名");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            数据库操作 dbc = new 数据库操作();
            dbc.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "DROP TABLE \'" + comboBox1.Text + "\';";
                SQLiteCommand mConnn = new SQLiteCommand(sql, mConn);
                mConnn.ExecuteNonQuery();
                comboBox1.Text = "";
                comboBox1.Items.Clear();
                using (DataTable mTables = mConn.GetSchema("Tables")) // "Tables"包含系统表详细信息；
                {
                    for (int i = 0; i < mTables.Rows.Count; i++)
                    {
                        comboBox1.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                    }
                    if (comboBox1.Items.Count > 0)
                    {
                        comboBox1.SelectedIndex = 0; // 默认选中第一张表.
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show("抛出异常" + er.ToString());
            }
            MessageBox.Show("操作完成。");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void 从剪切板读取数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void 保存当日信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button9_Click(sender, e);
        }

        private void 导出到excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }

        private void 关掉我ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 保存指定日期信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            指定日期保存 zd = new 指定日期保存();
            zd.ShowDialog();
            string tablename = "";
            try
            {
                string data = string.Empty;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                    {
                        data += "\"" + dataGridView1.Rows[i].Cells[j].Value + "\",";
                        richTextBox1.Text += "\"" + dataGridView1.Rows[i].Cells[j].Value + "\",";
                    }
                    data += ":";
                    richTextBox1.Text += ":";
                }
                if (idate != "")
                {
                    tablename = idate;
                    createTable("\"" + tablename + "\"", "\"userid\"  varchar NOT NULL,\"userway\"  varchar,\"shipid\"  varchar,\"shipclass\"  varchar,\"groupid\"  varchar,\"skilllevel\"  varchar,\"groupclass\"  varchar,PRIMARY KEY(\"userid\")");
                    string[] impdata = data.Split(':');
                    for (int i = 0; i < impdata.Length; i++)
                    {
                        if (impdata[i] != string.Empty)
                        {
                            impdata[i] = impdata[i].Remove(impdata[i].Length - 1, 1);
                            string sql = "insert into \"" + tablename + "\" values (" + impdata[i] + ");";
                            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("日期设置出错");
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception er)
            {
                MessageBox.Show("抛出异常" + er.ToString());
            }
        }

        private void 数据库操作窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            数据库操作 dbc = new 数据库操作();
            dbc.Show();
        }

        private void 清空数据面板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void 新建数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button10_Click(sender, e);
        }

        private void 我也不知道啥时候的ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("完了，你把开发者气坏了，要小姐姐亲亲抱抱才能起来");
        }

        private void 骚操作ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("完了，你把开发者气坏了，要小姐姐亲亲抱抱才能起来");
        }

        private void 统计集结率ToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                SQLiteCommand command1 = new SQLiteCommand(sql, DBConnection);
                SQLiteDataReader reader1 = command1.ExecuteReader();
                while (reader1.Read())
                {
                    if (reader1[0].ToString() != string.Empty && reader1[1].ToString() != string.Empty)
                    {
                        re += reader1[0] + "\",\"" + reader1[1] + ";";
                    }
                    if (reader1[0].ToString() != string.Empty && reader1[1].ToString() == string.Empty)
                    {
                        re += reader1[0] + "\",\"Unknown;";
                    }
                }
            }
            //richTextBox1.Text += re;
            string[] impdata = re.Split(';');
            for (int i = 0; i < impdata.Length - 1; i++)
            {
                if (impdata[i] != string.Empty)
                {
                    //impdata[i] = impdata[i].Remove(impdata[i].Length, 1);
                    string sqlim = "insert into \"" + tablename + "\" values (\"" + i + "\",\"" + impdata[i] + "\");";
                    SQLiteCommand command1 = new SQLiteCommand(sqlim, DBConnection);
                    command1.ExecuteNonQuery();
                }
            }
            comboBox1.Text = "";
            comboBox1.Items.Clear();
            using (DataTable mTables = DBConnection.GetSchema("Tables")) // "Tables"包含系统表详细信息；
            {
                for (int i = 0; i < mTables.Rows.Count; i++)
                {
                    comboBox1.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                }
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0; // 默认选中第一张表.
                }
            }



            //richTextBox2.Text = "";
            //SELECT DISTINCT 列名列表 FROM 表名
            string sql0 = "select distinct userid from \'" + tablename + "\';";
            string userinf = "";
            SQLiteCommand command = new SQLiteCommand(sql0, DBConnection);
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
                string sql1 = "select userid from \"" + tablename + "\" where userid=\"" + userarr[i] + "\";";
                SQLiteCommand command1 = new SQLiteCommand(sql1, DBConnection);
                SQLiteDataReader reader1 = command1.ExecuteReader();
                while (reader1.Read())
                {
                    j++;
                }
                usersum += j.ToString() + ";";
            }
            string[] usersumarr = usersum.Split(';');

            try
            {
                dataGridView1.DataSource = dt;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
            }
            catch { }
            dataGridView1.Columns.Add("userid", "游戏ID");
            dataGridView1.Columns.Add("userway", "出勤次数");

            for (int k = 0; k < userarr.Length - 1; k++)
            {
                if (userarr[k] != string.Empty)
                {
                    string[] starr = { userarr[k] , usersumarr[k] };
                    dataGridView1.Rows.Add(starr);
                    //richTextBox2.Text += userarr[k] + "\t" + usersumarr[k] + "\r\n";
                }
            }


        }

        private void 程序设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            设置 st = new 设置();
            st.ShowDialog();
        }

        private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            使用说明 ss = new 使用说明();
            ss.ShowDialog();
        }

        private void 作者信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("没有发现可用更新呢");
        }

        private void 关于软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            关于 g = new 关于();
            g.ShowDialog();
        }
    }
}

