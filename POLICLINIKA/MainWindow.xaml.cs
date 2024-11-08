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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POLICLINIKA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string comm = "Select id_user, login, password, role, first_name, last_name, sur_name FROM Usersz WHERE login='" + LoginTextBox.Text + "'AND password='" + PasswordTextBox.Password + "';";

            SqlConnection sqlConnection = new SqlConnection(Dannie.connectionString);
            SqlCommand sqlCommand = new SqlCommand(comm, sqlConnection);
            int Id_user = 0;
            string familiya = "";
            string name = "";
            string log = "";
            string otch = "";
            string pass = "";
            int Id_role = 0;

            sqlConnection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Id_user = sqlDataReader.GetInt32(0);
                familiya = sqlDataReader.GetString(5);
                name = sqlDataReader.GetString(4);
                otch = sqlDataReader.GetString(6);
                log = sqlDataReader.GetString(1);
                pass = sqlDataReader.GetString(2);
                Id_role = sqlDataReader.GetInt32(3);
            }
            sqlConnection.Close();

            if ((log == LoginTextBox.Text) && (pass == PasswordTextBox.Password) && (Id_role == 1))
            {
                MessageBox.Show("Вы успешно вошли под Клиентом");
                Dannie.Login = LoginTextBox.Text;
                Dannie.Familiya = familiya;
                Dannie.Imya = name;
                Dannie.Otchestvo = otch;
                Dannie.role = Id_role;
                Dannie.id_user = Id_user;
                GlavnayaKlient glavnayaKlient = new GlavnayaKlient();
                glavnayaKlient.Show();
                this.Hide();
            }
            else if ((log == LoginTextBox.Text) && (pass == PasswordTextBox.Password) && (Id_role == 2))
            {
                MessageBox.Show("Вы успешно вошли под Врачом");
                Dannie.Login = LoginTextBox.Text;
                Dannie.Familiya = familiya;
                Dannie.Imya = name;
                Dannie.Otchestvo = otch;
                Dannie.role = Id_role;
                Dannie.id_user = Id_user;
                GlavnayaVrach glavnayaVrach = new GlavnayaVrach();
                glavnayaVrach.Show();
                this.Hide();
            }
            else if ((log == LoginTextBox.Text) && (pass == PasswordTextBox.Password) && (Id_role == 3))
            {
                MessageBox.Show("Вы успешно вошли под Администратором");
                Dannie.Login = LoginTextBox.Text;
                Dannie.Familiya = familiya;
                Dannie.Imya = name;
                Dannie.Otchestvo = otch;
                Dannie.role = Id_role;
                Dannie.id_user = Id_user;
                GlavnayaAdmin glavnayaAdmin = new GlavnayaAdmin();
                glavnayaAdmin.Show();
                this.Hide();
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Hide();
        }
    }
}
