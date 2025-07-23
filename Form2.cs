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
        private const string ConnectionString = "Data Source=LAPTOP-FT905FTC\\SQLEXPRESS;Initial Catalog=RecordManagement;Integrated Security=True;";
        private const string AdminId = "10010";
        private const int MinimumPasswordLength = 8;
        private const int ContactNumberLength = 11;

        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
        }


        private void button1_Click(object sender, EventArgs e)
        {

            if (!ValidateRegistrationInputs())
            {
                return;
            }

            try
            {
                RegisterNewUser();
                MessageBox.Show("Registration Successful! You can now log in.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"A database error occurred: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Registration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateRegistrationInputs()
        {
            
            if (string.IsNullOrWhiteSpace(txtUname.Text) ||
                string.IsNullOrWhiteSpace(txtPass.Text) ||
                string.IsNullOrWhiteSpace(txtCPass.Text) ||
                string.IsNullOrWhiteSpace(txtFname.Text) ||
                string.IsNullOrWhiteSpace(txtLname.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

         
            if (txtID.Text != AdminId)
            {
                MessageBox.Show("You need a valid admin ID to register.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            
            if (HasLeadingWhitespace(txtUname.Text, txtPass.Text, txtCPass.Text, txtFname.Text,
                txtLname.Text, txtMname.Text, txtAddress.Text, txtEmail.Text, txtContact.Text, txtPosition.Text))
            {
                MessageBox.Show("Space is not allowed at the beginning of any input.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

           
            if (!Regex.IsMatch(txtUname.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Username must be alphanumeric with no spaces.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

          
            if (txtPass.Text.Length < MinimumPasswordLength)
            {
                MessageBox.Show($"Password must be at least {MinimumPasswordLength} characters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (txtPass.Text != txtCPass.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

          
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email address format.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

          
            if (!Regex.IsMatch(txtContact.Text, @"^\d{" + ContactNumberLength + "}$"))
            {
                MessageBox.Show($"Contact number must be exactly {ContactNumberLength} digits.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool HasLeadingWhitespace(params string[] inputs)
        {
            foreach (string input in inputs)
            {
                if (!string.IsNullOrEmpty(input) && char.IsWhiteSpace(input[0]))
                {
                    return true;
                }
            }
            return false;
        }

        private void RegisterNewUser()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        
                        if (UsernameExists(conn, transaction, txtUname.Text))
                        {
                            MessageBox.Show("Username already taken. Please choose a different one.",
                                "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int newUserId = InsertLoginCredentials(conn, transaction, txtUname.Text, txtPass.Text);

                       
                        InsertUserDetails(conn, transaction, newUserId, txtFname.Text, txtLname.Text,
                            txtMname.Text, txtPosition.Text, txtAddress.Text, txtContact.Text, txtEmail.Text);

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
        private bool UsernameExists(SqlConnection conn, SqlTransaction transaction, string username)
        {
            string query = "SELECT COUNT(1) FROM TBL_Login WHERE username = @username";
            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@username", username);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private int InsertLoginCredentials(SqlConnection conn, SqlTransaction transaction, string username, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            string query = "INSERT INTO TBL_Login (username, password_hash) VALUES (@username, @password_hash); SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password_hash", hashedPassword);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void InsertUserDetails(SqlConnection conn, SqlTransaction transaction, int userId,
            string firstName, string lastName, string middleName, string position,
            string address, string contactNumber, string email)
        {
            string query = @"INSERT INTO TBL_Register 
                            (UserID, FirstName, LastName, MiddleName, PositionInHOA, CompleteAddress, ContactNumber, EmailAddress) 
                            VALUES 
                            (@userID, @firstName, @lastName, @middleName, @hoaPosition, @address, @contactNumber, @email)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@userID", userId);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@middleName", middleName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@hoaPosition", position);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@contactNumber", contactNumber);
                cmd.Parameters.AddWithValue("@email", email);

                cmd.ExecuteNonQuery();
            }
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

        
    

