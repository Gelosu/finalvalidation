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
            LogActivity("System closed");
            Form3Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            CheckEnableButton();
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

            DialogResult result = MessageBox.Show($"Are you sure that the information selected and input is correct? '{newTableName}'?",
                                                  "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                LogActivity("Creating of new semester didn't proceed");
                return;
            }

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

                    CreateNewTable(newTableName, connection);

                    MessageBox.Show($"To update the status, please upload this filename: {inputTableName}");
                    LogActivity($"Creating of new semester successful: table name: {inputTableName}");
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

        private void CreateNewTable(string newTableName, MySqlConnection connection)
        {
            string query = $"CREATE TABLE `{newTableName}` (QRCODE VARCHAR(255), TUPCID VARCHAR(255), UID VARCHAR(255), STATUS VARCHAR(255), DATETIME DATETIME)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            CheckEnableButton();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            CheckEnableButton();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            CheckEnableButton();
        }

        private void CheckEnableButton()
        {
            bool isTextBoxFilled = !string.IsNullOrEmpty(textBox2.Text);
            bool isRadioButtonChecked = radioButton1.Checked || radioButton2.Checked;

            button1.Enabled = isTextBoxFilled && isRadioButtonChecked;
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
    }
}
