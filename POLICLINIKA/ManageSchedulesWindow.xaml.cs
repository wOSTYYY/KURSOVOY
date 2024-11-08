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
    /// Логика взаимодействия для ManageSchedulesWindow.xaml
    /// </summary>
    public partial class ManageSchedulesWindow : Window
    {
        private const int pageSize = 10;
        private int currentPage = 1;
        private int totalPages = 0;

        public ManageSchedulesWindow()
        {
            InitializeComponent();
            LoadSchedules(currentPage);
        }

        private void LoadSchedules(int page)
        {
            DataGridSchedule.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string countQuery = "SELECT COUNT(*) FROM Doctors_Schedule";
                SqlCommand countCmd = new SqlCommand(countQuery, conn);
                conn.Open();
                int totalSchedules = (int)countCmd.ExecuteScalar();
                totalPages = (int)Math.Ceiling((double)totalSchedules / pageSize);
                conn.Close();

                string query = @"
                    SELECT ds.id_schedule, ds.working_day, ds.start_time, ds.end_time, CONCAT(u.last_name, ' ', u.first_name) AS DoctorName
                    FROM Doctors_Schedule ds
                    JOIN Usersz u ON ds.id_doctor = u.id_user
                    ORDER BY ds.working_day, ds.start_time
                    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PageNumber", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    var schedule = new Schedule
                    {
                        Id = Convert.ToInt32(row["id_schedule"]),
                        WorkingDate = ((DateTime)row["working_day"]).ToString("dd.MM.yyyy"),
                        StartTime = row["start_time"].ToString(),
                        EndTime = row["end_time"].ToString(),
                        DoctorName = row["DoctorName"].ToString()
                    };

                    DataGridSchedule.Items.Add(schedule);
                }
            }

            UpdatePageNumber();
            UpdatePaginationButtons();
        }


        private void AddScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditScheduleDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadSchedules(currentPage); 
            }
        }

        private void EditScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridSchedule.SelectedItem is Schedule selectedSchedule)
            {
                int scheduleId = selectedSchedule.Id;
                var dialog = new AddEditScheduleDialog(scheduleId);
                if (dialog.ShowDialog() == true)
                {
                    LoadSchedules(currentPage);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите расписание для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridSchedule.SelectedItem is Schedule selectedSchedule)
            {
                int scheduleId = selectedSchedule.Id;

                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить это расписание?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                    {
                        string query = "DELETE FROM Doctors_Schedule WHERE id_schedule = @ScheduleId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    LoadSchedules(currentPage); 
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите расписание для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadSchedules(currentPage);
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadSchedules(currentPage);
            }
        }

        private void UpdatePaginationButtons()
        {
            PreviousPageButton.IsEnabled = currentPage > 1;
            NextPageButton.IsEnabled = currentPage < totalPages;
        }

        private void UpdatePageNumber()
        {
            PageNumberLabel.Text = $"Страница {currentPage} из {totalPages}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaAdmin glavnayaAdmin = new GlavnayaAdmin();
            glavnayaAdmin.Show();
            this.Close();
        }
    }
}
