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

namespace LoginAndRegisterForms
{
    public partial class LoginFrm : Form
    {
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


            string connectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT password_hash FROM TBL_Login WHERE username = @username";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            string storedHash = result.ToString();

                            if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                            {
                                MessageBox.Show("Login Succesful");
                            }
                            else
                            {
                                MessageBox.Show("Invalid Password");

                            }
                        }
                        else
                        {
                            MessageBox.Show("Username not found.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
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
