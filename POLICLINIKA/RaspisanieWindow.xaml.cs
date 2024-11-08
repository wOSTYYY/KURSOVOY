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
    /// Логика взаимодействия для RaspisanieWindow.xaml
    /// </summary>
    public partial class RaspisanieWindow : Window
    {
        public RaspisanieWindow()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            // Загрузка списка врачей из базы данных
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_user, first_name, last_name, specialization_name, phone, email " +
                               "FROM Usersz INNER JOIN Specializations ON Usersz.id_specialization = Specializations.id_specialization " +
                               "WHERE role = 2";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var doctor = new
                    {
                        Id = reader["id_user"],
                        FullName = $"{reader["last_name"]} {reader["first_name"]}",
                        Specialization = reader["specialization_name"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Email = reader["email"].ToString()
                    };
                    ComboBoxDoctors.Items.Add(doctor);
                }
                conn.Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaKlient glavnayaKlient = new GlavnayaKlient();
            glavnayaKlient.Show();
            this.Close();
        }

        private void ComboBoxDoctors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxDoctors.SelectedItem != null)
            {
                dynamic selectedDoctor = ComboBoxDoctors.SelectedItem;
                DoctorName.Text = $"ФИО: {selectedDoctor.FullName}";
                DoctorSpecialization.Text = $"Специализация: {selectedDoctor.Specialization}";
                DoctorPhone.Text = $"Телефон: {selectedDoctor.Phone}";
                DoctorEmail.Text = $"Электронная почта: {selectedDoctor.Email}";

                LoadSchedule(selectedDoctor.Id);
            }
        }

        private void LoadSchedule(object doctorId)
        {
            ListViewSchedule.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT working_day, start_time, end_time FROM Doctors_Schedule WHERE id_doctor = @DoctorId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var schedule = new
                    {
                        Date = ((DateTime)reader["working_day"]).ToString("dd.MM.yyyy"),
                        StartTime = ((TimeSpan)reader["start_time"]).ToString(@"hh\:mm"),
                        EndTime = ((TimeSpan)reader["end_time"]).ToString(@"hh\:mm")
                    };
                    ListViewSchedule.Items.Add(schedule);
                }
                conn.Close();
            }
        }
    }
}
