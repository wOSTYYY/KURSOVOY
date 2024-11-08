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
    /// Логика взаимодействия для HistoryDialog.xaml
    /// </summary>
    public partial class HistoryDialog : Window
    {
        public HistoryDialog()
        {
            InitializeComponent();
            LoadHistory();
        }
        private void LoadHistory()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT Appointments.appointment_date, Appointments.appointment_time, Appointments.status, " +
                               "Usersz.first_name, Usersz.last_name, Usersz.sur_name " +
                               "FROM Appointments " +
                               "INNER JOIN Usersz ON Appointments.id_doctor = Usersz.id_user " +
                               "WHERE Appointments.id_patient = @PatientId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", Dannie.id_user); 

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var historyRecord = new
                    {
                        AppointmentDate = ((DateTime)reader["appointment_date"]).ToString("dd.MM.yyyy"),
                        AppointmentTime = ((TimeSpan)reader["appointment_time"]).ToString(@"hh\:mm"),
                        DoctorName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}",
                        Status = reader["status"].ToString()
                    };
                    ListViewHistory.Items.Add(historyRecord);
                }
                conn.Close();
            }

        }
    }
}
