using Microsoft.VisualBasic.Devices;
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
    public partial class Form7 : Form


    {
        private string originalCourse;
        private string originalCourseDescription;
        private string connectionString = "server=localhost;database=validationfile;uid=root;pwd=12345;";

        public Form7(string course, string courseDescription)
        {
            
            InitializeComponent();
            originalCourse = course;
            originalCourseDescription = courseDescription;
            textBox1.Text = course;
            textBox2.Text = courseDescription;
            
        }

      
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string updatedCourse = textBox1.Text;
            string updatedCourseDescription = textBox2.Text;

            if (!string.IsNullOrEmpty(updatedCourse) && !string.IsNullOrEmpty(updatedCourseDescription))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "UPDATE course SET COURSE = @updatedCourse, COURSE_DESCRIPTION = @updatedCourseDescription WHERE COURSE = @originalCourse AND COURSE_DESCRIPTION = @originalCourseDescription";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@updatedCourse", updatedCourse);
                            command.Parameters.AddWithValue("@updatedCourseDescription", updatedCourseDescription);
                            command.Parameters.AddWithValue("@originalCourse", originalCourse);
                            command.Parameters.AddWithValue("@originalCourseDescription", originalCourseDescription);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Course updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LogActivity($"Course Updated: {updatedCourse}");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating the course: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Course and Course Description cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form7_Load(object sender, EventArgs e)
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
    }
}
