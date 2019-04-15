using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 集结统计
{
    public partial class 指定日期保存 : Form
    {
        public 指定日期保存()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = dateTimePicker1.Value.ToString("yyyyMMddHHmm");
            主程序.idate = dateTimePicker1.Value.ToString("yyyyMMddHHmm");
            Close();
        }
    }
}
