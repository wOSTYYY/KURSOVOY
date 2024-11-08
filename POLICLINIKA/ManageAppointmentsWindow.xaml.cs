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
    /// Логика взаимодействия для ManageAppointmentsWindow.xaml
    /// </summary>
    public partial class ManageAppointmentsWindow : Window
    {
        public ManageAppointmentsWindow()
        {
            InitializeComponent();
            LoadPatientRecords();
        }

        private void LoadPatientRecords()
        {
            DataGridPatientRecords.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = @"
                    SELECT a.id_appointment, u.last_name + ' ' + u.first_name AS PatientName, 
                           a.appointment_date, a.appointment_time, 
                           d.last_name + ' ' + d.first_name AS DoctorName, a.status 
                    FROM Appointments a
                    JOIN Usersz u ON a.id_patient = u.id_user
                    JOIN Usersz d ON a.id_doctor = d.id_user
                    ORDER BY a.appointment_date, a.appointment_time";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var record = new PatientRecord
                    {
                        IdAppointment = (int)reader["id_appointment"],
                        PatientName = reader["PatientName"].ToString(),
                        AppointmentDate = ((DateTime)reader["appointment_date"]).ToString("dd.MM.yyyy"),
                        AppointmentTime = ((TimeSpan)reader["appointment_time"]).ToString(@"hh\:mm"),
                        DoctorName = reader["DoctorName"].ToString(),
                        Status = reader["status"].ToString()
                    };
                    DataGridPatientRecords.Items.Add(record);
                }
                conn.Close();
            }
        }


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void DateFilterPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string searchText = SearchTextBox.Text.ToLower();
            string selectedStatus = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime? selectedDate = DateFilterPicker.SelectedDate;

            foreach (var item in DataGridPatientRecords.Items)
            {
                if (item is PatientRecord record)
                {
                    bool isVisible = (string.IsNullOrEmpty(searchText) || record.PatientName.ToLower().Contains(searchText)) &&
                                     (selectedStatus == "Все" || record.Status == selectedStatus) &&
                                     (!selectedDate.HasValue || record.AppointmentDate == selectedDate.Value.ToString("dd.MM.yyyy"));

                    DataGridRow row = (DataGridRow)DataGridPatientRecords.ItemContainerGenerator.ContainerFromItem(item);
                    if (row != null)
                        row.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void DataGridPatientRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridPatientRecords.SelectedItem is PatientRecord selectedRecord)
            {
                MessageBox.Show($"Пациент: {selectedRecord.PatientName}\n" +
                                             $"Дата: {selectedRecord.AppointmentDate}\n" +
                                             $"Время: {selectedRecord.AppointmentTime}\n" +
                                             $"Врач: {selectedRecord.DoctorName}\n" +
                                             $"Статус: {selectedRecord.Status}");

                EditStatusButton.IsEnabled = true;
            }
            else
            {
                EditStatusButton.IsEnabled = false;
            }
        }

        private void EditStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPatientRecords.SelectedItem is PatientRecord selectedAppointment)
            {
                int appointmentId = selectedAppointment.IdAppointment;
                string currentStatus = selectedAppointment.Status; // Получаем текущий статус из выбранной записи

                var dialog = new EditStatusDialog(appointmentId, currentStatus);
                if (dialog.ShowDialog() == true)
                {
                    LoadPatientRecords(); // Перезагружаем список после изменения статуса
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaAdmin mainAdminWindow = new GlavnayaAdmin();
            mainAdminWindow.Show();
            this.Close();
        }

        public class PatientRecord
        {
            public int IdAppointment { get; set; }
            public string PatientName { get; set; }
            public string AppointmentDate { get; set; }
            public string AppointmentTime { get; set; }
            public string DoctorName { get; set; }
            public string Status { get; set; }
        }
    }
}
