using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Diplom
{
    public partial class BlockWithQuestion : UserControl
    {
        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register("HintText", typeof(string), typeof(BlockWithQuestion),
                new PropertyMetadata("", OnHintTextChanged));

        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(BlockWithQuestion),
                new PropertyMetadata("", OnTitleTextChanged));

        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(Visibility), typeof(BlockWithQuestion),
                new PropertyMetadata(Visibility.Hidden));

        public static readonly DependencyProperty FieldContentProperty =
            DependencyProperty.Register("FieldContent", typeof(object), typeof(BlockWithQuestion),
                new PropertyMetadata(null, OnFieldContentChanged));

        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public Visibility Question
        {
            get => (Visibility)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public object FieldContent
        {
            get => GetValue(FieldContentProperty);
            set => SetValue(FieldContentProperty, value);
        }

        public BlockWithQuestion()
        {
            InitializeComponent();
        }

        private static void OnHintTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockWithQuestion control && control.hintText != null)
            {
                control.hintText.Text = e.NewValue as string;
            }
        }

        private static void OnTitleTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockWithQuestion control && control.title != null)
            {
                control.title.Text = e.NewValue as string;
            }
        }

        private static void OnFieldContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockWithQuestion control && control.content != null)
            {
                control.content.Content = e.NewValue;
            }
        }

        private void QuestionCircle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(HintText))
            {
                hintPopup.IsOpen = true;
            }
        }

        private void QuestionCircle_MouseLeave(object sender, MouseEventArgs e)
        {
            hintPopup.IsOpen = false;
        }
    }
}
