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
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace LoginAndRegisterForms
{
    public partial class LoginFrm : Form
    {
        private const string ConnectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";
        private const int MinimumPasswordLength = 8;
        public LoginFrm()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (!ValidateInputs(username, password))
            {
                return;
            }

            try
            {
                if (AuthenticateUser(username, password))
                {
                    ShowDashboard();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidateInputs(string username, string password)
        {
      
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        
            if (!Regex.IsMatch(username, @"^[^\s][A-Za-z]+(?:\s[A-Za-z]+)*(?:\s[A-Za-z]+)?$"))
            {
                MessageBox.Show("Username cannot start with a space and must contain only letters.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Focus();
                return false;
            }

        
            if (password.Length < MinimumPasswordLength)
            {
                MessageBox.Show($"Password must be at least {MinimumPasswordLength} characters.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Focus();
                return false;
            }

            return true;
        }


        private bool AuthenticateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string query = "SELECT password_hash FROM TBL_Login WHERE username = @username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    var result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Username not found.", "Login Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    string storedHash = result.ToString();
                    if (!BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        MessageBox.Show("Invalid password.", "Login Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    return true;
                }
            }
        }
        private void ShowDashboard()
        {
            MessageBox.Show("Login Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Dashboardfrm dashboard = new Dashboardfrm();
            dashboard.Show();
            this.Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegistrationForm f2 = new RegistrationForm();
            f2.Show();
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPassForm F3 = new ForgotPassForm();
            F3.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
