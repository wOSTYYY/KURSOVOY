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
    /// Логика взаимодействия для ManageTestResultsWindow.xaml
    /// </summary>
    public partial class ManageTestResultsWindow : Window
    {
        public ManageTestResultsWindow()
        {
            InitializeComponent();
            LoadTestResults();
        }

        private void LoadTestResults()
        {
            DataGridTestResults.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = @"
                    SELECT tr.id_test, u.last_name + ' ' + u.first_name AS PatientName,
                           tr.test_date, tr.result, d.last_name + ' ' + d.first_name AS DoctorName
                    FROM Tests_Results tr
                    JOIN Usersz u ON tr.id_patient = u.id_user
                    JOIN Usersz d ON tr.id_doctor = d.id_user";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var testResult = new TestResult
                    {
                        IdTest = (int)reader["id_test"],
                        PatientName = reader["PatientName"].ToString(),
                        TestDate = ((DateTime)reader["test_date"]).ToString("dd.MM.yyyy"),
                        Result = reader["result"].ToString(),
                        DoctorName = reader["DoctorName"].ToString()
                    };
                    DataGridTestResults.Items.Add(testResult);
                }
                conn.Close();
            }
        }

        private void AddTestResultButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditTestResultDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadTestResults();
            }
        }

        private void EditTestResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTestResults.SelectedItem is TestResult selectedTest)
            {
                int testId = selectedTest.IdTest;
                var dialog = new AddEditTestResultDialog(testId);
                if (dialog.ShowDialog() == true)
                {
                    LoadTestResults();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите результат анализа для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteTestResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTestResults.SelectedItem is TestResult selectedTest)
            {
                int testId = selectedTest.IdTest;

                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить этот результат анализа?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                    {
                        string query = "DELETE FROM Tests_Results WHERE id_test = @TestId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@TestId", testId);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    LoadTestResults();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите результат анализа для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaAdmin mainAdminWindow = new GlavnayaAdmin();
            mainAdminWindow.Show();
            this.Close();
        }

        private void SearchTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();
            DataGridTestResults.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = @"
            SELECT tr.id_test, u.last_name + ' ' + u.first_name AS PatientName,
                   tr.test_date, tr.result, d.last_name + ' ' + d.first_name AS DoctorName
            FROM Tests_Results tr
            JOIN Usersz u ON tr.id_patient = u.id_user
            JOIN Usersz d ON tr.id_doctor = d.id_user
            WHERE LOWER(u.last_name + ' ' + u.first_name) LIKE @SearchText OR 
                  LOWER(d.last_name + ' ' + d.first_name) LIKE @SearchText OR 
                  LOWER(tr.result) LIKE @SearchText";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var testResult = new TestResult
                    {
                        IdTest = (int)reader["id_test"],
                        PatientName = reader["PatientName"].ToString(),
                        TestDate = ((DateTime)reader["test_date"]).ToString("dd.MM.yyyy"),
                        Result = reader["result"].ToString(),
                        DoctorName = reader["DoctorName"].ToString()
                    };
                    DataGridTestResults.Items.Add(testResult);
                }
                conn.Close();
            }
        }
    }
}
