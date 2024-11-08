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
    /// Логика взаимодействия для Vrach_MedicalCardManagement.xaml
    /// </summary>
    public partial class Vrach_MedicalCardManagement : Window
    {
        public Vrach_MedicalCardManagement()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_user, first_name, last_name, sur_name FROM Usersz WHERE role = 1"; // role = 1 для пациентов

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var patient = new
                    {
                        Id = reader["id_user"],
                        FullName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}"
                    };
                    ComboBoxPatients.Items.Add(patient);
                }
                conn.Close();
            }
        }

        private void ComboBoxPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxPatients.SelectedItem != null)
            {
                dynamic selectedPatient = ComboBoxPatients.SelectedItem;
                LoadMedicalHistory(selectedPatient.Id);
            }
        }

        private void LoadMedicalHistory(int patientId)
        {
            ListViewMedicalHistory.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT visit_date, diagnosis, treatment FROM Medical_Cards WHERE id_patient = @PatientId ORDER BY visit_date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var record = new
                    {
                        VisitDate = ((DateTime)reader["visit_date"]).ToString("dd.MM.yyyy"),
                        Diagnosis = reader["diagnosis"].ToString(),
                        Treatment = reader["treatment"].ToString()
                    };
                    ListViewMedicalHistory.Items.Add(record);
                }
                conn.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxPatients.SelectedItem != null && DatePickerNewVisitDate.SelectedDate != null &&
                !string.IsNullOrEmpty(TextBoxNewDiagnosis.Text) && !string.IsNullOrEmpty(TextBoxNewTreatment.Text))
            {
                dynamic selectedPatient = ComboBoxPatients.SelectedItem;
                DateTime visitDate = DatePickerNewVisitDate.SelectedDate.Value;
                string diagnosis = TextBoxNewDiagnosis.Text;
                string treatment = TextBoxNewTreatment.Text;

                using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                {
                    string query = "INSERT INTO Medical_Cards (id_patient, id_doctor, visit_date, diagnosis, treatment) " +
                                   "VALUES (@PatientId, @DoctorId, @VisitDate, @Diagnosis, @Treatment)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientId", selectedPatient.Id);
                    cmd.Parameters.AddWithValue("@DoctorId", Dannie.id_user); // текущий врач
                    cmd.Parameters.AddWithValue("@VisitDate", visitDate);
                    cmd.Parameters.AddWithValue("@Diagnosis", diagnosis);
                    cmd.Parameters.AddWithValue("@Treatment", treatment);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Запись успешно сохранена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновление истории медицинских записей
                LoadMedicalHistory(selectedPatient.Id);
                DatePickerNewVisitDate.SelectedDate = null;
                TextBoxNewDiagnosis.Clear();
                TextBoxNewTreatment.Clear();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля для добавления записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
