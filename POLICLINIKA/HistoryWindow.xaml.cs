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
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
            LoadActiveAppointments();
        }

        private void LoadActiveAppointments()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT Appointments.id_appointment, Appointments.appointment_date, Appointments.appointment_time, " +
                               "Usersz.first_name, Usersz.last_name, Usersz.sur_name " +
                               "FROM Appointments " +
                               "INNER JOIN Usersz ON Appointments.id_doctor = Usersz.id_user " +
                               "WHERE Appointments.id_patient = @PatientId AND Appointments.status = 'Запланирован'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", Dannie.id_user); 

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var appointment = new
                    {
                        AppointmentId = reader["id_appointment"],
                        AppointmentDate = ((DateTime)reader["appointment_date"]).ToString("dd.MM.yyyy"),
                        AppointmentTime = ((TimeSpan)reader["appointment_time"]).ToString(@"hh\:mm"),
                        DoctorName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}"
                    };
                    ListViewActiveAppointments.Items.Add(appointment);
                }
                conn.Close();
            }
        }

        private void ListViewActiveAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewActiveAppointments.SelectedItem != null)
            {
                dynamic selectedAppointment = ListViewActiveAppointments.SelectedItem;

                string message = $"Вы выбрали запись:\n" +
                                 $"Дата: {selectedAppointment.AppointmentDate}\n" +
                                 $"Время: {selectedAppointment.AppointmentTime}\n" +
                                 $"Врач: {selectedAppointment.DoctorName}";

                MessageBox.Show(message, "Информация о записи", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
            this.Close();
        }

        private void ShowHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryDialog historyDialog = new HistoryDialog();
            historyDialog.Owner = this;
            historyDialog.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewActiveAppointments.SelectedItem != null)
            {
                dynamic selectedAppointment = ListViewActiveAppointments.SelectedItem;
                int appointmentId = selectedAppointment.AppointmentId;

                using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                {
                    string query = "DELETE FROM Appointments WHERE id_appointment = @AppointmentId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Запись удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ListViewActiveAppointments.Items.Remove(selectedAppointment);
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
