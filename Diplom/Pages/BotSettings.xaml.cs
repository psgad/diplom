using Diplom.Properties;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Diplom.Models;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для BotSettings.xaml
    /// </summary>
    public partial class BotSettings : Page, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _autoStartBot;
        public bool AutoStartBot
        {
            get => _autoStartBot;
            set
            {
                _autoStartBot = value;
                OnPropertyChanged(nameof(AutoStartBot));
            }
        }

        private bool _createLinkBot;
        public bool CreateLinkBot
        {
            get => _createLinkBot;
            set
            {
                _createLinkBot = value;
                OnPropertyChanged(nameof(CreateLinkBot));
            }
        }
        MainWindow parent;

        public BotSettings(MainWindow parent)
        {
            InitializeComponent();
            DataContext = this;
            CreateLinkBot = Settings.Default.SHORTCUT;
            AutoStartBot = Settings.Default.STARTUP_BOT;
        }

        private void SaveSettingsBot_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.SHORTCUT = CreateLinkBot;
            Settings.Default.STARTUP_BOT = AutoStartBot;
            Settings.Default.Save();
            utils.AddToStartup(AutoStartBot);
            utils.CreateOrDeleteDesktopShortcut(CreateLinkBot);
            MessageBoxCustom.Show("Настройки применены");

        }

        private void DeleteBot_Click(object sender, RoutedEventArgs e)
        {
            if (utils.validateAppInstall(APP_PROPERTIES.UnInstallApp))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Settings.Default.UNINSTALL_APP_PATH,
                    Arguments = "/silent",
                    UseShellExecute = true,
                    Verb = "runas"
                }).WaitForExit();
                MessageBox.Show("Бот удален!");
                Settings.Default.PropertyValues.Clear();
                Settings.Default.Reload();
                if (parent != null)
                    parent.content.Content = new Guide(parent);
            } 
            else MessageBoxCustom.Show("Не найден файл для удаления приложения");
        }

        private void StartBot_Click(object sender, RoutedEventArgs e)
        {
            if (utils.IsProcessRunning("VoiceAssistant.exe"))
                MessageBoxCustom.Show("Приложение уже запущено");
            else
            {
                try
                {

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Settings.Default.APP_PATH,
                        Verb = "runas",
                        UseShellExecute = true
                    });
                    MessageBoxCustom.Show("Успешный запуск бота");
                }
                catch
                {

                    MessageBoxCustom.Show("Приложение было не запущено по причине отмены запуска пользователем");
                }
            }
        }
    }
}
