using Diplom.Models;
using System.Linq;
using System.Windows.Controls;
    
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
