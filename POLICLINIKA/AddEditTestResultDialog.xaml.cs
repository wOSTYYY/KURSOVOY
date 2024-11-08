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
    /// Логика взаимодействия для AddEditTestResultDialog.xaml
    /// </summary>
    public partial class AddEditTestResultDialog : Window
    {
        private int? _testId = null;
        public AddEditTestResultDialog(int? testId = null)
        {
            InitializeComponent();
            _testId = testId;
            LoadComboBoxes();

            if (_testId.HasValue)
            {
                LoadTestResultData();
            }
        }

        private void LoadComboBoxes()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_user, last_name + ' ' + first_name AS FullName FROM Usersz WHERE role = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ComboBoxPatients.Items.Add(new
                    {
                        Id = reader["id_user"],
                        FullName = reader["FullName"].ToString()
                    });
                }
                conn.Close();
            }

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_user, last_name + ' ' + first_name AS FullName FROM Usersz WHERE role = 2";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

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

        private void LoadTestResultData()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_patient, id_doctor, test_date, result FROM Tests_Results WHERE id_test = @TestId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TestId", _testId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ComboBoxPatients.SelectedValue = reader["id_patient"];
                    ComboBoxDoctors.SelectedValue = reader["id_doctor"];
                    DatePickerTestDate.SelectedDate = Convert.ToDateTime(reader["test_date"]);
                    TextBoxResult.Text = reader["result"].ToString();
                }
                conn.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxPatients.SelectedItem == null || ComboBoxDoctors.SelectedItem == null || DatePickerTestDate.SelectedDate == null || string.IsNullOrWhiteSpace(TextBoxResult.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedPatientId = (int)((dynamic)ComboBoxPatients.SelectedItem).Id;
            int selectedDoctorId = (int)((dynamic)ComboBoxDoctors.SelectedItem).Id;
            DateTime testDate = DatePickerTestDate.SelectedDate.Value;
            string result = TextBoxResult.Text;

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = _testId.HasValue ?
                    "UPDATE Tests_Results SET id_patient = @PatientId, id_doctor = @DoctorId, test_date = @TestDate, result = @Result WHERE id_test = @TestId" :
                    "INSERT INTO Tests_Results (id_patient, id_doctor, test_date, result) VALUES (@PatientId, @DoctorId, @TestDate, @Result)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", selectedPatientId);
                cmd.Parameters.AddWithValue("@DoctorId", selectedDoctorId);
                cmd.Parameters.AddWithValue("@TestDate", testDate);
                cmd.Parameters.AddWithValue("@Result", result);

                if (_testId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@TestId", _testId);
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
