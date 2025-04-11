using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для ConsoleInput.xaml
    /// </summary>
    public partial class ConsoleInput : UserControl
    {
        public string path = "";
        CodeRedactor codeRedactor;
        public ConsoleInput(string path, CodeRedactor codeRedactor)
        {
            InitializeComponent();
            this.path = path;
            this.codeRedactor = codeRedactor;
            InitializeTextBox(path);
        }
        public ConsoleInput(CodeRedactor codeRedactor)
        {
            InitializeComponent();
            path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            this.codeRedactor = codeRedactor;
            InitializeTextBox(path);

        }

        // Инициализация текста с пробелами в начале
        private void InitializeTextBox(string path)
        {
            path += "> ";
            command.Text = path;
        }
        public string get_command() => command.Text.Remove(0, path.Length + 1);

        private void command_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (command.CaretIndex < path.Length + 1)
                command.CaretIndex = path.Length + 1;
        }

        private void command_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (command.SelectionLength > 0)
                command.SelectionLength = 0;
        }

        private void command_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (command.CaretIndex < path.Length + 1)
                command.CaretIndex = path.Length + 1;
        }

        private void command_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Back && command.SelectionStart <= path.Length + 1) || (e.Key == Key.Back && Keyboard.Modifiers == ModifierKeys.Control))
                e.Handled = true;
            if (e.Key == Key.Enter)
                RunCommand(get_command());
            if (command.CaretIndex < path.Length + 2)
                command.CaretIndex = path.Length + 2;
        }

        private void command_KeyDown(object sender, KeyEventArgs e)
        {
            if (command.CaretIndex < path.Length + 1)
                command.CaretIndex = path.Length + 1;
        }
        private async Task RunCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c cd \"{path}\" && {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding(866),
                StandardErrorEncoding = Encoding.GetEncoding(866)
            };

            await Task.Run(() =>
            {
                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Выводим результат в UI (нужно использовать Dispatcher)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(output))
                            AppendToConsole(output);
                        if (!string.IsNullOrEmpty(error))
                            AppendToConsole($"Ошибка: {error}");
                    });

                    if (command.Trim() == "clear")
                        Dispatcher.Invoke(() =>
                        {
                            codeRedactor.console.Children.Clear();
                            codeRedactor.console.Children.Add(new ConsoleInput(path, codeRedactor));
                        });

                    // Если команда 'cd' была введена
                    if (command.StartsWith("cd ", StringComparison.OrdinalIgnoreCase) || command.StartsWith(" cd ", StringComparison.OrdinalIgnoreCase))
                    {
                        string newPath = command.Substring(3).Trim(); // Получаем путь после 'cd '

                        if (newPath == "..")
                        {
                            // Убираем последний каталог из пути
                            int lastBackslashIndex = path.LastIndexOf('\\');
                            if (lastBackslashIndex > 3) // Убедимся, что это не корневой путь (например, "C:\")
                            {
                                path = path.Substring(0, lastBackslashIndex);
                            }
                        }
                        else if (Directory.Exists(path + "\\" + newPath))
                        {
                            // Обновляем путь, если папка существует
                            path = path + "\\" + newPath.Replace("/", "\\");
                        }
                        Dispatcher.Invoke(() =>
                            codeRedactor.LoadFileSystem(path)
                        );
                    }
                }
            });
        }




        private void AppendToConsole(string text)
        {
            Dispatcher.Invoke(() =>
            {
                WrapPanel parent = FindParent<WrapPanel>(this);
                TextBlock textBlock = new TextBlock();
                textBlock.Text = text;
                textBlock.Width = 390;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Foreground = new SolidColorBrush(Colors.White);
                parent.Children.Add(textBlock);
                var newBlock = (new ConsoleInput(path, codeRedactor));
                parent.Children.Add(newBlock);
                FindParent<ScrollViewer>(parent).ScrollToEnd();
                command.IsEnabled = false;
                Keyboard.Focus(newBlock);
            });
        }
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }
    }
}