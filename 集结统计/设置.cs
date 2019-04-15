using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 集结统计
{
    public partial class 设置 : Form
    {
        ReadIni rini = new ReadIni();

        public 设置()
        {
            InitializeComponent();
        }

        private void 设置_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\config.ini"))
            {

            }
            else
            {
                textBox1.Text = rini.IniReadValue("Seting", "DBlink", Path.GetDirectoryName(Application.ExecutablePath) + "\\config.ini");
                //textBox2.Text = rini.IniReadValue("Seting", "SetUserID", Path.GetDirectoryName(Application.ExecutablePath) + "\\config.ini");
                //textBox3.Text = rini.IniReadValue("Seting", "SetPassword", Path.GetDirectoryName(Application.ExecutablePath) + "\\config.ini");
                //textBox4.Text = rini.IniReadValue("Seting", "SetDataBase", Path.GetDirectoryName(Application.ExecutablePath) + "\\config.ini");
                //textBox5.Text = rini.IniReadValue("Seting", "SetPort", Path.GetDirectoryName(Application.ExecutablePath) + "\\config.ini");
                //textBox6.Text = rini.IniReadValue("Seting", "SetCharset", Path.GetDirectoryName(Application.ExecutablePath) + "\\config.ini");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rini.IniWrite("Seting", "DBlink", textBox1.Text, Directory.GetCurrentDirectory() + "\\config.ini");
            //rini.IniWrite("Seting", "SetUserID", textBox2.Text, Directory.GetCurrentDirectory() + "\\config.ini");
            //rini.IniWrite("Seting", "SetPassword", textBox3.Text, Directory.GetCurrentDirectory() + "\\config.ini");
            //rini.IniWrite("Seting", "SetDataBase", textBox4.Text, Directory.GetCurrentDirectory() + "\\config.ini");
            //rini.IniWrite("Seting", "SetPort", textBox5.Text, Directory.GetCurrentDirectory() + "\\config.ini");
            //rini.IniWrite("Seting", "SetCharset", textBox6.Text, Directory.GetCurrentDirectory() + "\\config.ini");


            this.Close();
        }
    }
}
