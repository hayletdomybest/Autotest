using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoUpdate
{
    public partial class AcceptForm : Form
    {
        internal AcceptForm(AutoUpdateXml info)
        {
            InitializeComponent();
            string curr = string.Format(labe_title_curr.Text, ProductVersion.ToString());
            string lastest = string.Format(lab_title_lastest.Text, info.Version.ToString());
            labe_title_curr.Text = curr;
            lab_title_lastest.Text = lastest;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
