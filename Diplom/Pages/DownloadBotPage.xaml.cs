using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using Diplom.Properties;
using System.Net.Http;
using System.Diagnostics;
using Diplom.Models;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Threading.Tasks;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для DownloadBotPage.xaml
    /// </summary>
    public partial class DownloadBotPage : Page, INotifyPropertyChanged
    {
        const string LICENCE = "ЛИЦЕНЗИОННОЕ СОГЛАШЕНИЕ НА ИСПОЛЬЗОВАНИЕ ПРОГРАММНОГО ОБЕСПЕЧЕНИЯ ОБЩИЕ ПОЛОЖЕНИЯ 1.1. Настоящее Лицензионное соглашение(далее — \"Соглашение\") регулирует использование программного обеспечения(далее — \"ПО\"), именуемого \"VoiceAssistant\". 1.2. Установка, запуск или иное использование ПО означает полное согласие пользователя с условиями данного Соглашения. ПРАВА И ОГРАНИЧЕНИЯ 2.1. Лицензиар предоставляет Пользователю неисключительное право на установку и использование ПО в личных или корпоративных целях. 2.2. Пользователь не имеет права: Изменять, декомпилировать, дизассемблировать или модифицировать ПО. Распространять, продавать или сдавать ПО в аренду без письменного разрешения Лицензиара. Использовать ПО для действий, нарушающих законодательство. ОТВЕТСТВЕННОСТЬ 3.1. ПО предоставляется \"как есть\" без каких-либо гарантий, явных или подразумеваемых. 3.2. Лицензиар не несёт ответственности за возможный ущерб, связанный с использованием или невозможностью использования ПО. ОБНОВЛЕНИЯ И ПОДДЕРЖКА 4.1. Лицензиар оставляет за собой право выпускать обновления и изменять функционал ПО без обязательства уведомления Пользователя. СРОК ДЕЙСТВИЯ И ПРЕКРАЩЕНИЕ 5.1. Настоящее Соглашение действует бессрочно. 5.2. Лицензиар может прекратить действие Соглашения в случае нарушения его условий Пользователем. ЗАКЛЮЧИТЕЛЬНЫЕ ПОЛОЖЕНИЯ 6.1. Настоящее Соглашение регулируется законодательством, применимым по месту регистрации Лицензиара. 6.2. Споры, возникающие в связи с исполнением Соглашения, подлежат разрешению в судебном порядке. Если вы не согласны с условиями данного Соглашения, немедленно прекратите использование ПО и удалите его со своего устройства.";

        #region Свойства
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _pathInstall;
        public string PathInstall
        {
            get { return _pathInstall; }
            set
            {
                _pathInstall = value;
                bool resultFormValid = !utils.validateAppInstall(APP_PROPERTIES.AllProperties) && Directory.Exists(value) && AcceptLicence;
                (InstallButton.IsEnabled, IsValid) = (resultFormValid, resultFormValid);
                OnPropertyChanged(nameof(PathInstall));
            }
        }
        private string _licenceText;
        public string LicenceText
        {
            get { return _licenceText; }
            set
            {
                _licenceText = value;
                OnPropertyChanged(nameof(LicenceText));
            }
        }

        private bool _acceptLicence;
        public bool AcceptLicence
        {
            get => _acceptLicence;
            set
            {
                _acceptLicence = value;
                bool resultFormValid = !utils.validateAppInstall(APP_PROPERTIES.AllProperties) && PathInstall != null && Directory.Exists(PathInstall) && value;
                (InstallButton.IsEnabled, IsValid) = (resultFormValid, resultFormValid);
                OnPropertyChanged(nameof(AcceptLicence));
            }
        }
        private bool _appendInAutoDownloads;
        public bool AppendInAutoDownloads
        {
            get => _appendInAutoDownloads;
            set
            {
                _appendInAutoDownloads = value;
                OnPropertyChanged(nameof(AppendInAutoDownloads));
            }
        }
        private bool _createLinkInDesktopFolder;
        public bool CreateLinkInDesktopFolder
        {
            get => _createLinkInDesktopFolder;
            set
            {
                _createLinkInDesktopFolder = value;
                OnPropertyChanged(nameof(CreateLinkInDesktopFolder));
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private string _hintTextInstallHelper;
        public string HintTextInstallHelper
        {
            get => _hintTextInstallHelper;
            set
            {
                _hintTextInstallHelper = value;
                OnPropertyChanged(nameof(HintTextInstallHelper));
            }
        }

        private double _progressDownload;
        public double ProgressDownload
        {
            get => _progressDownload;
            set
            {
                _progressDownload = value;
                OnPropertyChanged(nameof(ProgressDownload));
            }
        }
        #endregion
        public DownloadBotPage()
        {
            InitializeComponent();
            DataContext = this;
            AcceptLicence = false;
            LicenceText = LICENCE;
            HintTextInstallHelper = "Условия для установки должны удовлеворять следующием требования:\n1. Бот не установлен\n2. Принято лицензионное соглашение\n3. Указанный путь до установки приложения существует на вашем компьютере";
        }


        private void FolderChanged(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog changeFolder = new FolderBrowserDialog();
            var result = changeFolder.ShowDialog();

            if (result == DialogResult.OK)
                PathInstall = changeFolder.SelectedPath;
        }

        private void ChangeDesktopFolder(object sender, RoutedEventArgs e) => PathInstall = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        private void DonwloadApp(object sender, RoutedEventArgs e)
        {
            PathInstall = $@"{System.IO.Path.GetFullPath(PathInstall)}";
            if (!IsPathInstallCorrect(PathInstall))
            {
                MessageBoxCustom.Show("Путь для установки программы не найден");
                return;
            }
            mainBorder.IsEnabled = false;
            DownloadAndInstallExe("https://psgad.stariybog.ru/api/app/download");


        }

        bool IsPathInstallCorrect(string path) => Directory.Exists(path);
        private async void DownloadAndInstallExe(string exeUrl)
        {
            const int MaxRetries = 3;
            const int BufferSize = 8192;
            int attempt = 0;
            bool downloadSuccess = false;
            string installDir = Path.Combine(PathInstall, "VoiceAssistantBot");
            string exeFileName = Path.Combine(installDir, "installer.exe");

            try
            {
                if (!Directory.Exists(installDir))
                    Directory.CreateDirectory(installDir);

                while (attempt < MaxRetries && !downloadSuccess)
                {
                    attempt++;
                    try
                    {
                        using (HttpClientHandler handler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                        })
                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.Timeout = TimeSpan.FromMinutes(5); // больше времени на большой файл

                            long existingLength = 0;
                            if (File.Exists(exeFileName))
                                existingLength = new FileInfo(exeFileName).Length;

                            // Запрашиваем оставшийся контент
                            var request = new HttpRequestMessage(HttpMethod.Get, exeUrl);
                            if (existingLength > 0)
                                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);

                            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                            {
                                response.EnsureSuccessStatusCode();

                                long totalBytes = (response.Content.Headers.ContentLength ?? 0) + existingLength;
                                long bytesReceived = existingLength;

                                using (var fileStream = new FileStream(exeFileName, FileMode.Append, FileAccess.Write, FileShare.None))
                                using (var stream = await response.Content.ReadAsStreamAsync())
                                {
                                    var buffer = new byte[BufferSize];
                                    int bytesRead;
                                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                    {
                                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                                        bytesReceived += bytesRead;

                                        ProgressDownload = (double)bytesReceived / totalBytes * 100;
                                    }
                                }

                                downloadSuccess = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (attempt >= MaxRetries)
                            throw new Exception($"Не удалось скачать файл после {MaxRetries} попыток: {ex.Message}");
                        await Task.Delay(1000); // подождать перед повторной попыткой
                    }
                }

                MessageBoxCustom.Show("Файл успешно скачан!");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exeFileName,
                        Arguments = $"/silent /dir=\"{installDir}\"",
                        Verb = "runas"
                    }
                };
                process.Start();
                process.WaitForExit();

                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "VoiceAssistantBot");
                _pathInstall = installDir;

                Settings.Default.APPDATA_FOLDER_PATH = appDataPath;
                Settings.Default.APP_PATH_FOLDER = _pathInstall;
                Settings.Default.IS_APP_INSTALL = true;
                Settings.Default.APPS_DICT_PATH = Path.Combine(appDataPath, "config", "apps_dict.json");
                Settings.Default.LOG_FILE_BOT = Path.Combine(appDataPath, "config", "assistant_log.txt");
                Settings.Default.BINDINGS_PATH = Path.Combine(appDataPath, "config", "bindings.json");
                Settings.Default.SCENARIOS_PATH = Path.Combine(appDataPath, "config", "scenarios.json");
                Settings.Default.APP_PATH = Path.Combine(_pathInstall, "VoiceAssistant.exe");
                Settings.Default.SHORTCUT = CreateLinkInDesktopFolder;
                Settings.Default.STARTUP_BOT = AppendInAutoDownloads;
                Settings.Default.UNINSTALL_APP_PATH = Path.Combine(_pathInstall, "unins000.exe");
                Settings.Default.Save();

                utils.CreateOrDeleteDesktopShortcut(CreateLinkInDesktopFolder);
                utils.AddToStartup(AppendInAutoDownloads);

                mainBorder.IsEnabled = true;
                MessageBox.Show("Установка завершена.");
            }
            catch (Exception ex)
            {
                Settings.Default.IS_APP_INSTALL = false;
                Settings.Default.Save();
                MessageBoxCustom.Show($"Ошибка при скачивании или установке: {ex.Message}");
                mainBorder.IsEnabled = true;
            }
        }





        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool resultFormValid = !utils.validateAppInstall(APP_PROPERTIES.AllProperties) && PathInstall != null && Directory.Exists((sender as System.Windows.Controls.TextBox).Text) && AcceptLicence;
            (InstallButton.IsEnabled, IsValid) = (resultFormValid, resultFormValid);
        }
    }
}
