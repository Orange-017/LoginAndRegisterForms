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
    public partial class ForgotPassForm : Form
    {
        string connectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";

        public ForgotPassForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUname.Text;
            string newpass = txtNewPass.Text;
            string newConfirmPass = txtNewCPass.Text;


            if (newpass != newConfirmPass)
            {
                MessageBox.Show("Password do not match");
                return;

            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newpass);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(1) FROM TBL_Login WHERE username = @username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    int userExists = (int)cmd.ExecuteScalar();

                    if (userExists == 0)
                    {
                        MessageBox.Show("Username does not exist.");
                        return;
                    }
                }
                string updatePasswordQuery = "UPDATE TBL_Login SET password_hash = @password WHERE username = @username";
                using (SqlCommand cmd = new SqlCommand(updatePasswordQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Password has been reset successfully.");
                this.Close();
            }
        }
        
        
        

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void ForgotPassForm_Load(object sender, EventArgs e)
        {

        }
    }
}
