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
using System.Drawing.Text;
using System.Text.RegularExpressions;

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

            if (!ValidationForRegistration())
            {
                return; 
            }


            string username = txtUname.Text;
            string pass = txtPass.Text;
            string Confirmpass = txtCPass.Text;
            string AuthorizedID = txtID.Text;

            string firstname = txtFname.Text;
            string lastname = txtLname.Text;
            string middlename = txtMname.Text;
            string address = txtAddress.Text;
            string email = txtEmail.Text;
            string number = txtContact.Text;
            string HOAposition = txtPosition.Text;

           

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
        private bool ValidationForRegistration()
        {
            string username = txtUname.Text;
            string pass = txtPass.Text;
            string Confirmpass = txtCPass.Text;
            string AuthorizedID = txtID.Text;

            string firstname = txtFname.Text;
            string lastname = txtLname.Text;
            string middlename = txtMname.Text;
            string address = txtAddress.Text;
            string email = txtEmail.Text;
            string number = txtContact.Text;
            string HOAposition = txtPosition.Text;

            string[] inputs = { username, pass, Confirmpass, firstname, lastname, middlename, address, email, number, HOAposition };

            foreach (string input in inputs)
            {
                if (!string.IsNullOrEmpty(input) && char.IsWhiteSpace(input[0]))
                {
                    MessageBox.Show("Space is not allowed at the beginning of any input.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(Confirmpass) || string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(lastname) || string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(number))
            {
                MessageBox.Show("Please fill in all required fields.");
                return false;
            }

            else if (AuthorizedID != "10010")
            {
                MessageBox.Show("You need a valid admin ID to register.");
                return false;
            }

            else if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Username must be alphanumeric.");
                return false;
            }

           else if (pass.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters.");
                return false;
            }

            else if (pass != Confirmpass)
            {
                MessageBox.Show("Passwords do not match.");
                return false;
            }

            else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email address.");
                return false;
            }

            else if (!Regex.IsMatch(number, @"^\d{11}$"))
            {
                MessageBox.Show("Contact number must be 11 digits");
                return false;
            }

            return true;
        }
    }
    

}/* SQL 
  *
  *CREATE TABLE TBL_Login(
userID INT IDENTITY(1,1) PRIMARY KEY,
username varchar(50) NOT NULL UNIQUE,
password_hash varchar(255) NOT NULL,
);

INSERT INTO TBL_Login (username, password_hash)
VALUES ('admin', '$2y$10$8geJ8LgHDFn1nB0Pv4n1peM...');


CREATE TABLE TBL_Register ( 
    UserID INT, 
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    MiddleName NVARCHAR(50),
    PositionInHOA NVARCHAR(100),
    CompleteAddress NVARCHAR(255),
    ContactNumber NVARCHAR(20),
    EmailAddress NVARCHAR(100),
    CONSTRAINT FK_Register_Login FOREIGN KEY (UserID) REFERENCES TBL_Login(UserID)
);
*/

        
    

