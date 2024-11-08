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

    public partial class Vrach_TestResultsManagement : Window
    {
        public Vrach_TestResultsManagement()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_user, first_name, last_name, sur_name FROM Usersz WHERE role = 1"; 

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
                LoadTestResults(selectedPatient.Id);
            }
        }

        private void LoadTestResults(int patientId)
        {
            ListViewTestResults.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT test_date, result FROM Tests_Results WHERE id_patient = @PatientId ORDER BY test_date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var testResult = new
                    {
                        TestDate = ((DateTime)reader["test_date"]).ToString("dd.MM.yyyy"),
                        Result = reader["result"].ToString()
                    };
                    ListViewTestResults.Items.Add(testResult);
                }
                conn.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxPatients.SelectedItem != null && DatePickerNewTestDate.SelectedDate != null && !string.IsNullOrEmpty(TextBoxNewTestResult.Text))
            {
                dynamic selectedPatient = ComboBoxPatients.SelectedItem;
                DateTime testDate = DatePickerNewTestDate.SelectedDate.Value;
                string result = TextBoxNewTestResult.Text;

                using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                {
                    string query = "INSERT INTO Tests_Results (id_patient, test_date, result, id_doctor) VALUES (@PatientId, @TestDate, @Result, @DoctorId)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientId", selectedPatient.Id);
                    cmd.Parameters.AddWithValue("@TestDate", testDate);
                    cmd.Parameters.AddWithValue("@Result", result);
                    cmd.Parameters.AddWithValue("@DoctorId", Dannie.id_user); // текущий врач

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Результат анализа успешно сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновление списка анализов
                LoadTestResults(selectedPatient.Id);
                DatePickerNewTestDate.SelectedDate = null;
                TextBoxNewTestResult.Clear();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля для добавления результата.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
