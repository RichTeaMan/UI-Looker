using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextBoxBoxSocial
{
    public partial class Mdi : Form
    {
        public Mdi()
        {
            InitializeComponent();
        }

        private void textboxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new Form1();

            form.MdiParent = this;
            form.Show();
        }

        private void theOtherOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new Form2();

            form.MdiParent = this;
            form.Show();
        }
    }
}
