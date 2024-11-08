using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для GlavnayaAdmin.xaml
    /// </summary>
    public partial class GlavnayaAdmin : Window
    {
        public GlavnayaAdmin()
        {
            InitializeComponent();
        }

        private void ManageUsersButton_Click(object sender, RoutedEventArgs e)
        {
            ManageUsersWindow manageUsersWindow = new ManageUsersWindow();
            manageUsersWindow.Show();
            this.Close();
        }

        private void ManageSchedulesButton_Click(object sender, RoutedEventArgs e)
        {
            ManageSchedulesWindow manageSchedulesWindow = new ManageSchedulesWindow();
            manageSchedulesWindow.Show();
            this.Close();
        }

        private void ManageTestResultsButton_Click(object sender, RoutedEventArgs e)
        {
            ManageTestResultsWindow manageTestResultsWindow = new ManageTestResultsWindow();
            manageTestResultsWindow.Show();
            this.Close();
        }

        private void ManageAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            ManageAppointmentsWindow manageAppointmentsWindow = new ManageAppointmentsWindow();
            manageAppointmentsWindow.Show();
            this.Close();
        }

        private void ViewReportsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewReportsWindow viewReportsWindow = new ViewReportsWindow();
            viewReportsWindow.Show();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
