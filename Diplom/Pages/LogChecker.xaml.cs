using Diplom.Models;
using Diplom.Properties;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                try
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
                catch (IOException e)
                {
                    MessageBoxCustom.Show("В данный момент логи нельзя открыть, так как в них производится запись ботом");
                }
                catch (Exception e)
                {
                    MessageBoxCustom.Show("Произошла ошибка: " + e.Message);
                }
            }
            else MessageBoxCustom.Show("Не удалось открыть файл с логами или вы не установили приложение\nПуть до логов: " + Settings.Default.LOG_FILE_BOT);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                File.WriteAllText(Settings.Default.LOG_FILE_BOT, string.Empty);
                parent.content.Content = new LogChecker(parent);
            }
            catch(IOException ex)
            {
                MessageBoxCustom.Show("В данный момент нельзя очистить логи, так как в них производится запись ботом");
            }
        }
    }
}
