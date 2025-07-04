using Diplom.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Diplom
{
    public partial class ActionView : UserControl, INotifyPropertyChanged
    {
        private float counter = 0;
        string value;
        float delay;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private List<string> actionTypes;
        public List<string> ActionTypes
        {
            get => actionTypes;
            set
            {
                actionTypes = value;
                OnPropertyChanged(nameof(ActionTypes));
            }
        }

        private string hintTextType;
        public string HintTextType
        {
            get => hintTextType;
            set
            {
                hintTextType = value;
                OnPropertyChanged(nameof(HintTextType));
            }
        }

        private string selectedType;
        public string SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
                createItem(value);
            }
        }

        private string counterText;
        public string CounterText
        {
            get => counterText;
            set
            {
                counterText = value;
                if (float.TryParse(value, out float newvalue) && newvalue >= 0 && newvalue <= 30)
                {
                    counter = newvalue;
                    OnPropertyChanged(nameof(CounterText));
                }
                else
                {
                    CounterText = counter.ToString();
                    OnPropertyChanged(nameof(CounterText));
                }
            }
        }

        // Expose action data for serialization
        public dynamic ActionValue
        {
            get
            {
                if (updatedBlock.FieldContent is ScrollViewer scroll && scroll.Content is TextBox textBox)
                    return textBox.Text;
                if (updatedBlock.FieldContent is ComboBox comboBox)
                    return comboBox.SelectedItem?.ToString();
                return null;
            }
        }

        public ActionView()
        {
            InitializeComponent();
            DataContext = this;
            ActionTypes = Enum.GetNames(typeof(ActionType)).ToList();
            HintTextType = "В данном блоке выбирается тип действия где\n" +
                " → software - обычное приложение, которое добавляется через пункт добавления и обновления приложений бота\n" +
                " → input_text - ввод текста\n" +
                " → press_key - нажатие кнопки, которая привязана через пункт настройки привязок клавиш к названиям\n" +
                " → system_command - внутренние команды бота\n" +
                " → sleep - задержка перед действием (используется обычно для первого действия, если нужно подождать какое то изначальное построение)";
            CounterText = 0f.ToString();
        }
        public ActionView(string type, string value, float delay)
        {
            InitializeComponent();
            DataContext = this;
            ActionTypes = Enum.GetNames(typeof(ActionType)).ToList();
            HintTextType = "В данном блоке выбирается тип действия где\n" +
                " → software - обычное приложение, которое добавляется через пункт добавления и обновления приложений бота\n" +
                " → input_text - ввод текста\n" +
                " → press_key - нажатие кнопки, которая привязана через пункт настройки привязок клавиш к названиям\n" +
                " → system_command - внутренние команды бота\n" +
                " → sleep - задержка перед действием (используется обычно для первого действия, если нужно подождать какое то изначальное построение)";
            this.value = value.ToString();
            this.delay = delay;
            SelectedType = type;
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            if (counter >= 30) return;
            counter++;
            CounterText = counter.ToString();
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            if (counter <= 0) return;
            counter--;
            CounterText = counter.ToString();
        }

        private void action_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedType != null && value != null && delay != null)
                createItem(SelectedType, value, delay);
            else if (SelectedType != null)
                createItem(SelectedType);

        }

        public void createItem(string type, string value = null, float delay = 0f)
        {
            if (type == "input_text")
            {
                updatedBlock.TitleText = "Напечатайте текст для данного действия";
                updatedBlock.HintText = "Данный блок отвечает за текст, который будет выводиться по сценарию";
                updatedBlock.Question = Visibility.Visible;
                ScrollViewer scrollViewer = new ScrollViewer()
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
                };
                scrollViewer.Content = new TextBox()
                {
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0.25),
                    Text = value ?? "",
                    Width = 250,
                    TextWrapping = TextWrapping.Wrap,
                    Height = Double.NaN,
                    Margin = new Thickness(10)
                };
                updatedBlock.FieldContent = scrollViewer;
            }
            else if (type == "software" || type == "press_key" || type == "system_command")
            {
                updatedBlock.TitleText = type == "software" ? "Выберите приложение для данного действия" :
                                        type == "press_key" ? "Выберите клавишу для данного действия" :
                                        "Выберите системное действие";
                updatedBlock.HintText = type == "software" ? "Данный блок отвечает за выбор приложения, которое будет исполнено по сценарию" :
                                       type == "press_key" ? "Данный блок отвечает за выбор клавиши, которая будет нажата по сценарию" :
                                       "Данный блок отвечает за выбор системного действия, которое будет происходить по сценарию";
                updatedBlock.Question = Visibility.Visible;
                ComboBox comboBox = new ComboBox
                {
                    ItemsSource = type == "software" ? utils.getAllApps().Select(software => software.title).ToList() :
                                  type == "press_key" ? utils.getAllBindings().Where(binding => binding.Value != "").Select(binding => binding.Value).ToList() :
                                  new List<string> { "shutdown", "restart", "sleep", "lock", "volume_up", "volume_down", "set_volume_50", "scan_system" },
                    Margin = new Thickness(10),
                };
                comboBox.SelectedItem = value;
                updatedBlock.FieldContent = comboBox;
            }
            else if (type == "pause")
            {
                updatedBlock.TitleText = "Для данного блока нужно поставить только задержку";
                updatedBlock.HintText = "";
                updatedBlock.Question = Visibility.Collapsed;
                updatedBlock.FieldContent = null;
            }
            CounterText = delay != 0 ? delay.ToString() : counter.ToString();
        }

        private void DragHandle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
            e.Handled = true;
        }

        private void ActionView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(ActionView)) is ActionView draggedElement && draggedElement != this)
            {
                var wrapPanel = VisualTreeHelper.GetParent(this) as WrapPanel;
                if (wrapPanel != null)
                {
                    int dropIndex = wrapPanel.Children.IndexOf(this);
                    wrapPanel.Children.Remove(draggedElement);
                    wrapPanel.Children.Insert(dropIndex, draggedElement);
                }
            }
            e.Handled = true;
        }
    }
}