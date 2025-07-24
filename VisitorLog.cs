using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCrypt.Net;


namespace LoginAndRegisterForms
{
    public partial class VisitorLogFrm : Form
    {
        private const string ConnectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";

        public VisitorLogFrm()
        {
            InitializeComponent();
        }
        private void VisitorLog() {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                VisitorName,
                                ContactNumber,
                                Date,
                                VisitPurpose,
                                TimeIn,
                                TimeOut
                                FROM VisitorsLog
                                ORDER BY Date DESC, TimeIn DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);


           
            
            }
        }
        private void DGVFormat()
        {
            VisitorDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            VisitorDGV.Columns["VisitDate"].DefaultCellStyle.Format = "MMM dd, yyyy";
            VisitorDGV.Columns["TimeIn"].DefaultCellStyle.Format = "hh :mm tt";
            VisitorDGV.Columns["TimeOut"].DefaultCellStyle.Format = "hh :mm tt";

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void VisitorLogFrm_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {


        }
    }
}
