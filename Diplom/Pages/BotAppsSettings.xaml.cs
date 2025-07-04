using Diplom.Models;
using Diplom.Properties;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    // ДОБАВИТЬ НАДО ПОИСК ПРИЛОЖЕНИЙ И ФИЛЬТРАЦИЮ
    /// <summary>
    /// Логика взаимодействия для BotAppsSettings.xaml
    /// </summary>
    public partial class BotAppsSettings : Page
    {
        List<Software> softwares;
        MainWindow parentWindow;

        void initialize()
        {
            appsWrapPanel.Children.Clear();
            for (int i = 0; i < softwares.Count; i++)
            {
                AppView appView = new AppView(softwares[i]);
                appView.Width = 500;
                addClickByIndexDelete(appView.deleteButton, i);
                addClickByIndexUpdate(appView.editButton, i);
                appsWrapPanel.Children.Add(appView);
            }
        }

        void addClickByIndexUpdate(Button button, int i) => button.Click += (s, e) => parentWindow.content.Content = new UpdateSoftware(softwares[i], parentWindow, i);
        void addClickByIndexDelete(Button button, int i)
        {
            button.Click += (s, e) =>
            {
                softwares.Remove(softwares[i]);
                serializer.serialize(softwares, Settings.Default.APPS_DICT_PATH);
                parentWindow.message("Для корректности работы, рекомендуется перезагрузить бота, ведь вы удалили приложение из списка");
                initialize();
            };
        }

        public BotAppsSettings(MainWindow mainWindow)
        {
            InitializeComponent();
            parentWindow = mainWindow;
            if (!utils.validateAppInstall(APP_PROPERTIES.AppsDictExists))
                parentWindow.message($"Не удалось найти файл с приложкениями \"{Settings.Default.APPS_DICT_PATH}\"");
            else
            {
                softwares = utils.getAllApps();
                initialize();
            }
        }


        private void AddSoftware_Click(object sender, RoutedEventArgs e) => parentWindow.content.Content = new UpdateSoftware(parentWindow);
    }
}

