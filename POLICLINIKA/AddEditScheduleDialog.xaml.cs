using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POLICLINIKA
{
    /// <summary>
    /// Логика взаимодействия для AddEditScheduleDialog.xaml
    /// </summary>
    public partial class AddEditScheduleDialog : Window
    {
        private int? _scheduleId;
        public AddEditScheduleDialog(int? scheduleId = null)
        {
            InitializeComponent();
            _scheduleId = scheduleId;
            LoadDoctors();

            if (_scheduleId.HasValue)
            {
                LoadScheduleData();
            }
        }

        private void LoadDoctors()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_user, CONCAT(last_name, ' ', first_name) AS FullName FROM Usersz WHERE role = 2";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ComboBoxDoctors.Items.Clear();
                while (reader.Read())
                {
                    ComboBoxDoctors.Items.Add(new
                    {
                        Id = reader["id_user"],
                        FullName = reader["FullName"].ToString()
                    });
                }
                conn.Close();
            }
        }

        private void LoadScheduleData()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_doctor, working_day, start_time, end_time FROM Doctors_Schedule WHERE id_schedule = @ScheduleId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ScheduleId", _scheduleId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    DatePickerWorkingDay.SelectedDate = (DateTime)reader["working_day"];
                    TextBoxStartTime.Text = reader["start_time"].ToString();
                    TextBoxEndTime.Text = reader["end_time"].ToString();

                    foreach (var item in ComboBoxDoctors.Items)
                    {
                        if (((dynamic)item).Id == (int)reader["id_doctor"])
                        {
                            ComboBoxDoctors.SelectedItem = item;
                            break;
                        }
                    }
                }
                conn.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxDoctors.SelectedItem == null || DatePickerWorkingDay.SelectedDate == null ||
                            string.IsNullOrWhiteSpace(TextBoxStartTime.Text) || string.IsNullOrWhiteSpace(TextBoxEndTime.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var doctorId = ((dynamic)ComboBoxDoctors.SelectedItem).Id;
            var workingDay = DatePickerWorkingDay.SelectedDate.Value;
            var startTime = TimeSpan.Parse(TextBoxStartTime.Text);
            var endTime = TimeSpan.Parse(TextBoxEndTime.Text);

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query;
                if (_scheduleId.HasValue)
                {
                    query = "UPDATE Doctors_Schedule SET id_doctor = @DoctorId, working_day = @WorkingDay, start_time = @StartTime, end_time = @EndTime WHERE id_schedule = @ScheduleId";
                }
                else
                {
                    query = "INSERT INTO Doctors_Schedule (id_doctor, working_day, start_time, end_time) VALUES (@DoctorId, @WorkingDay, @StartTime, @EndTime)";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                cmd.Parameters.AddWithValue("@WorkingDay", workingDay);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);

                if (_scheduleId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", _scheduleId.Value);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
