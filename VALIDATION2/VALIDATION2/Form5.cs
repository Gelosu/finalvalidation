using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace VALIDATION2
{
    public partial class Form5 : Form
    {
        private string connectionString = "server=localhost;database=validationfile;uid=root;pwd=12345;";

        public Form5()
        {
            InitializeComponent();
            this.FormClosing += Form5_FormClosing;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadLogs();
            LoadDates();
            LogActivity("Logs Loaded");
        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogActivity("System closed"); 
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

    



private void LoadLogs(string filter = "", string dateFilter = "")
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT logs, datetime AS `DATE AND TIME` FROM logs";
                    if (!string.IsNullOrEmpty(filter) || !string.IsNullOrEmpty(dateFilter))
                    {
                        query += " WHERE";
                        if (!string.IsNullOrEmpty(filter))
                        {
                            query += " logs LIKE @filter";
                        }
                        if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(dateFilter))
                        {
                            query += " AND";
                        }
                        if (!string.IsNullOrEmpty(dateFilter))
                        {
                            query += " DATE(`datetime`) = @dateFilter";
                        }
                    }

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    if (!string.IsNullOrEmpty(filter))
                    {
                        cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                    }
                    if (!string.IsNullOrEmpty(dateFilter))
                    {
                        cmd.Parameters.AddWithValue("@dateFilter", dateFilter);
                    }
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    
                    if (dataGridView1.Columns["logs"] != null)
                    {
                        dataGridView1.Columns["logs"].HeaderText = "LOGS";
                    }
                    if (dataGridView1.Columns["DATE AND TIME"] != null)
                    {
                        dataGridView1.Columns["DATE AND TIME"].HeaderText = "DATE AND TIME";
                    }
                  
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void LoadDates()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT DISTINCT DATE(`datetime`) AS LogDate FROM logs";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("Select Date");
                    while (reader.Read())
                    {
                        DateTime logDate = Convert.ToDateTime(reader["LogDate"]);
                        string formattedDate = logDate.ToString("MMMM d yyyy", CultureInfo.InvariantCulture);
                        comboBox1.Items.Add(formattedDate);
                    }

                    comboBox1.SelectedIndex = 0; // Set default selection
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                string selectedDate = DateTime.ParseExact(comboBox1.SelectedItem.ToString(), "MMMM d yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                LoadLogs(textBox1.Text, selectedDate);
            }
            else
            {
                LoadLogs(textBox1.Text);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                string selectedDate = DateTime.ParseExact(comboBox1.SelectedItem.ToString(), "MMMM d yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                LoadLogs(textBox1.Text, selectedDate);
            }
            else
            {
                LoadLogs(textBox1.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filter = textBox1.Text;
            if (comboBox1.SelectedIndex > 0)
            {
                string selectedDate = DateTime.ParseExact(comboBox1.SelectedItem.ToString(), "MMMM d yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                LoadLogs(filter, selectedDate);
            }
            else
            {
                LoadLogs(filter);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
