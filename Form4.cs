using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace LoginAndRegisterForms
{
    public partial class Dashboardfrm : Form
    {
        public Dashboardfrm()
        {
            InitializeComponent();
        }

        private void Dashboardfrm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            MonthlyDues MonthlyDues = new MonthlyDues();
            MonthlyDues.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            VisitorLogFrm VisitorForm = new VisitorLogFrm();
            VisitorForm.Show();
        }
    }
}
