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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = LastName.Text;
            string firstName = FirstName.Text;
            string surName = SurName.Text;
            string phone = Phone.Text;
            string email = Email.Text;
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surName) ||
                string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Dannie.connectionString))
                {
                    sqlConnection.Open();

                    string checkUserQuery = "SELECT COUNT(*) FROM Usersz WHERE login=@login";
                    SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, sqlConnection);
                    checkUserCommand.Parameters.AddWithValue("@login", login);
                    int userExists = (int)checkUserCommand.ExecuteScalar();

                    if (userExists > 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!");
                        return;
                    }

                    string insertUserQuery = "INSERT INTO Usersz (login, password, role, first_name, last_name, sur_name, phone, email) " +
                                             "VALUES (@login, @password, 1, @firstName, @lastName, @surName, @phone, @Email)";
                    SqlCommand insertUserCommand = new SqlCommand(insertUserQuery, sqlConnection);
                    insertUserCommand.Parameters.AddWithValue("@login", login);
                    insertUserCommand.Parameters.AddWithValue("@password", password);
                    insertUserCommand.Parameters.AddWithValue("@firstName", firstName);
                    insertUserCommand.Parameters.AddWithValue("@lastName", lastName);
                    insertUserCommand.Parameters.AddWithValue("@surName", surName);
                    insertUserCommand.Parameters.AddWithValue("@phone", phone);
                    insertUserCommand.Parameters.AddWithValue("@Email", email);

                    insertUserCommand.ExecuteNonQuery();
                    MessageBox.Show("Регистрация успешно завершена!");

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
