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
    /// Логика взаимодействия для ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow()
        {
            InitializeComponent();
            LoadTestResults();
        }

        private void LoadTestResults()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT Tests_Results.test_date, Tests_Results.result, Usersz.first_name, Usersz.last_name, Usersz.sur_name " +
                               "FROM Tests_Results " +
                               "INNER JOIN Usersz ON Tests_Results.id_doctor = Usersz.id_user " +
                               "WHERE Tests_Results.id_patient = @PatientId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", Dannie.id_user); 

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var testResult = new
                    {
                        Date = ((DateTime)reader["test_date"]).ToString("dd.MM.yyyy"),
                        DoctorName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}",
                        Result = reader["result"].ToString()
                    };
                    ListViewTests.Items.Add(testResult);
                }
                conn.Close();
            }
        }

        private void ListViewTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewTests.SelectedItem != null)
            {
                dynamic selectedTest = ListViewTests.SelectedItem;
                TestResult.Text = selectedTest.Result;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
            this.Close();
        }
    }
}
