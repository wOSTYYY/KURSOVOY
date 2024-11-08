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
    /// Логика взаимодействия для ManageUsersWindow.xaml
    /// </summary>
    public partial class ManageUsersWindow : Window
    {

        private const int pageSize = 5;
        private int currentPage = 1;
        private int totalPages = 0;

        public ManageUsersWindow()
        {
            InitializeComponent();
            LoadUsers(currentPage);
        }

        private void LoadUsers(int page)
        {
            DataGridUsers.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                // Сначала определим общее количество пользователей
                string countQuery = "SELECT COUNT(*) FROM Usersz";
                SqlCommand countCmd = new SqlCommand(countQuery, conn);
                conn.Open();
                int totalUsers = (int)countCmd.ExecuteScalar();
                conn.Close();

                // Вычисляем количество страниц
                totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

                // Загружаем пользователей для текущей страницы
                string query = @"
            SELECT u.id_user, u.last_name, u.first_name, u.sur_name, u.phone, u.email, u.login, u.password, r.role_name 
            FROM Usersz u 
            JOIN Roles r ON u.role = r.id_role
            ORDER BY u.id_user
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PageNumber", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    var user = new User
                    {
                        IdUser = Convert.ToInt32(row["id_user"]),
                        LastName = row["last_name"].ToString(),
                        FirstName = row["first_name"].ToString(),
                        SurName = row["sur_name"].ToString(),
                        RoleName = row["role_name"].ToString(),
                        Phone = row["phone"].ToString(),
                        Email = row["email"].ToString(),
                        Login = row["login"].ToString(),
                        Password = row["password"].ToString()
                    };

                    DataGridUsers.Items.Add(user);
                }
            }

            UpdatePageNumber();
            UpdatePaginationButtons();
        }
    
            
        

        private void UpdatePaginationButtons()
        {
            PreviousPageButton.IsEnabled = currentPage > 1;
            NextPageButton.IsEnabled = currentPage < totalPages;
        }

        private void UpdatePageNumber()
        {
            PageNumberLabel.Content = $"Страница {currentPage}";
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditUserDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadUsers(currentPage); // Обновляем список после добавления
            }
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsers.SelectedItem is User selectedUser)
            {
                int userId = selectedUser.IdUser;
                var dialog = new AddEditUserDialog(userId);
                if (dialog.ShowDialog() == true)
                {
                    LoadUsers(currentPage); // Обновляем список после редактирования
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsers.SelectedItem is User selectedUser)
            {
                int userId = selectedUser.IdUser;

                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить этого пользователя?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                    {
                        string query = "DELETE FROM Usersz WHERE id_user = @UserId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    LoadUsers(currentPage); // Обновляем список после удаления
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadUsers(currentPage);
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadUsers(currentPage);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaAdmin glavnayaAdmin = new GlavnayaAdmin();
            glavnayaAdmin.Show();
            this.Close();
        }
    }
}
