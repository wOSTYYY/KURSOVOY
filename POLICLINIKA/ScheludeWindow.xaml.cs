using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        private int currentPage = 1;
        private const int pageSize = 3;
        private int totalPages = 0;
        public ScheduleWindow()
        {
            InitializeComponent();
            LoadData(currentPage);
        }

        private void LoadData(int page)
        {
            string connectionString = Dannie.connectionString;
            string query = @"
                SELECT Usersz.id_user, Usersz.last_name, Usersz.first_name, Usersz.sur_name, 
                       Specializations.specialization_name, Usersz.phone, Usersz.email
                FROM Usersz
                INNER JOIN Specializations ON Usersz.id_specialization = Specializations.id_specialization
                WHERE Usersz.role = 2
                ORDER BY Usersz.id_user
                OFFSET (@PageNumber - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PageNumber", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                ListViewDoctors.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    var doctor = new
                    {
                        FIO = $"{row["last_name"]} {row["first_name"]} {row["sur_name"]}",
                        Specialization = row["specialization_name"].ToString(),
                        Phone = row["phone"].ToString(),
                        Email = row["email"].ToString()
                    };

                    ListViewDoctors.Items.Add(doctor);
                }
                SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) FROM Usersz WHERE role = 2", conn);
                conn.Open();
                int totalDoctors = (int)countCmd.ExecuteScalar();
                totalPages = (int)Math.Ceiling((double)totalDoctors / pageSize);
                conn.Close();
            }

            UpdatePaginationButtons();
            UpdatePageNumber();
        }

        private void UpdatePaginationButtons()
        {
            btnPrevious.IsEnabled = currentPage > 1;
            btnNext.IsEnabled = currentPage < totalPages;
        }

        private void UpdatePageNumber()
        {
            lblPageNumber.Content = $"Страница {currentPage}";
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadData(currentPage);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadData(currentPage);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaKlient glavnayaKlient = new GlavnayaKlient();
            glavnayaKlient.Show();
            this.Hide();
        }
    }
}
