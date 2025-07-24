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
            VisitorLog();
        }
        private void VisitorLog() {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                VisitorID,
                                VisitorName,
                                ContactNumber,
                                FORMAT(Date, 'MMM dd, yyyy') AS Date,                           
                                VisitPurpose,
                                FORMAT(TimeIn, 'hh :mm tt') AS TimeIn,
                                CASE
                                    WHEN TimeOut IS NULL THEN 'Active'
                                    ELSE FORMAT(TimeOut, 'hh :mm tt')
                                END AS TimeOut                                                          
                                FROM TBL_VisitorsLog
                                ORDER BY Date DESC, TimeIn DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                VisitorDGV.DataSource = dt;
                DGVFormat(); 
            
            }
        }
        private void DGVFormat()
        {
            try
            {
                VisitorDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

               
                if (VisitorDGV.Columns.Contains("VisitDate"))
                    VisitorDGV.Columns["VisitDate"].DefaultCellStyle.Format = "MMM dd, yyyy";

                if (VisitorDGV.Columns.Contains("TimeIn"))
                    VisitorDGV.Columns["TimeIn"].DefaultCellStyle.Format = "hh:mm tt";

                if (VisitorDGV.Columns.Contains("TimeOut"))
                    VisitorDGV.Columns["TimeOut"].DefaultCellStyle.Format = "hh:mm tt";

                
                foreach (DataGridViewRow row in VisitorDGV.Rows)
                {
                    if (row.Cells["TimeOut"]?.Value?.ToString() == "Active")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error formatting grid: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        

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
            AddVisitor Add = new AddVisitor();
            if (Add.ShowDialog() == DialogResult.OK)
            {
                VisitorLog();
            }

        }

        private void btnRecordExit_Click(object sender, EventArgs e)
        {
            if (VisitorDGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a visitor first", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (VisitorDGV.SelectedRows[0].Cells["VisitorID"].Value == null ||
                VisitorDGV.SelectedRows[0].Cells["VisitorID"].Value == DBNull.Value)
            {
                MessageBox.Show("Invalid visitor record selected", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int visitorID = Convert.ToInt32(VisitorDGV.SelectedRows[0].Cells["VisitorID"].Value);

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "UPDATE TBL_VisitorsLog SET TimeOut = GETDATE() WHERE VisitorID = @VisitorID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@VisitorID", visitorID);
                    cmd.ExecuteNonQuery();
                    VisitorLog(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error recording exit: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                  }
            }
     }
    

