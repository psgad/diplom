using Diplom.Models;
using Diplom.Properties;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для LogChecker.xaml
    /// </summary>
    public partial class LogChecker : Page
    {
        MainWindow parent;
        public LogChecker(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            if (utils.validateAppInstall(APP_PROPERTIES.LogFileExists))
            {
                foreach (var text in File.ReadAllLines(Settings.Default.LOG_FILE_BOT, Encoding.UTF8)
                        .Reverse()
                        .Take(251)
                        .Reverse())
                {
                    logs.Children.Add(new TextBlock
                    {
                        Foreground = new SolidColorBrush(Colors.White),
                        Text = text,  // Добавляем перенос строки
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 16,
                        FontFamily = new FontFamily("Consolas")
                    });
                }
            }
            else MessageBoxCustom.Show("Не удалось открыть файл с логами или вы не установили приложение\nПуть до логов: " + Settings.Default.LOG_FILE_BOT);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Settings.Default.LOG_FILE_BOT, string.Empty);
            parent.content.Content = new LogChecker(parent);
        }
    }
}
