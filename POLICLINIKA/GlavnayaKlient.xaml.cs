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
    /// Логика взаимодействия для GlavnayaKlient.xaml
    /// </summary>
    public partial class GlavnayaKlient : Window
    {
        public GlavnayaKlient()
        {
            InitializeComponent();
            SetWelcomeMessage();
        }

        private void SetWelcomeMessage()
        {
            WelcomeTextBlock.Text = $"Добро пожаловать, {Dannie.Imya}!";
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            ScheduleWindow scheduleWindow = new ScheduleWindow();
            scheduleWindow.Show();
            this.Close();
        }

        private void SpecialistsButton_Click(object sender, RoutedEventArgs e)
        {
            RaspisanieWindow raspisanieWindow = new RaspisanieWindow();
            raspisanieWindow.Show();
            this.Close();
        }

        private void AppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            AppointmentWindow appointmentWindow = new AppointmentWindow();
            appointmentWindow.Show();
            this.Close();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
            this.Close();
        }
    }
}
