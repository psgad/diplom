using System.Windows.Controls;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для AppNameWithDelete.xaml
    /// </summary>
    public partial class AppNameWithDelete : UserControl
    {
        public AppNameWithDelete(string title)
        {
            InitializeComponent();
            nameSoftware.Text = title;
        }
    }
}
