using System.Windows.Controls;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для BindingView.xaml
    /// </summary>
    public partial class BindingView : UserControl
    {
        public BindingView(string key, string name)
        {
            InitializeComponent();
            this.key.Text = key;
            this.name.Text = name;
        }
    }
}
