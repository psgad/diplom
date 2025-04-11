using Diplom.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using File = System.IO.File;
using System.Diagnostics;
using System.Runtime;
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

        public BotSettings()
        {
            InitializeComponent();
            DataContext = this;
            CreateLinkBot = Settings.Default.SHORTCUT;
            AutoStartBot = Settings.Default.STARTUP_BOT;
        }

        private void SaveSettingsBot_Click(object sender, RoutedEventArgs e)
        {
            utils.AddToStartup(AutoStartBot);
            utils.CreateOrDeleteDesktopShortcut(CreateLinkBot);
            MessageBoxCustom.Show("Настройки применены");

        }

        private void DeleteBot_Click(object sender, RoutedEventArgs e)
        {
            if (utils.validateAppInstall(APP_PROPERTIES.UnInstallApp))
            {
                Settings.Default.Reset();
                Process.Start(new ProcessStartInfo
                {
                    FileName = Settings.Default.UNINSTALL_APP_PATH,
                    Arguments = "/silent",
                    UseShellExecute = true,
                    Verb = "runas"
                });
            }
            else MessageBoxCustom.Show("Не найден файл для удаления приложения");
        }

        private void StartBot_Click(object sender, RoutedEventArgs e)
        {
            if (utils.IsProcessRunning(Settings.Default.APP_PATH))
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
