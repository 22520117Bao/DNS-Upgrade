using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TryForBetter
{
    public partial class ThongKeForm : Form
    {
        public ThongKeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerLogin form1 = new ServerLogin();
            form1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SignUpForm form2 = new SignUpForm();  
            form2.Show();
        }

       
    }
}
