using LiveCharts;
using LiveCharts.Wpf;
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
    /// Логика взаимодействия для ViewReportsWindow.xaml
    /// </summary>
    public partial class ViewReportsWindow : Window
    {
        public ViewReportsWindow()
        {
            InitializeComponent();
            LoadTotalStatistics();
            LoadPieChartData();
            LoadLineChartData();
        }

        private void LoadTotalStatistics()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                conn.Open();

                // Общее количество пациентов
                SqlCommand totalPatientsCmd = new SqlCommand("SELECT COUNT(*) FROM Usersz WHERE id_specialization IS NULL", conn);
                int totalPatients = (int)totalPatientsCmd.ExecuteScalar();
                TotalPatientsText.Text = totalPatients.ToString();

                // Общее количество анализов
                SqlCommand totalTestsCmd = new SqlCommand("SELECT COUNT(*) FROM Tests_Results", conn);
                int totalTests = (int)totalTestsCmd.ExecuteScalar();
                TotalTestsText.Text = totalTests.ToString();

                conn.Close();
            }
        }

        private void LoadPieChartData()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                conn.Open();

                // Распределение записей по врачам
                SqlCommand doctorRecordsCmd = new SqlCommand(
                    "SELECT u.last_name + ' ' + u.first_name AS DoctorName, COUNT(a.id_appointment) AS Count " +
                    "FROM Appointments a " +
                    "JOIN Usersz u ON a.id_doctor = u.id_user " +
                    "GROUP BY u.last_name, u.first_name", conn);

                SqlDataReader doctorReader = doctorRecordsCmd.ExecuteReader();
                SeriesCollection doctorSeries = new SeriesCollection();
                while (doctorReader.Read())
                {
                    doctorSeries.Add(new PieSeries
                    {
                        Title = doctorReader["DoctorName"].ToString(),
                        Values = new ChartValues<int> { (int)doctorReader["Count"] },
                        DataLabels = true
                    });
                }
                DoctorRecordsPieChart.Series = doctorSeries;
                doctorReader.Close();

                // Распределение результатов анализов
                SqlCommand testResultsCmd = new SqlCommand(
                    "SELECT result, COUNT(*) AS ResultCount FROM Tests_Results GROUP BY result", conn);

                SqlDataReader testReader = testResultsCmd.ExecuteReader();
                SeriesCollection testSeries = new SeriesCollection();
                while (testReader.Read())
                {
                    testSeries.Add(new PieSeries
                    {
                        Title = testReader["result"].ToString(),
                        Values = new ChartValues<int> { (int)testReader["ResultCount"] },
                        DataLabels = true
                    });
                }
                TestResultsPieChart.Series = testSeries;
                testReader.Close();

                conn.Close();
            }
        }

        private void LoadLineChartData()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                conn.Open();

                // Динамика записей по дням
                SqlCommand recordsByDateCmd = new SqlCommand(
                    "SELECT appointment_date, COUNT(*) AS AppointmentCount " +
                    "FROM Appointments GROUP BY appointment_date ORDER BY appointment_date", conn);

                SqlDataReader dateReader = recordsByDateCmd.ExecuteReader();
                ChartValues<int> recordsValues = new ChartValues<int>();
                List<string> dates = new List<string>();
                while (dateReader.Read())
                {
                    recordsValues.Add((int)dateReader["AppointmentCount"]);
                    dates.Add(((DateTime)dateReader["appointment_date"]).ToString("dd.MM"));
                }
                RecordsByDateChart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Записи по дням",
                        Values = recordsValues,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 10
                    }
                };
                RecordsByDateChart.AxisX.Add(new Axis
                {
                    Title = "Дата",
                    Labels = dates
                });
                RecordsByDateChart.AxisY.Add(new Axis
                {
                    Title = "Количество записей"
                });
                dateReader.Close();

                conn.Close();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GlavnayaAdmin mainAdminWindow = new GlavnayaAdmin();
            mainAdminWindow.Show();
            this.Close();
        }
    }
}
