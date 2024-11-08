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
    /// Логика взаимодействия для AppointmentWindow.xaml
    /// </summary>
    public partial class AppointmentWindow : Window
    {
        public AppointmentWindow()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_user, first_name, last_name, sur_name, specialization_name, phone, email " +
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
                        FullName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}",
                        Specialization = reader["specialization_name"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Email = reader["email"].ToString()
                    };
                    ComboBoxDoctors.Items.Add(doctor);
                }
                conn.Close();
            }
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

                LoadAvailableTimeSlots();
            }
        }

        private void DatePickerAppointment_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAvailableTimeSlots();
        }


        private void LoadAvailableTimeSlots()
        {
            ListBoxTimeSlots.Items.Clear();

            if (ComboBoxDoctors.SelectedItem != null && DatePickerAppointment.SelectedDate != null)
            {
                var selectedDoctorId = ((dynamic)ComboBoxDoctors.SelectedItem).Id;
                var selectedDate = DatePickerAppointment.SelectedDate.Value;

                using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                {
                    string scheduleQuery = "SELECT start_time, end_time FROM Doctors_Schedule " +
                                           "WHERE id_doctor = @DoctorId AND working_day = @SelectedDate";
                    SqlCommand scheduleCmd = new SqlCommand(scheduleQuery, conn);
                    scheduleCmd.Parameters.AddWithValue("@DoctorId", selectedDoctorId);
                    scheduleCmd.Parameters.AddWithValue("@SelectedDate", selectedDate);

                    conn.Open();
                    SqlDataReader reader = scheduleCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var startTime = (TimeSpan)reader["start_time"];
                        var endTime = (TimeSpan)reader["end_time"];

                        reader.Close();

                        for (var time = startTime; time < endTime; time += TimeSpan.FromMinutes(30))
                        {
                            string appointmentQuery = "SELECT COUNT(*) FROM Appointments " +
                                                      "WHERE id_doctor = @DoctorId AND appointment_date = @SelectedDate AND appointment_time = @Time";
                            SqlCommand appointmentCmd = new SqlCommand(appointmentQuery, conn);
                            appointmentCmd.Parameters.AddWithValue("@DoctorId", selectedDoctorId);
                            appointmentCmd.Parameters.AddWithValue("@SelectedDate", selectedDate);
                            appointmentCmd.Parameters.AddWithValue("@Time", time);

                            int count = (int)appointmentCmd.ExecuteScalar();

                            if (count == 0)
                            {
                                ListBoxTimeSlots.Items.Add(time.ToString(@"hh\:mm"));
                            }
                        }
                    }

                    conn.Close();
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxDoctors.SelectedItem != null && DatePickerAppointment.SelectedDate != null && ListBoxTimeSlots.SelectedItem != null)
            {
                var selectedDoctorId = ((dynamic)ComboBoxDoctors.SelectedItem).Id;
                var selectedDate = DatePickerAppointment.SelectedDate.Value;
                var selectedTime = TimeSpan.Parse(ListBoxTimeSlots.SelectedItem.ToString());

                using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                {
                    string query = "INSERT INTO Appointments (id_patient, id_doctor, appointment_date, appointment_time, status) " +
                                   "VALUES (@PatientId, @DoctorId, @Date, @Time, 'Запланирован')";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientId", Dannie.id_user); 
                    cmd.Parameters.AddWithValue("@DoctorId", selectedDoctorId);
                    cmd.Parameters.AddWithValue("@Date", selectedDate);
                    cmd.Parameters.AddWithValue("@Time", selectedTime);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Вы успешно записаны на приём!", "Запись подтверждена", MessageBoxButton.OK, MessageBoxImage.Information);
                GlavnayaKlient glavnayaKlient = new GlavnayaKlient();
                glavnayaKlient.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля перед записью на приём.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaKlient glavnayaKlient = new GlavnayaKlient();
            glavnayaKlient.Show();
            this.Close();
        }
    }
}
