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
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUname.Text.Trim();
            string pass = txtPass.Text;
            string Confirmpass = txtCPass.Text;

            string firstname = txtFname.Text;
            string lastname = txtLname.Text;
            string middlename = txtMname.Text;
            string address = txtAddress.Text;
            string email = txtEmail.Text;
            string number = txtContact.Text;
            string HOAposition = txtPosition.Text;

            if (pass != Confirmpass) {
                MessageBox.Show("not match");
                return;
            }
            string connectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null; 
              try  {
                    conn.Open();
                    transaction = conn.BeginTransaction(); 

                   
                    string checkUserQuery = "SELECT COUNT(1) FROM TBL_Login WHERE username = @username";
                    using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        int userCount = (int)checkCmd.ExecuteScalar();
                        if (userCount > 0)
                        {
                            MessageBox.Show("Username already taken. Please choose a different one.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback(); 
                            return;
                        }
                    }

                   
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);

                  
                    string insertLoginQuery = "INSERT INTO TBL_Login (username, password_hash) VALUES (@username, @password_hash); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand loginCmd = new SqlCommand(insertLoginQuery, conn, transaction))
                    {
                        loginCmd.Parameters.AddWithValue("@username", username);
                        loginCmd.Parameters.AddWithValue("@password_hash", hashedPassword);

                      
                        int newUserId = Convert.ToInt32(loginCmd.ExecuteScalar());
                        string insertRegisterQuery = "INSERT INTO TBL_Register (UserID, FirstName, LastName, MiddleName, PositionInHOA, CompleteAddress, ContactNumber, EmailAddress) " +
                                                     "VALUES (@userID, @firstName, @lastName, @middleName, @hoaPosition, @address, @contactNumber, @email)";

                        using (SqlCommand registerCmd = new SqlCommand(insertRegisterQuery, conn, transaction))
                        {
                            registerCmd.Parameters.AddWithValue("@userID", newUserId);
                            registerCmd.Parameters.AddWithValue("@firstName", firstname);
                            registerCmd.Parameters.AddWithValue("@lastName", lastname);
                            registerCmd.Parameters.AddWithValue("@middleName", middlename);
                            registerCmd.Parameters.AddWithValue("@hoaPosition", HOAposition); 
                            registerCmd.Parameters.AddWithValue("@address", address);
                            registerCmd.Parameters.AddWithValue("@contactNumber", number); 
                            registerCmd.Parameters.AddWithValue("@email", email);

                            registerCmd.ExecuteNonQuery();

                            transaction.Commit(); 
                            MessageBox.Show("Registration Successful! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close(); 
                        }
                    }
                }             
                catch (SqlException ex)
                {
                    
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    MessageBox.Show("A database error occurred during registration: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    MessageBox.Show("An unexpected error occurred: " + ex.Message, "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
        
    

