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
    public partial class AddVisitor : Form
    {
        private const string ConnectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";

        public AddVisitor()
        {
            InitializeComponent();
            DTPdate.Value = DateTime.Now;
            DTPtime.Value = DateTime.Now;

            DTPdate.Format = DateTimePickerFormat.Custom;
            DTPdate.CustomFormat = "dddd, dd MMMM yyyy"; 

            DTPtime.Format = DateTimePickerFormat.Time;
            DTPtime.ShowUpDown = true;


        }


        private void AddVisitor_Load(object sender, EventArgs e)
        {

        }

        private void Savebtn_Click(object sender, EventArgs e)
        {

            if (!ValidateInputs()) return;
           
           using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO TBL_VisitorsLog
                                (VisitorName, ContactNumber, Date, VisitPurpose, TimeIn)
                                VALUES 
                                (@Name, @ContactNumber, @Date, @Purpose, @TimeIn)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@ContactNumber", txtContactNumber.Text);
                cmd.Parameters.AddWithValue("@Date", DTPdate.Value.Date);
                cmd.Parameters.AddWithValue("@Purpose", txtPurpose.Text);
                cmd.Parameters.AddWithValue("@TimeIn", DTPtime.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }


        }
        private bool ValidateInputs() {



            if (string.IsNullOrWhiteSpace(txtName.Text) ||
               string.IsNullOrWhiteSpace(txtContactNumber.Text) ||
               string.IsNullOrWhiteSpace(txtPurpose.Text))
               
            {
                MessageBox.Show("Please fill in all required fields.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
/* CREATE TABLE 

CREATE TABLE TBL_VisitorsLog (
VisitorID INT PRIMARY KEY IDENTITY(1,1),
VisitorName VARCHAR(100) NOT NULL,
ContactNumber VARCHAR(20),
Date DATETIME NOT NULL DEFAULT GETDATE(),
VisitPurpose VARCHAR(200),
TimeIn DATETIME NOT NULL,
TimeOut DATETIME NULL,
)

*/
