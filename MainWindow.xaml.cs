using Diplom.Models;
using Diplom.Properties;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Storyboard showAnim, hideAnim;
        object selectedButton = null;
        bool hideMessageBlock = false;
        public MainWindow()
        {
            InitializeComponent();
            new Task(async () =>
            {
                await Task.Delay(2000);
                Dispatcher.Invoke(() => WindowState = WindowState.Maximized);
            }).Start();
            showAnim = (Storyboard)FindResource("ShowMessageAnimation");
            hideAnim = (Storyboard)FindResource("HideMessageAnimation");
            if (!utils.validateAppInstall((APP_PROPERTIES)10))
            {
                Settings.Default.IS_APP_INSTALL = false;
                Settings.Default.Save();
            }
            Button_Click(guideButton, new RoutedEventArgs());

        }


        void selectButton(object sender)
        {
            SolidColorBrush dark = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF333333"));
            SolidColorBrush light = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));

            Button button = sender as Button;
            if (button.Background is SolidColorBrush currentBackground)
            {
                if (currentBackground.Color == light.Color)
                {
                    button.Background = dark;
                    button.Foreground = light;
                }
                else if (currentBackground.Color == dark.Color && selectedButton != sender)
                {
                    button.Background = light;
                    button.Foreground = dark;
                }
            }
            if (selectedButton != null && sender != selectedButton)
            {
                Button btn = selectedButton as Button;
                btn.Background = light;
                btn.Foreground = dark;
            }
            selectedButton = sender;
            changePage(sender);
        }

        private void changePage(object obj)
        {
            Button btn = obj as Button;
            switch ((btn.Content as TextBlock).Text)
            {
                case "Установка бота":
                    content.Content = new DownloadBotPage();
                    break;
                case "Настройка приложений бота":
                    ValidateAppInstallAndMoveToPage(() => content.Content = new BotAppsSettings(this));
                    break;
                case "Настройки бота":
                    ValidateAppInstallAndMoveToPage(() => content.Content = new BotSettings());
                    break;
                case "Добавление скрипта":
                    ValidateAppInstallAndMoveToPage(() => content.Content = new CodeRedactor(this));
                    break;
                case "Просмотр логов":
                    ValidateAppInstallAndMoveToPage(() => content.Content = new LogChecker(this));
                    break;
                case "Общие настрйоки":

                    break;
                case "Руководство пользователя":
                    content.Content = new Guide();
                    break;
            }
        }
        void ValidateAppInstallAndMoveToPage(Action action)
        {
            if (utils.validateAppInstall(APP_PROPERTIES.AppInstall))
                action();
            else MessageBoxCustom.Show("Для начала нужно установить приложение");
        }

        public void message(string message, MessageType messageType = MessageType.INFO, int delay = 5000)
        {
            if (messageType == MessageType.WARNING) { messageTitle.Text = "Предупреждение"; messageIcon.Source = new BitmapImage(new Uri("Resources/info.ico", UriKind.Relative)); }
            else if (messageType == MessageType.INFO) { messageTitle.Text = "Информация"; messageIcon.Source = new BitmapImage(new Uri("Resources/info.ico", UriKind.Relative)); }
            else if (messageType == MessageType.ERROR) { messageTitle.Text = "Ошибка"; messageIcon.Source = new BitmapImage(new Uri("Resources/error.ico", UriKind.Relative)); }
            messageText.Text = message;
            new Task(() =>
                {
                    Dispatcher.Invoke(async () =>
                    {
                        showAnim.Begin(messageGrid);
                        await Task.Delay(delay);
                        if (!hideMessageBlock)
                            hideAnim.Begin(messageGrid);
                    });
                }).Start();
            new Task(() =>
            {
                do
                {
                    hideAnim.Begin(messageGrid);
                    return;
                } while (!hideMessageBlock);
            });
        }

        #region Нажатия на объекты


        private void Button_Click(object sender, RoutedEventArgs e) => selectButton(sender);

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Environment.Exit(0);

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        #endregion

        #region Предзагрузка и управление окном
        private void Window_Loaded(object sender, RoutedEventArgs e) => WindowStartupLocation = WindowStartupLocation.CenterOwner;

        private void AdjustWindowPosition()
        {
            double workAreaWidth = SystemParameters.WorkArea.Width;
            double workAreaHeight = SystemParameters.WorkArea.Height;
            double workAreaLeft = SystemParameters.WorkArea.Left;
            double workAreaTop = SystemParameters.WorkArea.Top;

            Left = Math.Max(workAreaLeft, Math.Min(workAreaLeft + workAreaWidth - Width, Left));
            Top = Math.Max(workAreaTop, Math.Min(workAreaTop + workAreaHeight - Height, Top));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) => hideMessageBlock = true;

        #endregion
    }

}
