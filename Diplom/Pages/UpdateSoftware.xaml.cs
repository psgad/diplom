using Diplom.Models;
using Diplom.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для UpdateSoftware.xaml
    /// </summary>
    public partial class UpdateSoftware : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        MainWindow parentWindow;

        private string _helpTextSoftwareTypes;
        public string HelpTextSoftwareTypes
        {
            get => _helpTextSoftwareTypes;
            set
            {
                _helpTextSoftwareTypes = value;
                OnPropertyChanged(nameof(HelpTextSoftwareTypes));
            }
        }

        private List<string> _softwareTypes;
        public List<string> SoftwareTypes
        {
            get => _softwareTypes;
            set
            {
                _softwareTypes = value;
                OnPropertyChanged(nameof(SoftwareTypes));
            }
        }

        private string _selectedSoftwareType;
        public string SelectedSoftwareType
        {
            get => _selectedSoftwareType;
            set
            {
                _selectedSoftwareType = value;
                software.software_type = getSoftwareType(value);
                (PathSoftware, TitleBlockPathSoftware) = (CreateTextBox(value), GetTitleByType(value));
                OnPropertyChanged(nameof(SelectedSoftwareType));
            }
        }
        software_type getSoftwareType(string name) => new software_type(Software.getSoftwareTypes()[name]);

        private TextBox _pathSoftware;
        public TextBox PathSoftware
        {
            get => _pathSoftware;
            set
            {
                _pathSoftware = value;
                if (value.Text != "")
                    software.executable = value.Text;
                OnPropertyChanged(nameof(PathSoftware));
            }
        }
        private string _titleBlockPathSoftware;
        public string TitleBlockPathSoftware
        {
            get => _titleBlockPathSoftware;
            set
            {
                _titleBlockPathSoftware = value;
                OnPropertyChanged(nameof(TitleBlockPathSoftware));
            }
        }

        private Visibility _visibleHelpTextSelectSoftware;
        public Visibility VisibleHelpTextSelectSoftware
        {
            get => _visibleHelpTextSelectSoftware;
            set
            {
                _visibleHelpTextSelectSoftware = value;
                OnPropertyChanged(nameof(VisibleHelpTextSelectSoftware));
            }
        }

        private string _helpTextSelectSoftware;
        public string HelpTextSelectSoftware
        {
            get => _helpTextSelectSoftware;
            set
            {
                _helpTextSelectSoftware = value;
                OnPropertyChanged(nameof(HelpTextSelectSoftware));
            }
        }

        private string _addSoftwareName;
        public string AddSoftwareName
        {
            get => _addSoftwareName;
            set
            {
                _addSoftwareName = value;
                OnPropertyChanged(nameof(AddSoftwareName));
            }
        }

        private WrapPanel _listNamesSoftware;
        public WrapPanel ListNamesSoftware
        {
            get => _listNamesSoftware;
            set
            {
                _listNamesSoftware = value;
                OnPropertyChanged(nameof(ListNamesSoftware));
            }
        }

        private string _titleSoftware;
        public string TitleSoftware
        {
            get => _titleSoftware;
            set
            {
                _titleSoftware = value;
                software.title = value;
                OnPropertyChanged(nameof(TitleSoftware));
            }
        }
        string titleOldSoftware = "";
        Software software;
        bool createApp = false;

        void initialize(Software software = null)
        {
            DataContext = this;
            HelpTextSoftwareTypes = "Данное поле позволяет боту определить, что конкретно вы хотите запустить:\n" +
                 " • Приложение - обычный запуск .exe программ\n" +
                 " • Ссылка на сайт - позволяет открыть с помощью установленного по умолчанию браузера ссылку\n" +
                 " • Папка - позволяет открыть папку с помощью проводника\n" +
                 " • Самописный скрипт - позволяет запустить программу, написанную на языке программирования Python\n" +
                 " • Сочетание клавиш - позволяет нажать за вас сочетание клавиш\n" +
                 " • Файл - позволяет открыть файл с помощью более подходящих графических программ, к примеру файлы формата .docx бот попытается открыть с помощью Microsoft Word";
            SoftwareTypes = Software.getSoftwareTypes().Keys.ToList();
            ListNamesSoftware = new WrapPanel();
            if (software != null)
            {
                ListNamesSoftware.Children.Clear();
                foreach (var a in software.names)
                    ListNamesSoftware.Children.Add(createAppNameWithDelete(a));
                TitleSoftware = software.title;
                titleOldSoftware = software.title;
                SelectedSoftwareType = Software.getSoftwareTypes().First(x => x.Value == software.software_type.name).Key;
                PathSoftware.Text = software.executable;
            }
        }
        public UpdateSoftware(MainWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            createApp = true;
            software = new Software("", new List<string>(), "", new software_type(""));
            initialize();
        }
        int listIndex = 0;
        public UpdateSoftware(Software software, MainWindow parentWindow, int listIndex)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            this.listIndex = listIndex;
            this.software = software;
            initialize(software);
        }
        public TextBox CreateTextBox(string type)
        {
            TextBox textBox = new TextBox
            {
                Width = 200,
                Height = 30,
            };
            textBox.TextChanged += (s, e) => ValidateInput(textBox, type);

            return textBox;
        }
        string GetTitleByType(string type)
        {
            string title = string.Empty;
            if (type == "Приложение") title = "Путь до приложения";
            else if (type == "Ссылка на сайт") title = "Ссылка на сайт";
            else if (type == "Папка") title = "Путь до папки";
            else if (type == "Самописный скпит") title = "Путь до файла .py";
            else if (type == "Сочетание клавиш") title = "Введите сочетание клавиш";
            else if (type == "Файл") title = "Путь до файла";
            return title;
        }
        private void ValidateInput(TextBox textBox, string type)
        {
            string input = textBox.Text.Trim();
            bool isValid = false;
            string errorMessage = string.Empty;

            switch (type)
            {
                case "Приложение":
                    isValid = input.EndsWith(".exe") || input.EndsWith(".bat");
                    if (!isValid) errorMessage = "Файл должен иметь расширение .exe или .bat";
                    break;

                case "Ссылка на сайт":
                    isValid = Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                        && uriResult.Host.Contains('.');
                    if (!isValid) errorMessage = "Введите корректный URL (например, https://example.com)";
                    break;

                case "Папка":
                    isValid = System.IO.Directory.Exists(input);
                    if (!isValid) errorMessage = "Указанная папка не существует";
                    break;

                case "Самописный скрипт":
                    isValid = input.EndsWith(".py");
                    if (!isValid) errorMessage = "Файл должен иметь расширение .py";
                    break;

                case "Сочетание клавиш":
                    isValid = Regex.IsMatch(input, @"^[\w\+\-]+$");
                    if (!isValid) errorMessage = "Допустимы только буквы, цифры, + и -";
                    break;

                case "Файл":
                    isValid = System.IO.File.Exists(input);
                    if (!isValid) errorMessage = "Указанный файл не найден";
                    break;
            }

            textBox.BorderBrush = isValid ? Brushes.Green : Brushes.Red;
            HelpTextSelectSoftware = isValid ? string.Empty : errorMessage;
            VisibleHelpTextSelectSoftware = isValid ? Visibility.Hidden : Visibility.Visible;
        }

        private void AddNewNameSoftware(object sender, RoutedEventArgs e)
        {
            if (!utils.ValidateTitle(AddSoftwareName, "Название клавиши"))
                return;
            List<string> softwares = utils.getAllApps().SelectMany(x => x.names).ToList();
            softwares = softwares.Concat(ListNamesSoftware.Children.Cast<AppNameWithDelete>().Select(x => x.nameSoftware.Text).ToList()).ToList();
            if (!softwares.Contains(AddSoftwareName))
                ListNamesSoftware.Children.Add(createAppNameWithDelete(AddSoftwareName));
            else MessageBoxCustom.Show("Наименование не должно совпадать с предыдущими наименованиями приложений");
        }

        AppNameWithDelete createAppNameWithDelete(string title)
        {
            AppNameWithDelete appNameWithDelete = new AppNameWithDelete(title);
            appNameWithDelete.Width = 210;
            appNameWithDelete.deleteName.Click += (s, e) =>
            {
                ListNamesSoftware.Children.Remove(appNameWithDelete);
            };
            return appNameWithDelete;
        }

        private void UpdateOrCreateSoftware(object sender, RoutedEventArgs e)
        {
            if (!validTitleApp(TitleSoftware))
            {
                MessageBoxCustom.Show("Не пройдена проверка на название приложения");
                return;
            }
            else if (PathSoftware.BorderBrush == Brushes.Red)
            {
                MessageBoxCustom.Show($"Не пройдена проверка на поле {TitleBlockPathSoftware}");
                return;
            }
            List<Software> softwares = utils.getAllApps();
            software = new Software(TitleSoftware, ListNamesSoftware.Children.Cast<AppNameWithDelete>().Select(x => x.nameSoftware.Text).ToList(), PathSoftware.Text, getSoftwareType(SelectedSoftwareType));
            if (createApp)
                softwares.Add(software);
            else softwares[listIndex] = software;
            serializer.serialize(softwares, Settings.Default.APPS_DICT_PATH);
            parentWindow.message("Рекомендуется перезапустить бота через трей программ, так как вы внесли изменения в конфигурацию");

        }

        private void BackToApps(object sender, RoutedEventArgs e) => parentWindow.content.Content = new BotAppsSettings(parentWindow);

        private void TestAppTitle_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxCustom.Show(validTitleApp(TitleSoftware) ? "Проверка на название пройдена" : "Проверка на название не пройдена - должны соблюдаться следующие пункты:\n" +
                " - название приложения не должно повторяться с другими");
        }
        bool validTitleApp(string title)
        {
            List<string> list = utils.getAllApps().Where(x => x.title != titleOldSoftware).Select(x => x.title).ToList();
            bool test1 = !list.Contains(title);
            return test1;
        }
    }
}
