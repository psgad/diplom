using Diplom.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для AppView.xaml
    /// </summary>
    public partial class AppView : UserControl
    {
        Software software;

        public AppView(Software software)
        {
            this.software = software;
            InitializeComponent();
            softwareType.Text = Software.getSoftwareTypes().Keys.First(x => Software.getSoftwareTypes()[x] == software.software_type.name);
            titleSotware.Text = software.title;
        }

        public AppView()
        {
            InitializeComponent();
        }

    }
}
