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
    /// Логика взаимодействия для MedicalCardWindow.xaml
    /// </summary>
    public partial class MedicalCardWindow : Window
    {
        public MedicalCardWindow()
        {
            InitializeComponent();
            LoadMedicalCard();
        }

        private void LoadMedicalCard()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT Medical_Cards.visit_date, Medical_Cards.diagnosis, Medical_Cards.treatment, " +
                               "Usersz.first_name, Usersz.last_name, Usersz.sur_name " +
                               "FROM Medical_Cards " +
                               "INNER JOIN Usersz ON Medical_Cards.id_doctor = Usersz.id_user " +
                               "WHERE Medical_Cards.id_patient = @PatientId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", Dannie.id_user); 

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var visit = new
                    {
                        VisitDate = ((DateTime)reader["visit_date"]).ToString("dd.MM.yyyy"),
                        DoctorName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}",
                        Diagnosis = reader["diagnosis"].ToString(),
                        Treatment = reader["treatment"].ToString()
                    };
                    ListViewVisits.Items.Add(visit);
                }
                conn.Close();
            }
        }

        private void ListViewVisits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewVisits.SelectedItem != null)
            {
                dynamic selectedVisit = ListViewVisits.SelectedItem;
                VisitDetails.Text = $"Диагноз: {selectedVisit.Diagnosis}\n\n" +
                                    $"Лечение: {selectedVisit.Treatment}\n\n" +
                                    $"Врач: {selectedVisit.DoctorName}\n\n" +
                                    $"Дата визита: {selectedVisit.VisitDate}";
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
