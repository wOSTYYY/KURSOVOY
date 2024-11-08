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
    /// Логика взаимодействия для EditStatusDialog.xaml
    /// </summary>
    public partial class EditStatusDialog : Window
    {
        public string SelectedStatus { get; private set; }
        private readonly int _appointmentId;

        public EditStatusDialog()
        {
            InitializeComponent();
        }

        public EditStatusDialog(int appointmentId, string currentStatus)
        {
            InitializeComponent();
            _appointmentId = appointmentId;
            StatusComboBox.SelectedItem = StatusComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == currentStatus);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedStatus = selectedItem.Content.ToString();

                using (SqlConnection conn = new SqlConnection(Dannie.connectionString))
                {
                    string query = "UPDATE Appointments SET status = @Status WHERE id_appointment = @AppointmentId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Status", SelectedStatus);
                    cmd.Parameters.AddWithValue("@AppointmentId", _appointmentId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Статус успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите статус.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
