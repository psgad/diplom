using System;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Diplom
{
    public partial class CodeRedactor : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        private Visibility _isFolderOpen;
        public Visibility IsFolderOpen
        {
            get => _isFolderOpen;
            set
            {
                _isFolderOpen = value;
                OnPropertyChanged(nameof(IsFolderOpen));
            }
        }
        private Visibility _folderOpen;
        public Visibility FolderOpen
        {
            get => _folderOpen;
            set
            {
                _folderOpen = value;
                IsFolderOpen = value == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                OnPropertyChanged(nameof(FolderOpen));
            }
        }

        private Visibility _consoleOpen;
        public Visibility ConsoleOpen
        {
            get => _consoleOpen;
            set
            {
                _consoleOpen = value;
                OnPropertyChanged(nameof(ConsoleOpen));
            }
        }

        private string currentFilePath;
        private bool isTextChanged;

        MainWindow parentWindow;

        public CodeRedactor(MainWindow parentWindow)
        {
            InitializeComponent();
            console.Children.Add(new ConsoleInput(this));
            DataContext = this;
            this.parentWindow = parentWindow;
            FolderOpen = Visibility.Hidden;
            CodeEditor.TextChanged += CodeEditor_TextChanged;
            CodeEditor.PreviewKeyDown += CodeEditor_PreviewKeyDown;
        }
        string path = "";
        public async Task LoadFileSystem(string path)
        {
            FolderOpen = Visibility.Visible;
            this.path = path;
            treeView.Items.Clear();
            DirectoryInfo rootDirectory = new DirectoryInfo(path);
            TreeViewItem rootItem = CreateTreeItem(rootDirectory);
            treeView.Items.Add(rootItem);

            console.Children.Clear();
            console.Children.Add(new ConsoleInput(path, this));
        }
        private async Task LoadDirectoryItems(TreeViewItem directoryItem, string directoryPath)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                foreach (var dir in directory.GetDirectories())
                {
                    directoryItem.Items.Add(CreateTreeItem(dir)); // Добавляем подкаталоги
                }
                foreach (var file in directory.GetFiles())
                {
                    var fileItem = new TreeViewItem { Header = file.Name, Tag = file.FullName };

                    // Добавляем контекстное меню для файла
                    ContextMenu fileContextMenu = new ContextMenu();
                    MenuItem renameFileMenuItem = new MenuItem { Header = "Переименовать" };
                    renameFileMenuItem.Click += (sender, e) => RenameItem(fileItem);
                    MenuItem deleteFileMenuItem = new MenuItem { Header = "Удалить" };
                    deleteFileMenuItem.Click += (sender, e) => DeleteItem(fileItem);
                    fileContextMenu.Items.Add(renameFileMenuItem);
                    fileContextMenu.Items.Add(deleteFileMenuItem);

                    fileItem.ContextMenu = fileContextMenu;
                    fileItem.Selected += FileItem_Selected;
                    directoryItem.Items.Add(fileItem); // Добавляем файл
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки содержимого папки: {ex.Message}");
            }
        }

        private TreeViewItem CreateTreeItem(DirectoryInfo directory)
        {
            TreeViewItem directoryItem = new TreeViewItem
            {
                Header = directory.Name,
                Tag = directory.FullName
            };

            // Добавляем контекстное меню для папки
            ContextMenu directoryContextMenu = new ContextMenu();
            MenuItem createFolderMenuItem = new MenuItem { Header = "Создать папку" };
            MenuItem createFileMenuItem = new MenuItem { Header = "Создать файл" };
            createFolderMenuItem.Click += (sender, e) => CreateNewItem(directoryItem, true);
            createFileMenuItem.Click += (sender, e) => CreateNewItem(directoryItem, false);
            directoryContextMenu.Items.Add(createFolderMenuItem);
            directoryContextMenu.Items.Add(createFileMenuItem);

            // Добавляем контекстное меню для папки
            MenuItem renameFolderMenuItem = new MenuItem { Header = "Переименовать" };
            renameFolderMenuItem.Click += (sender, e) => RenameItem(directoryItem);
            MenuItem deleteFolderMenuItem = new MenuItem { Header = "Удалить" };
            deleteFolderMenuItem.Click += (sender, e) => DeleteItem(directoryItem);
            directoryContextMenu.Items.Add(renameFolderMenuItem);
            directoryContextMenu.Items.Add(deleteFolderMenuItem);

            directoryItem.ContextMenu = directoryContextMenu;

            // Изначально не загружаем вложенные папки и файлы
            directoryItem.Expanded += async (sender, e) =>
            {
                if (directoryItem.Items.Count == 0) // Если содержимое ещё не загружено
                {
                    try
                    {
                        // Подгружаем содержимое только для этой папки
                        foreach (var dir in directory.GetDirectories())
                        {
                            directoryItem.Items.Add(CreateTreeItem(dir)); // Добавляем вложенные папки
                        }

                        foreach (var file in directory.GetFiles())
                        {
                            var fileItem = new TreeViewItem { Header = file.Name, Tag = file.FullName };

                            // Добавляем контекстное меню для файла
                            ContextMenu fileContextMenu = new ContextMenu();
                            MenuItem renameFileMenuItem = new MenuItem { Header = "Переименовать" };
                            renameFileMenuItem.Click += (s, ee) => RenameItem(fileItem);
                            MenuItem deleteFileMenuItem = new MenuItem { Header = "Удалить" };
                            deleteFileMenuItem.Click += (s, ee) => DeleteItem(fileItem);
                            fileContextMenu.Items.Add(renameFileMenuItem);
                            fileContextMenu.Items.Add(deleteFileMenuItem);

                            fileItem.ContextMenu = fileContextMenu;
                            fileItem.Selected += FileItem_Selected;
                            directoryItem.Items.Add(fileItem);
                        }
                    }
                    catch
                    {
                        // Ошибка при загрузке содержимого
                    }
                }
            };

            return directoryItem;
        }



        private void CreateNewItem(TreeViewItem parentItem, bool isFolder)
        {
            // Создаем текстовое поле для ввода имени нового файла/папки
            var newItemTextBox = new TextBox
            {
                Width = 200,
                Height = 25,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5),
            };

            // Добавляем текстовое поле в родительский элемент (папку)
            parentItem.Items.Add(newItemTextBox);

            // Разворачиваем родительскую папку, чтобы видно было новое текстовое поле
            parentItem.IsExpanded = true;

            // Отложенно устанавливаем фокус на TextBox
            parentItem.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                newItemTextBox.Focus(); // Устанавливаем фокус на TextBox
                newItemTextBox.SelectAll(); // Выбираем весь текст для удобства редактирования
            }));

            // Обработчик нажатия клавиш
            newItemTextBox.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    string itemName = newItemTextBox.Text.Trim();
                    if (!string.IsNullOrEmpty(itemName))
                    {
                        string parentPath = parentItem.Tag as string;
                        string itemPath = Path.Combine(parentPath, itemName);
                        try
                        {
                            if (isFolder)
                            {
                                Directory.CreateDirectory(itemPath); // Создаем папку
                                TreeViewItem newFolderItem = CreateTreeItem(new DirectoryInfo(itemPath)); // Создаем новый элемент для папки
                                parentItem.Items.Add(newFolderItem); // Добавляем папку в родительский элемент

                                // Отложенно разворачиваем и фокусируем папку
                                parentItem.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                                {
                                    newFolderItem.IsExpanded = true; // Разворачиваем новую папку
                                    newFolderItem.Focus(); // Устанавливаем фокус на только что созданную папку
                                }));
                            }
                            else
                            {
                                File.Create(itemPath).Close(); // Создаем файл
                                TreeViewItem newFileItem = new TreeViewItem { Header = itemName, Tag = itemPath }; // Новый элемент для файла
                                parentItem.Items.Add(newFileItem); // Добавляем файл в родительскую папку

                                // Отложенно устанавливаем фокус на только что созданный файл
                                parentItem.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                                {
                                    new LoadingWindow(() => LoadFileSystem(path)).ShowDialog();
                                    newFileItem.Focus(); // Переводим фокус на новый файл
                                }));
                            }

                            // Разворачиваем родительскую папку, если она не была развернута
                            parentItem.IsExpanded = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}");
                        }
                    }

                    // После создания элемента, удаляем текстовое поле
                    parentItem.Items.Remove(newItemTextBox);
                }
                else if (e.Key == Key.Escape)
                {
                    // Если Escape, отменяем создание и удаляем текстовое поле
                    parentItem.Items.Remove(newItemTextBox);
                }
            };
        }

        private void FileItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                string filePath = item.Tag.ToString();

                if (File.Exists(filePath))
                {
                    string extension = Path.GetExtension(filePath).ToLower();
                    try
                    {

                        CodeEditor.Text = File.ReadAllText(filePath);
                        currentFilePath = filePath;
                        isTextChanged = false;
                    }
                    catch
                    {
                        MessageBoxCustom.Show($"Файлы с раширением {extension} не поддерживаются данным редактором");
                        return;
                    }
                }
            }
        }

        private void RenameItem(TreeViewItem item)
        {
            if (item != null)
            {
                // Сохраняем оригинальное название элемента
                string originalName = item.Header.ToString();
                string originalPath = item.Tag.ToString();

                // Ожидаем, что это будет текстовое поле для переименования
                var renameTextBox = new TextBox
                {
                    Text = originalName, // Изначально устанавливаем в текстовое поле имя элемента
                    Width = 100,
                    Height = 25,
                };

                // Заменяем старое имя на TextBox для ввода нового имени
                item.Header = renameTextBox;

                // Используем Dispatcher, чтобы установить фокус после рендеринга
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
                {
                    renameTextBox.Focus();          // Устанавливаем фокус
                    renameTextBox.SelectAll();      // Выбираем весь текст для удобства редактирования
                }));

                // При нажатии клавиши Enter пытаемся переименовать
                renameTextBox.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        string newItemName = renameTextBox.Text.Trim();
                        if (!string.IsNullOrEmpty(newItemName) && newItemName != originalName)
                        {
                            string parentPath = Path.GetDirectoryName(originalPath);
                            string newPath = Path.Combine(parentPath, newItemName);

                            // Проверяем, если новое имя совпадает с текущим, то не переименовываем
                            if (originalPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                            {
                                // Если имя не изменилось, восстанавливаем старое имя
                                item.Header = originalName;
                                return;
                            }

                            try
                            {
                                // Переименовываем файл или папку
                                if (Directory.Exists(originalPath))
                                {
                                    Directory.Move(originalPath, newPath); // Переименовываем папку
                                }
                                else if (File.Exists(originalPath))
                                {
                                    File.Move(originalPath, newPath); // Переименовываем файл
                                }

                                // Обновляем заголовок и путь
                                item.Header = newItemName;
                                item.Tag = newPath; // Обновляем путь
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка переименования: {ex.Message}");
                            }
                        }
                        else
                        {
                            // Если имя не изменилось или пустое, восстанавливаем старое имя
                            item.Header = originalName;
                        }
                    }
                    else if (e.Key == Key.Escape)
                    {
                        // Если нажали Escape, восстанавливаем оригинальное имя
                        item.Header = originalName;
                    }
                };
            }
        }





        private void DeleteItem(TreeViewItem item)
        {
            if (item != null)
            {
                string itemPath = item.Tag.ToString();

                MessageBoxResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить элемент {item.Header}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (Directory.Exists(itemPath))
                        {
                            Directory.Delete(itemPath, true); // Удаляем папку
                        }
                        else if (File.Exists(itemPath))
                        {
                            File.Delete(itemPath); // Удаляем файл
                        }

                        // Удаляем элемент из TreeView
                        var parentItem = item.Parent as TreeViewItem;
                        if (parentItem != null)
                        {
                            parentItem.Items.Remove(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
        }


        private void CodeEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SaveFile();
                e.Handled = true;
            }
        }

        private void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            isTextChanged = true;
        }

        private void SaveFile()
        {
            if (!string.IsNullOrEmpty(currentFilePath) && isTextChanged)
            {
                File.WriteAllText(currentFilePath, CodeEditor.Text);
                MessageBox.Show("Файл успешно сохранен!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                isTextChanged = false;
            }
        }

        public void ExecutePythonCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(currentFilePath))
                {
                    MessageBox.Show("Сначала выберите файл для выполнения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string workingDirectory = Path.GetDirectoryName(currentFilePath);

                var startInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"-c \"{code}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                };

                var process = Process.Start(startInfo);
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                MessageBox.Show(string.IsNullOrEmpty(error) ? output : error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения Python-кода: {ex.Message}");
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                new LoadingWindow(() => LoadFileSystem(folderDialog.SelectedPath)).ShowDialog();
        }
        // Создание нового файла
        private void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = Path.Combine(folderDialog.SelectedPath, "new_file.py");
                File.WriteAllText(filePath, "# Новый Python файл");
                MessageBox.Show($"Файл '{filePath}' успешно создан.", "Создание файла", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем дерево файлов
                LoadFileSystem(folderDialog.SelectedPath);
            }
        }

        private void ConsoleOpenOrClose(object sender, RoutedEventArgs e)
        {

            if (ConsoleOpen == Visibility.Visible)
            {
                ConsoleOpen = Visibility.Hidden;
                console.Children.Clear();
            }
            else
            {
                ConsoleOpen = Visibility.Visible;
                console.Children.Add(new ConsoleInput(path, this));
            }
        }
    }
}
