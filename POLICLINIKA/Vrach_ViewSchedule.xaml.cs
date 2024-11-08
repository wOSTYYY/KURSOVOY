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
    public partial class Vrach_ViewSchedule : Window
    {
        public Vrach_ViewSchedule()
        {
            InitializeComponent();
            LoadScheduleForSelectedDate(DateTime.Now);
        }

        private void DatePickerSchedule_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePickerSchedule.SelectedDate != null)
            {
                DateTime selectedDate = DatePickerSchedule.SelectedDate.Value;
                LoadScheduleForSelectedDate(selectedDate);
            }
        }

        private void LoadScheduleForSelectedDate(DateTime date)
        {
            ListViewPatients.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT Appointments.appointment_time, Usersz.first_name, Usersz.last_name, Usersz.sur_name " +
                               "FROM Appointments " +
                               "INNER JOIN Usersz ON Appointments.id_patient = Usersz.id_user " +
                               "WHERE Appointments.id_doctor = @DoctorId AND Appointments.appointment_date = @Date " +
                               "ORDER BY Appointments.appointment_time";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", Dannie.id_user); 
                cmd.Parameters.AddWithValue("@Date", date);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var appointment = new
                    {
                        AppointmentTime = ((TimeSpan)reader["appointment_time"]).ToString(@"hh\:mm"),
                        PatientName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}"
                    };
                    ListViewPatients.Items.Add(appointment);
                }
                conn.Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaVrach glavnayaVrach = new GlavnayaVrach();
            glavnayaVrach.Show();
            this.Close();
        }
    }
}
