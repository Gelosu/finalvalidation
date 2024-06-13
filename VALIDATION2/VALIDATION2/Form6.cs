using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VALIDATION2
{
    public partial class Form6 : Form
    {
        public event EventHandler Form6Closed;
        private string connectionString = "server=localhost;database=validationfile;uid=root;pwd=12345;";
        public Form6()
        {
            InitializeComponent();
            this.FormClosed += Form6_FormClosed;
        }
        private void Form6_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form6Closed?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string course = textBox1.Text;
            string courseDescription = textBox2.Text;

            if (string.IsNullOrEmpty(course) || string.IsNullOrEmpty(courseDescription))
            {
                MessageBox.Show("Please fill in both course and course description.");
                return;
            }


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string queryCheck = "SELECT COUNT(*) FROM course WHERE COURSE = @course OR COURSE_DESCRIPTION = @courseDescription";
                string queryInsert = "INSERT INTO COURSE (COURSE, COURSE_DESCRIPTION, STATUS) VALUES (@course, @courseDescription, 'ENABLED')";

                try
                {
                    connection.Open();


                    using (MySqlCommand commandCheck = new MySqlCommand(queryCheck, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@course", course);
                        commandCheck.Parameters.AddWithValue("@courseDescription", courseDescription);

                        int count = Convert.ToInt32(commandCheck.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Course or Course Description already exists.");
                            return;
                        }
                    }


                    using (MySqlCommand commandInsert = new MySqlCommand(queryInsert, connection))
                    {
                        commandInsert.Parameters.AddWithValue("@course", course);
                        commandInsert.Parameters.AddWithValue("@courseDescription", courseDescription);

                        int rowsAffected = commandInsert.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Course added successfully.");
                            LogActivity($"Course added: {course} ");

                        }
                        else
                        {
                            MessageBox.Show("Failed to add course.");
                            LogActivity($"Course failed to add: {course} ");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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

        private void Form6_Load(object sender, EventArgs e)
        {

        }
    }
}
