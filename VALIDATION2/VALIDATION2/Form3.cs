using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VALIDATION2
{
    public partial class Form3 : Form
    {
        public event EventHandler Form3Closed;
        private string connectionString = "server=localhost;database=validationfile;uid=root;pwd=12345;";

        public Form3()
        {
            InitializeComponent();
            this.FormClosed += Form3_FormClosed;
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form3Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string semester = radioButton1.Checked ? "1st" : radioButton2.Checked ? "2nd" : "";

            if (string.IsNullOrEmpty(semester))
            {
                MessageBox.Show("Please select a semester.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string inputTableName = $"Validation ({semester} Sem {textBox2.Text})";
            string newTableName = SanitizeTableName(inputTableName);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    if (DoesTableExist(connection, newTableName))
                    {
                        MessageBox.Show("Table already exists. Please create another name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string lastTableName = GetLastTableName(connection);

                    CreateNewTable(newTableName, lastTableName, connection);


                    MessageBox.Show($"To update the status, please upload this filename: {inputTableName}");

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create table or copy data. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string SanitizeTableName(string tableName)
        {
            return Regex.Replace(tableName, "[^a-zA-Z0-9_]", "_");
        }

        private bool DoesTableExist(MySqlConnection connection, string tableName)
        {
            string query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'validationfile' AND table_name = '{tableName}'";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private string GetLastTableName(MySqlConnection connection)
        {
            string query = "SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA = 'validationfile' AND TABLE_NAME NOT IN ('credentials', 'logs', 'faculty', 'course') ORDER BY CREATE_TIME DESC LIMIT 1";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();
                return result?.ToString();
            }
        }

        private void CreateNewTable(string newTableName, string sourceTableName, MySqlConnection connection)
        {
            string query = $"CREATE TABLE `{newTableName}` LIKE `{sourceTableName}`";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

       

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
