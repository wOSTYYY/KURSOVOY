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
    /// Логика взаимодействия для GlavnayaVrach.xaml
    /// </summary>
    public partial class GlavnayaVrach : Window
    {
        public GlavnayaVrach()
        {
            InitializeComponent();
            SetWelcomeMessage();
        }

        private void SetWelcomeMessage()
        {
            WelcomeTextBlock.Text = $"Добро пожаловать, {Dannie.Imya + " " + Dannie.Otchestvo}!";
        }

        private void ViewSchelude_Click(object sender, RoutedEventArgs e)
        {
            Vrach_ViewSchedule vrach_viewSchedule = new Vrach_ViewSchedule();
            vrach_viewSchedule.Show();
            this.Close();
        }

        private void ViewPatient_Click(object sender, RoutedEventArgs e)
        {
            Vrach_PatientAppointmentsView vrach_patientAppointmentsView = new Vrach_PatientAppointmentsView();
            vrach_patientAppointmentsView.ShowDialog();
            this.Close();
        }

        private void TestResults_Click(object sender, RoutedEventArgs e)
        {
            Vrach_TestResultsManagement vrach_testResultsManagement = new Vrach_TestResultsManagement();
            vrach_testResultsManagement.Show();
            this.Close();
        }

        private void MedicalCard_Click(object sender, RoutedEventArgs e)
        {
            Vrach_MedicalCardManagement vrach_medicalCardManagement = new Vrach_MedicalCardManagement();
            vrach_medicalCardManagement.Show();
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
