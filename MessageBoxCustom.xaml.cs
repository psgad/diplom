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

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для MessageBoxCustom.xaml
    /// </summary>
    public partial class MessageBoxCustom : Window
    {
        public bool Result { get; private set; } = false; // Результат (true = ОК, false = Отмена)

        public MessageBoxCustom(string message, bool showCancelButton = false)
        {
            InitializeComponent();
            MessageText.Text = message;
            if (showCancelButton)
                CancelButton.Visibility = Visibility.Visible;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
        public static bool Show(string message, bool showCancelButton = false)
        {
            try
            {

                MessageBoxCustom msgBox = new MessageBoxCustom(message, showCancelButton);
                msgBox.ShowDialog();
                return msgBox.Result;
            }
            catch
            {
                MessageBox.Show(message);
            }
            return false;
        }
    }
}
