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
    /// Логика взаимодействия для AddEditUserDialog.xaml
    /// </summary>
    public partial class AddEditUserDialog : Window
    {
        private int? _userId;

        public AddEditUserDialog(int? userId = null)
        {
            InitializeComponent();
            _userId = userId;
            LoadRoles();

            if (_userId.HasValue)
            {
                LoadUserData(_userId.Value);
                DialogTitle.Text = "Редактировать пользователя";
            }
            else
            {
                DialogTitle.Text = "Добавить пользователя";
            
            }
        }

        private void LoadRoles()
        {
            RoleComboBox.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT id_role, role_name FROM Roles";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RoleComboBox.Items.Add(new
                    {
                        RoleId = reader["id_role"],
                        RoleName = reader["role_name"].ToString()
                    });
                }
                conn.Close();
            }
            RoleComboBox.DisplayMemberPath = "RoleName";
            RoleComboBox.SelectedValuePath = "RoleId";
        }

        private void LoadUserData(int userId)
        {
            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query = "SELECT last_name, first_name, sur_name, role, phone, email, login, password FROM Usersz WHERE id_user = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    LastNameTextBox.Text = reader["last_name"].ToString();
                    FirstNameTextBox.Text = reader["first_name"].ToString();
                    SurNameTextBox.Text = reader["sur_name"].ToString();
                    RoleComboBox.SelectedValue = reader["role"];
                    PhoneTextBox.Text = reader["phone"].ToString();
                    EmailTextBox.Text = reader["email"].ToString();
                    LoginTextBox.Text = reader["login"].ToString();
                    PasswordBox.Password = reader["password"].ToString();
                }
                conn.Close();
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = LastNameTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string surName = SurNameTextBox.Text;
            int role = Convert.ToInt32(RoleComboBox.SelectedValue);
            string phone = PhoneTextBox.Text;
            string email = EmailTextBox.Text;
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password; 

            using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
            {
                string query;
                if (_userId.HasValue) // Редактирование
                {
                    query = "UPDATE Usersz SET last_name = @LastName, first_name = @FirstName, sur_name = @SurName, role = @Role, phone = @Phone, email = @Email, login = @Login, password = @Password WHERE id_user = @UserId";
                }
                else // Добавление
                {
                    query = "INSERT INTO Usersz (last_name, first_name, sur_name, role, phone, email, login, password) VALUES (@LastName, @FirstName, @SurName, @Role, @Phone, @Email, @Login, @Password)";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@SurName", surName);
                cmd.Parameters.AddWithValue("@Role", role);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", password);

                if (_userId.HasValue)
                    cmd.Parameters.AddWithValue("@UserId", _userId);

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
