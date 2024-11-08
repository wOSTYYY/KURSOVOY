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
    public partial class Vrach_PatientAppointmentsView : Window
    {
        public Vrach_PatientAppointmentsView()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT Appointments.id_patient, Appointments.appointment_date, Appointments.appointment_time, " +
                               "Usersz.first_name, Usersz.last_name, Usersz.sur_name " +
                               "FROM Appointments " +
                               "INNER JOIN Usersz ON Appointments.id_patient = Usersz.id_user " +
                               "WHERE Appointments.id_doctor = @DoctorId AND Appointments.status = 'Запланирован' " +
                               "ORDER BY Appointments.appointment_date, Appointments.appointment_time";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", Dannie.id_user); 

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var appointment = new
                    {
                        PatientId = reader["id_patient"],
                        AppointmentDate = ((DateTime)reader["appointment_date"]).ToString("dd.MM.yyyy"),
                        AppointmentTime = ((TimeSpan)reader["appointment_time"]).ToString(@"hh\:mm"),
                        PatientName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}"
                    };
                    ListViewAppointments.Items.Add(appointment);
                }
                conn.Close();
            }
        }

        private void ListViewAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewAppointments.SelectedItem != null)
            {
                dynamic selectedAppointment = ListViewAppointments.SelectedItem;

                AppointmentDetails.Text = $"Дата: {selectedAppointment.AppointmentDate}\n" +
                                          $"Время: {selectedAppointment.AppointmentTime}\n" +
                                          $"Пациент: {selectedAppointment.PatientName}";

                LoadPatientHistory(selectedAppointment.PatientId);
            }
        }

        private void LoadPatientHistory(object patientId)
        {
            ListBoxPatientHistory.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT TOP 1 visit_date, diagnosis, treatment " +
                               "FROM Medical_Cards " +
                               "WHERE id_patient = @PatientId " +
                               "ORDER BY visit_date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) 
                {
                    string visitDate = ((DateTime)reader["visit_date"]).ToString("dd.MM.yyyy");
                    string diagnosis = reader["diagnosis"].ToString();
                    string treatment = reader["treatment"].ToString();

                    string historyEntry = $"Дата визита: {visitDate}\nДиагноз: {diagnosis}\nЛечение: {treatment}";
                    ListBoxPatientHistory.Items.Add(historyEntry);
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
