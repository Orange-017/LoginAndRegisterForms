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
        private const string ConnectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";
        private const int MinimumPasswordLength = 8;

        public ForgotPassForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }
            string username = txtUname.Text.Trim();
            if (!UsernameExists(username))
            {
                MessageBox.Show("Username does not exist.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                ResetUserPassword(txtUname.Text.Trim(), txtNewPass.Text);
                MessageBox.Show("Password has been reset successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool UsernameExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM TBL_Login WHERE username = @username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }
        private bool ValidateInputs()
        {
            
            if (string.IsNullOrWhiteSpace(txtUname.Text) ||
                string.IsNullOrWhiteSpace(txtNewPass.Text) ||
                string.IsNullOrWhiteSpace(txtNewCPass.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

           
            if (txtNewPass.Text.Length < MinimumPasswordLength)
            {
                MessageBox.Show($"Password must be at least {MinimumPasswordLength} characters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            
            if (txtNewPass.Text != txtNewCPass.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ResetUserPassword(string username, string newPassword)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        UpdatePassword(conn, transaction, username, hashedPassword);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

      
        private void UpdatePassword(SqlConnection conn, SqlTransaction transaction, string username, string hashedPassword)
        {
            string updateQuery = "UPDATE TBL_Login SET password_hash = @password WHERE username = @username";
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.ExecuteNonQuery();
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
