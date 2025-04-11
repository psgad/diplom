using System;
using System.Collections.Generic;
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
