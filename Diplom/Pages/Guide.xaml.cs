using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Diplom
{
    public partial class Guide : Page
    {
        MainWindow parent;

        public Guide(MainWindow mainWindow)
        {
            InitializeComponent();
            parent = mainWindow;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            parent.content.Content = new Scenarios(parent);
        }

        private void ScrollToSection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string sectionName)
            {
                Section targetSection = null;

                switch (sectionName)
                {
                    case "Настройка ассистента":
                        targetSection = SectionSetup;
                        break;
                    case "Команды управления программами":
                        targetSection = SectionPrograms;
                        break;
                    case "Управление системой":
                        targetSection = SectionSystem;
                        break;
                    case "Примеры":
                        targetSection = SectionExamples;
                        break;
                    case "Поиск в интернете":
                        targetSection = SectionSearch;
                        break;
                    case "Темы оформления":
                        targetSection = SectionThemes;
                        break;
                    case "Переключение между окнами":
                        targetSection = SectionWindows;
                        break;
                    case "Нажатие клавиш":
                        targetSection = SectionKeys;
                        break;
                    case "Таблица доступных команд":
                        targetSection = SectionCommandsTable;
                        break;
                }

                if (targetSection != null)
                    ScrollToSection(targetSection);
            }
        }

        private void ScrollToSection(Section section)
        {
            // Найти RichTextBox, содержащий FlowDocument
            RichTextBox richTextBox = FindVisualChild<RichTextBox>(ContentScrollViewer);

            TextPointer sectionStart = section.ContentStart;
            richTextBox.UpdateLayout();
            Dispatcher.Invoke(() => { }, DispatcherPriority.Render);

            // Попробовать получить координаты TextPointer
            Rect rect = sectionStart.GetCharacterRect(LogicalDirection.Forward);
            if (rect != Rect.Empty)
            {
                // Вычислить целевую позицию прокрутки
                double targetOffset = rect.Y; // Абсолютная позиция секции относительно RichTextBox
                double currentOffset = ContentScrollViewer.VerticalOffset;
                double distance = targetOffset - currentOffset;

                // Настроить анимацию через DispatcherTimer
                double duration = 500; // Длительность в миллисекундах (0.5 сек)
                double steps = 30; // Количество шагов анимации
                double stepDuration = duration / steps;

                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(stepDuration)
                };

                int currentStep = 0;

                timer.Tick += (s, e) =>
                {
                    currentStep++;
                    if (currentStep >= steps)
                    {
                        ContentScrollViewer.ScrollToVerticalOffset(targetOffset);
                        timer.Stop();
                    }
                    else
                    {
                        // Применяем QuadraticEase вручную
                        double t = currentStep / steps;
                        double easedT = 1 - (1 - t) * (1 - t); // Quadratic EaseOut
                        double newOffset = currentOffset + distance * easedT;
                        ContentScrollViewer.ScrollToVerticalOffset(newOffset);
                    }
                };

                timer.Start();
            }
        }

        // Вспомогательный метод для поиска RichTextBox в визуальном дереве
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                T foundChild = FindVisualChild<T>(child);
                if (foundChild != null)
                    return foundChild;
            }
            return null;
        }
    }
}