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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VALIDATION2
{
    public partial class Form4 : Form
    {
        public event EventHandler Form4Closed;
        private string connectionString = "server=localhost;database=validationfile;uid=root;pwd=12345;";

        public Form4()
        {
            InitializeComponent();
            LoadCourses();
            this.FormClosed += Form4_FormClosed;
        }

        private void LoadCourses()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COURSE, COURSE_DESCRIPTION, STATUS FROM course";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    listBox3.Items.Clear();

                    while (reader.Read())
                    {
                        listBox1.Items.Add(reader["COURSE"].ToString());
                        listBox2.Items.Add(reader["COURSE_DESCRIPTION"].ToString());
                        listBox3.Items.Add(reader["STATUS"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadCourses();
                return;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SearchCourses();
        }

        private void SearchCourses()
        {
            string searchText = textBox1.Text.ToLower();



            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COURSE, COURSE_DESCRIPTION, STATUS FROM course WHERE LOWER(COURSE) LIKE @searchText OR LOWER(COURSE_DESCRIPTION) LIKE @searchText";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                    MySqlDataReader reader = cmd.ExecuteReader();

                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    listBox3.Items.Clear();

                    while (reader.Read())
                    {
                        listBox1.Items.Add(reader["COURSE"].ToString());
                        listBox2.Items.Add(reader["COURSE_DESCRIPTION"].ToString());
                        listBox3.Items.Add(reader["STATUS"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            form6.Show();
            form6.Form6Closed += Form6_Form6Closed;
        }
        private void Form6_Form6Closed(object sender, EventArgs e)
        {

            this.ReloadForm2();
        }

        private void ReloadForm2()
        {
            LoadCourses();


        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox2.SelectedItem != null)
            {
                string selectedCourse = listBox1.SelectedItem.ToString();
                string selectedCourseDescription = listBox2.SelectedItem.ToString();

                Form7 form7 = new Form7(selectedCourse, selectedCourseDescription);
                form7.ShowDialog();


                LoadCourses();
            }
            else
            {
                MessageBox.Show("Please select a course and its description to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateButtons()
        {
            if (listBox3.SelectedItem != null)
            {
                string status = listBox3.SelectedItem.ToString();
                if (status == "ENABLED")
                {
                    button4.Enabled = false;
                    button5.Enabled = true;
                }
                else if (status == "DISABLED")
                {
                    button4.Enabled = true;
                    button5.Enabled = false;
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            UpdateStatus("ENABLED");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UpdateStatus("DISABLED");
        }

        private void UpdateStatus(string newStatus)
        {
            if (listBox1.SelectedItem == null || listBox2.SelectedItem == null || listBox3.SelectedItem == null)
            {
                MessageBox.Show("Please select a course, course description, and status to update.");
                return;
            }

            string course = listBox1.SelectedItem.ToString();
            string courseDescription = listBox2.SelectedItem.ToString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryUpdate = "UPDATE course SET STATUS = @newStatus WHERE COURSE = @course AND COURSE_DESCRIPTION = @courseDescription";

                    connection.Open();

                    using (MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, connection))
                    {
                        commandUpdate.Parameters.AddWithValue("@newStatus", newStatus);
                        commandUpdate.Parameters.AddWithValue("@course", course);
                        commandUpdate.Parameters.AddWithValue("@courseDescription", courseDescription);

                        int rowsAffected = commandUpdate.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Course status updated successfully.");
                            LoadCourses();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update course status.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || listBox2.SelectedItem == null || listBox3.SelectedItem == null)
            {
                MessageBox.Show("Please select a course, course description, and status to delete.");
                return;
            }

            string course = listBox1.SelectedItem.ToString();
            string courseDescription = listBox2.SelectedItem.ToString();
            string status = listBox3.SelectedItem.ToString();

            DialogResult result = MessageBox.Show("Are you sure you want to delete the selected course?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryDelete = "DELETE FROM COURSE WHERE COURSE = @course AND COURSE_DESCRIPTION = @courseDescription AND STATUS = @status";

                    try
                    {
                        connection.Open();

                        using (MySqlCommand commandDelete = new MySqlCommand(queryDelete, connection))
                        {
                            commandDelete.Parameters.AddWithValue("@course", course);
                            commandDelete.Parameters.AddWithValue("@courseDescription", courseDescription);
                            commandDelete.Parameters.AddWithValue("@status", status);

                            int rowsAffected = commandDelete.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Course deleted successfully.");
                                LoadCourses();
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete course.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                listBox2.SelectedIndex = listBox1.SelectedIndex;
                listBox3.SelectedIndex = listBox1.SelectedIndex;
                UpdateButtons();
            }
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = listBox2.SelectedIndex;
                listBox3.SelectedIndex = listBox2.SelectedIndex;
                UpdateButtons();
            }
        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = listBox3.SelectedIndex;
                listBox2.SelectedIndex = listBox3.SelectedIndex;
                UpdateButtons();
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form4Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
