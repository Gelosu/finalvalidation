using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using VALIDATION2;
using System.Text;
using System.Security.Cryptography;

namespace VALIDATION2
{
    public partial class Form1 : Form
    {
        private string connectionString = "server=localhost;database=validationfile;uid=root;pwd=12345;";

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                LogActivity("System closed");
            }
        }
        private void createlogs()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();


                    
                    string checklogsQuery = "SHOW TABLES LIKE 'logs'";
                    using (MySqlCommand checklogsCmd = new MySqlCommand(checklogsQuery, connection))
                    {
                        object result = checklogsCmd.ExecuteScalar();
                        if (result == null)
                        {
                            string createlogsQuery = "CREATE TABLE logs (" +
                                "logs VARCHAR(255) , " + "`DATETIME` DATETIME)";
                            using (MySqlCommand createlogCmd = new MySqlCommand(createlogsQuery, connection))
                            {
                                createlogCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create tables. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createlogs();

        }
        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {

         
            string username = textBox1.Text;
            string password = textBox2.Text;
            string hashedPassword = HashPassword(password);

            if (ValidateCredentials(username, hashedPassword))
            {
              LogActivity("Login Successfully");
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
            }
            else
            {

              LogActivity($"Login Failed");

     
            MessageBox.Show($"Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM credentials WHERE username = @username AND password = @password";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password); 

                        int result = Convert.ToInt32(cmd.ExecuteScalar());
                        return result > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to validate credentials. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

      
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            { 
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                string hashedPassword = Convert.ToHexString(hashBytes);

                return hashedPassword;
            }
        }


        private void LogActivity(string message)
        {
            string logQuery = "INSERT INTO logs (logs, datetime) VALUES (@log, @datetime)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(logQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@log", message);
                        cmd.Parameters.AddWithValue("@datetime", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to log activity. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
