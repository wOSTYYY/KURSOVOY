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
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT last_name, first_name, sur_name, login, password FROM Usersz JOIN Roles ON Usersz.role = Roles.id_role WHERE Id_user = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", Dannie.id_user);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string fullName = $"{reader["last_name"]} {reader["first_name"]} {reader["sur_name"]}";
                        FullNameLabel.Content = fullName;
                        LoginTextBox.Text = reader["login"].ToString();
                        PasswordBox.Password = reader["password"].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newLogin = LoginTextBox.Text;
            string newPassword = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(newLogin) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Usersz SET login = @Login, password = @Password WHERE id_user = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Login", newLogin);
                    cmd.Parameters.AddWithValue("@Password", newPassword); 
                    cmd.Parameters.AddWithValue("@UserId", Dannie.id_user);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GlavnayaKlient glavnayaKlient = new GlavnayaKlient();
            glavnayaKlient.Show();
            this.Hide();
        }

        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsWindow resultsWindow = new ResultsWindow();
            resultsWindow.ShowDialog();
            this.Close();
        }

        private void MedicalCardButton_Click(object sender, RoutedEventArgs e)
        {
            MedicalCardWindow medicalCardWindow = new MedicalCardWindow();
            medicalCardWindow.ShowDialog();
            this.Close();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow();
            historyWindow.Show();
            this.Close();
        }
    }
}
